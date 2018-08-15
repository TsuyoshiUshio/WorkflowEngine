using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace WorkflowEngine
{
    public static class Workflow
    {
        [FunctionName("Workflow")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context, ILogger log)
        {
            var workflowContext = context.GetInput<WorkflowContext>();

            log.LogInformation($"CurrentProcess: {workflowContext.CurrentProcess.ToString()}");
            switch( workflowContext.CurrentProcess) {
                case Process.Started:
                    workflowContext = await context.CallActivityAsync<WorkflowContext>("Workflow_Started", workflowContext);
                    break;
                case Process.Arrived:
                    workflowContext = await context.CallActivityAsync<WorkflowContext>("Workflow_Arrived", workflowContext);
                    break;
                default:
                    return workflowContext.Message;
            };
            context.ContinueAsNew(workflowContext);
            return workflowContext.Message;
        }

        [FunctionName("Workflow_Started")]
        public static WorkflowContext Started([ActivityTrigger] WorkflowContext context, ILogger log)
        {
            log.LogInformation($"Workflow Started");
            context.Transit(Event.Next);
            context.AppendMessage("Started:");
            return context;
        }
        [FunctionName("Workflow_Arrived")]
        public static WorkflowContext Arrived([ActivityTrigger] WorkflowContext context, ILogger log)
        {
            log.LogInformation($"Workflow Arrived");
            context.Transit(Event.Next);
            context.AppendMessage("Arrived");
            return context;
        }

        [FunctionName("Workflow_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req,
            [OrchestrationClient]DurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            var workflowContext = new WorkflowContext();

            string instanceId = await starter.StartNewAsync("Workflow", workflowContext);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}