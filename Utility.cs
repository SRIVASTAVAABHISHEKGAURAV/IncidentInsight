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
        public static string GetPatToken() => config.GetSection("Secrets").Get<Secret>().PATToken;
        public static string GetOrgUri() => config.GetSection("Secrets").Get<Secret>().Uri;
        public static Uri GetUri() => new Uri(GetOrgUri());
        public static string GetProject() => config.GetSection("Secrets").Get<Secret>().Project;
        public static string GetAoaiDeploymentName() => config.GetSection("Secrets").Get<Secret>().AoaiDeploymentName;
        public static string GetAoaiEndpoint() => config.GetSection("Secrets").Get<Secret>().AoaiEndpoint;
        public static string GetAoaiKey() => config.GetSection("Secrets").Get<Secret>().AoaiKey;
        public static int GetMaxTokens() => config.GetSection("Secrets").Get<Secret>().MaxTokens;
        public static string CleanHtmlTags(string input) => Regex.Replace(input, "<.*?>", string.Empty);
        private static VssConnection CreateConnection() => new VssConnection(new Uri(GetOrgUri()), new VssBasicCredential(string.Empty, GetPatToken()));

        private class Secret
        {
            public string PATToken { get; set; }
            public string Uri { get; set; }
            public string Project { get; set; }
            public string AoaiDeploymentName { get; set; }
            public string AoaiEndpoint { get; set; }
            public string AoaiKey { get; set; }
            public int MaxTokens { get; internal set; }
        }
    }
}
