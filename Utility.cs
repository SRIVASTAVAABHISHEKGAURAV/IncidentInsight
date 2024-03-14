using IncidentInsight;
using Microsoft.Extensions.Configuration;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace TicketManager
{
    public static class Utility
    {
        private static readonly IConfiguration config;
        static Utility()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName)
               .AddJsonFile("config.json", optional: false);

            config = builder.Build();
        }

        public static WorkItemTrackingHttpClient GetClient()
        {
            var connection = CreateConnection();
            return connection.GetClient<WorkItemTrackingHttpClient>();
        }
        public static string GetEnvironment() => config.GetSection("Secrets").Get<Secret>().Environment;
        public static string GetPatToken() => config.GetSection("Secrets").Get<Secret>().PATToken;
        public static string GetOrgUri() => config.GetSection("Secrets").Get<Secret>().Uri;
        public static Uri GetUri() => new Uri(GetOrgUri());
        public static string GetProject() => config.GetSection("Secrets").Get<Secret>().Project;
        public static string GetAoaiDeploymentName() => config.GetSection("Secrets").Get<Secret>().AoaiDeploymentName;
        public static string GetAoaiEndpoint() => config.GetSection("Secrets").Get<Secret>().AoaiEndpoint;
        public static string GetAoaiKey() => config.GetSection("Secrets").Get<Secret>().AoaiKey;
        public static string GetApiToken() => config.GetSection("Secrets").Get<Secret>().ApiToken;
        public static string GetUserName() => config.GetSection("Secrets").Get<Secret>().UserName;
        public static string GetJiraBaseUrl() => config.GetSection("Secrets").Get<Secret>().JiraBaseUrl;
        public static string GetPrompt() => config.GetSection("Secrets").Get<Secret>().Prompt;
        public static int GetMaxTokens() => config.GetSection("Secrets").Get<Secret>().MaxTokens;
        public static string CleanHtmlTags(string input) => Regex.Replace(input, "<.*?>", string.Empty);
        public static void CaptureScreenShot(object summary)
        {
            Console.WriteLine(summary);
            Thread.Sleep(2000);
            var folderPath = FileHelper.GetFolderPath(Utility.GetEnvironment(), "screenshot");
            ScreenCapture.CaptureScreen(folderPath);
        }

        public static string GetFormattedText(List<string> comments)
        {
            string combinedComments = string.Join(Environment.NewLine, comments);

            string cleanedText = Utility.CleanHtmlTags(combinedComments);

            return cleanedText;
        }
        private static VssConnection CreateConnection() => new VssConnection(new Uri(GetOrgUri()), new VssBasicCredential(string.Empty, GetPatToken()));

        private class Secret
        {
            public string PATToken { get; set; }
            public string Uri { get; set; }
            public string Project { get; set; }
            public string AoaiDeploymentName { get; set; }
            public string AoaiEndpoint { get; set; }
            public string AoaiKey { get; set; }
            public string Prompt { get; set; }
            public int MaxTokens { get; set; }
            public string ApiToken { get; set; }
            public string UserName { get; set; }
            public string JiraBaseUrl { get; set; }
            public string Environment { get; set; }
        }
    }
}
