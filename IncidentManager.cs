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
            if(!FileHelper.FileExists(workItemId))
            {
                combinedComments = await GetWorkItemDetails(workItemId);
                await FileHelper.SaveContentAsync(combinedComments, workItemId);
            }
            else
            {
                combinedComments = FileHelper.GetFileContent(workItemId);
            }
            
            Console.WriteLine(await SummarizationService.SummarizeIncidentDetails(combinedComments));
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

        private static void CreateNewIncident()
        {
            Console.WriteLine("Creating Incident...");
            try
            {
                CreateIncidentUsingClientLib();
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error creating Incident Main: {0}", ex.InnerException.Message);
                throw ex;
            }
        }

        public static WorkItem CreateIncidentUsingClientLib()
        {

            var credentials = new VssBasicCredential("", Utility.GetPatToken());
            var patchDocument = new JsonPatchDocument();

            //add fields and their values to your patch document
            CreatePatchDcoument(ref patchDocument);

            //VssConnection connection = new VssConnection(_uri, credentials);
            //WorkItemTrackingHttpClient workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                WorkItem result = witClient.CreateWorkItemAsync(patchDocument, Utility.GetProject(), "Incident").Result;

                if (result != null && result.Id > 0)
                {
                    var comment = witClient.AddCommentAsync(new CommentCreate() { Text = "Microsoft team will be looking into this incident and update you further on this" }, Utility.GetProject(), result.Id.Value).Result;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Successfully Created: Incident #{0}", result.Id);
                }

                return result;
            }
            catch (AggregateException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error creating Incident: {0}", ex.InnerException.Message);
                return null;
            }
        }

        private static void CreatePatchDcoument(ref JsonPatchDocument patchDocument)
        {
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.Title",
                    Value = "Please Enter Title........"
                }
            );

            patchDocument.Add(
                 new JsonPatchOperation()
                 {
                     Operation = Operation.Add,
                     Path = "/fields/System.Description",
                     Value = "Please Enter Description......"
                 }
             );
            patchDocument.Add(
                 new JsonPatchOperation()
                 {
                     Operation = Operation.Add,
                     Path = "/fields/System.AreaPath",
                     Value = "DTC\\WS6\\WS6-Operations-Support"
                 }
             );
            patchDocument.Add(
                 new JsonPatchOperation()
                 {
                     Operation = Operation.Add,
                     Path = "/fields/System.State",
                     Value = "New"
                 }
             );
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/System.IterationPath",
                    Value = Utility.GetProject()
                }
            );

            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Custom.NBAIncidentPriority",
                    Value = "P4 - Low"
                }
            );

            patchDocument.Add(
              new JsonPatchOperation()
              {
                  Operation = Operation.Add,
                  Path = "/fields/Custom.NBAIncidentIncidentSeverity",
                  Value = "S4 - Low"
              }
          );

            patchDocument.Add(
              new JsonPatchOperation()
              {
                  Operation = Operation.Add,
                  Path = "/fields/Custom.NBAIncidentIncidentType",
                  Value = "Mission Control Dashboard"
              }
          );

            patchDocument.Add(
             new JsonPatchOperation()
             {
                 Operation = Operation.Add,
                 Path = "/fields/Custom.NBAIncidentDeviceType",
                 Value = "Web"
             }
         );

            patchDocument.Add(
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/Custom.NBAIncidentEnvironment",
                Value = "PROD"
            }
        );

            patchDocument.Add(
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/Custom.NBAIncidentPlatform",
                Value = "Web"
            }
        );

            patchDocument.Add(
            new JsonPatchOperation()
            {
                Operation = Operation.Add,
                Path = "/fields/Custom.NBAIncidentTicketcreatedDate",
                Value = DateTime.UtcNow
            }
        );

            //Custom.NBAIncidentTicketRaisedBy =;
            //Custom.LESTag = LES;
            //Custom.NBAIncidentTicketcreatedDate
        }
    }

}
