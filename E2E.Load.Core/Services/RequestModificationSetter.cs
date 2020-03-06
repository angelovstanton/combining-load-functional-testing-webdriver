using System;
using System.Linq;

namespace E2E.Load.Core.Services
{
    public static class RequestModificationSetter
    {
        public static void SetRequestModification(string actionValue)
        {
            if (LoadTestingWorkflowPluginContext.IsLoadTestingEnabled &&
                !string.IsNullOrEmpty(LoadTestingWorkflowPluginContext.CurrentTestName) &&
                LoadTestingWorkflowPluginContext.HttpRequestsPerTest != null &&
                LoadTestingWorkflowPluginContext.HttpRequestsPerTest.ContainsKey(LoadTestingWorkflowPluginContext
                    .CurrentTestName))
            {
                var currentHttpRequests =
                    LoadTestingWorkflowPluginContext.HttpRequestsPerTest[
                        LoadTestingWorkflowPluginContext.CurrentTestName];
                var htmlRequest = currentHttpRequests.LastOrDefault(x => x.Headers.Any(y => y.Contains("text/html")));
                htmlRequest?.RequestModifications.Add(new RequestModification(actionValue));
            }
        }
    }
}