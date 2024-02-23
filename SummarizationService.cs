using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace TicketManager
{
    public static class SummarizationService
    {
        public static IKernelBuilder builder;
        public static Kernel kernel;
        static SummarizationService()
        {
            builder = Kernel.CreateBuilder();
            builder.AddAzureOpenAIChatCompletion(Utility.GetAoaiDeploymentName(), Utility.GetAoaiEndpoint(), Utility.GetAoaiKey());
            kernel = builder.Build();
        }

        public static async Task<object> SummarizeIncidentDetails(string incidentDetails)
        {
            var prompt = @"{{$input}}
            Summarize the incident details provided below and present the summary in bullet points. The incident details include the following:
            Brief description of the incident.
            Date and time of the incident.
            Impact of the incident.
            Steps taken to resolve the incident.
            Lessons learned or recommendations for future incidents.";

            var summarize = kernel.CreateFunctionFromPrompt(prompt, executionSettings: new OpenAIPromptExecutionSettings { MaxTokens = 300 });
            //Console.WriteLine(await kernel.InvokeAsync(summarize, new() { ["input"] = incidentDetails }));
            return await kernel.InvokeAsync(summarize, new() { ["input"] = incidentDetails });
        }
    }
}
