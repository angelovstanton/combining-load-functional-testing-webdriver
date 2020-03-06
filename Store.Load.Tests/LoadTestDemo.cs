using E2E.Load.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Store.Load.Tests
{
    [TestClass]
    public class LoadTestDemo
    {
        [TestMethod]
        public void PurchasingDemoWebSiteLoadTest()
        {
            var loadTestEngine = new LoadTestEngine();
            loadTestEngine.Settings.LoadTestType = LoadTestType.ExecuteForTime;
            loadTestEngine.Settings.MixtureMode = MixtureMode.Equal;
            loadTestEngine.Settings.NumberOfProcesses = 2;
            loadTestEngine.Settings.PauseBetweenStartSeconds = 0;
            loadTestEngine.Settings.SecondsToBeExecuted = 30;
            loadTestEngine.Settings.ShouldExecuteRecordedRequestPauses = true;
            loadTestEngine.Settings.IgnoreUrlRequestsPatterns.Add(".*theming.js.*");
            loadTestEngine.Settings.IgnoreUrlRequestsPatterns.Add(".*loginHash.*");
            loadTestEngine.Settings.TestScenariosToBeExecutedPatterns.Add(".*.CompletePurchaseSuccessfully_WhenNewClient_SingleItem");
            loadTestEngine.Assertions.AssertAllRequestStatusesAreSuccessful();
            loadTestEngine.Assertions.AssertAllRecordedEnsureAssertions();
            loadTestEngine.Execute("loadTestResults.html");
        }
    }
}