using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using IncidentInsight;

namespace TicketManager
{
    public static class IncidentManager
    {
        private static readonly WorkItemTrackingHttpClient witClient;
        private static string combinedComments;
        static IncidentManager()
        {
            witClient = Utility.GetClient();
        }

        public static async Task ProcessWorkItem(int workItemId)
        {
            if(!FileHelper.FileExists(workItemId, "data"))
            {
                combinedComments = await GetWorkItemDetails(workItemId);
                await FileHelper.SaveContentAsync(combinedComments, workItemId, "data");
            }
            else
            {
                combinedComments = FileHelper.GetFileContent(workItemId, "data");
            }
            
            var summary = await SummarizationService.SummarizeIncidentDetails(combinedComments);
            Console.WriteLine(summary);
            await FileHelper.SaveContentAsync(Convert.ToString(summary), workItemId, "result");

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

            string combinedComments = string.Join(Environment.NewLine, comments);

            string cleanedText = Utility.CleanHtmlTags(combinedComments);

            return cleanedText;
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
