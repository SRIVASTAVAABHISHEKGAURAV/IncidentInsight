using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using IncidentInsight;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace TicketManager
{
    public static class IncidentManager
    {
        private static readonly WorkItemTrackingHttpClient witClient;
        private static readonly string jiraBaseUrl;
        private static readonly string apiToken;
        private static readonly string username;
        private static string combinedComments;
        static IncidentManager()
        {
            witClient = Utility.GetClient();
            jiraBaseUrl = Utility.GetJiraBaseUrl();
            apiToken = Utility.GetApiToken();
            username = Utility.GetUserName();
        }

        public static async Task ProcessWorkItem(int workItemId)
        {
            if (!FileHelper.FileExists(Convert.ToString(workItemId), "data"))
            {
                combinedComments = await GetWorkItemDetails(workItemId);
                await FileHelper.SaveContentAsync(combinedComments, Convert.ToString(workItemId), "data");
            }
            else
            {
                combinedComments = FileHelper.GetFileContent(Convert.ToString(workItemId), "data");
            }
            var summary = await Summarize(combinedComments);
            Console.WriteLine(summary);
            //Utility.CaptureScreenShot(summary);
            await FileHelper.SaveContentAsync(Convert.ToString(summary), Convert.ToString(workItemId), "result");
        }

        public static async Task ProcessWorkItem(string workItemId)
        {
            if (!FileHelper.FileExists(workItemId, "data"))
            {
                combinedComments = await GetIssueKeyDetails(workItemId);
                await FileHelper.SaveContentAsync(combinedComments, workItemId, "data");
            }
            else
            {
                combinedComments = FileHelper.GetFileContent(workItemId, "data");
            }
            var summary = await Summarize(combinedComments);
            Console.WriteLine(summary);
            //Utility.CaptureScreenShot(summary);
            await FileHelper.SaveContentAsync(Convert.ToString(summary), workItemId, "result");
        }

        private static async Task<object> Summarize(string summarytext)
        {
            var summary = await SummarizationService.SummarizeIncidentDetails(summarytext);
            return summary;
        }
        private static async Task<string> GetIssueKeyDetails(string issueKey)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(jiraBaseUrl);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{apiToken}")));

                HttpResponseMessage response = await client.GetAsync($"/rest/api/3/issue/{issueKey}/comment");

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(responseData);

                    JArray comments = (JArray)json["comments"];

                    var commentTexts = new List<string>();
                    foreach (JToken comment in comments)
                    {
                        string commentText = Convert.ToString(comment["body"]["content"][0]["content"][0]["text"]);

                        if (!string.IsNullOrEmpty(commentText))
                        {
                            commentTexts.Add(commentText);
                        }
                    }

                    return Utility.GetFormattedText(commentTexts);
                }
                else
                {
                    Console.WriteLine($"Failed to fetch issue comments. Status code: {response.StatusCode}");
                    return String.Empty;
                }
            }
        }

        private static async Task<string> GetWorkItemDetails(int workItemId)
        {

            WorkItem incident = await witClient.GetWorkItemAsync(workItemId, expand: WorkItemExpand.Relations);

            List<WorkItem> revisions = await witClient.GetRevisionsAsync(Utility.GetProject(), workItemId);

            var comments = new List<string>();
            foreach (var revision in revisions)
            {
                if (revision.Fields.ContainsKey("System.History"))
                {
                    var history = Convert.ToString(revision.Fields["System.History"]);
                    comments.Add(history);
                }
            }

            return Utility.GetFormattedText(comments);
        }

        private static async Task SaveResult(string data, string id)
        {
            await FileHelper.SaveContentAsync(data, id, "result");
        }

        private static async Task GetWorkItemById(int workItemId)
        {
            try
            {
                WorkItem workitem = await witClient.GetWorkItemAsync(workItemId);

                foreach (var field in workitem.Fields)
                {
                    Console.WriteLine("  {0}: {1}", field.Key, field.Value);
                }
            }
            catch (AggregateException aex)
            {
                VssServiceException vssex = aex.InnerException as VssServiceException;
                if (vssex != null)
                {
                    Console.WriteLine(vssex.Message);
                }
            }
        }

    }
}
