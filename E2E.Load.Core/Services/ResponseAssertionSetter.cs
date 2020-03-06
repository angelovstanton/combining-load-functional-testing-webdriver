using System.Diagnostics;
using System.Linq;

namespace E2E.Load.Core.Services
{
    public static class ResponseAssertionSetter
    {
        public static void AddResponseAssertionToHttpRequest(string locatorType, string locatorValue,
            string actionValue)
        {
            if (LoadTestingWorkflowPluginContext.IsLoadTestingEnabled
                && !string.IsNullOrEmpty(LoadTestingWorkflowPluginContext.CurrentTestName)
                && LoadTestingWorkflowPluginContext.HttpRequestsPerTest != null
                && LoadTestingWorkflowPluginContext.HttpRequestsPerTest.ContainsKey(LoadTestingWorkflowPluginContext
                    .CurrentTestName))
            {
                var currentHttpRequests = LoadTestingWorkflowPluginContext
                    .HttpRequestsPerTest[LoadTestingWorkflowPluginContext.CurrentTestName].OrderBy(x => x.CreationTime);
                var httpRequest = currentHttpRequests.LastOrDefault(x => x.Headers.Any(y => y.Contains("text/html")));
                if (httpRequest != null)
                {
                    if (httpRequest.Headers.Any(x => x.Contains("Referer")))
                    {
                        string refererHeader = httpRequest.Headers.FirstOrDefault(x => x.Contains("Referer"));
                        string refererUrl = refererHeader?.Trim().Split(' ').LastOrDefault()?.Trim();
                        if (!string.IsNullOrEmpty(refererUrl))
                        {
                            var refererHttpRequest = currentHttpRequests.LastOrDefault(x =>
                                x.Headers.Any(y => y.Contains("text/html")) && x.Url.Equals(refererUrl));
                            if (refererHttpRequest != null)
                            {
                                httpRequest = refererHttpRequest;
                            }
                        }
                    }

                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    var currentMethodName = sf.GetMethod();

                    var responseAssert = new ResponseAssertion
                    {
                        AssertionType = currentMethodName.Name.Replace("EventHandler", string.Empty),
                        ExpectedValue = actionValue,
                        Locator = locatorType,
                        LocatorValue = locatorValue,
                    };
                    httpRequest.ResponseAssertions.Add(responseAssert);
                }
            }
        }
    }
}