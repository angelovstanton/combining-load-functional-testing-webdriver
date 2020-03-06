// // <copyright file="LoadTestDemo.cs" company="Automate The Planet Ltd.">
// // Copyright 2020 Automate The Planet Ltd.
// // Licensed under the Apache License, Version 2.0 (the "License");
// // You may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.
// // </copyright>
// // <author>Anton Angelov</author>

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
            loadTestEngine.Settings.TestScenariosToBeExecutedPatterns.Add(
                ".*.CompletePurchaseSuccessfully_WhenNewClient_SingleItem");
            loadTestEngine.Assertions.AssertAllRequestStatusesAreSuccessful();
            loadTestEngine.Assertions.AssertAllRecordedEnsureAssertions();
            loadTestEngine.Execute("loadTestResults.html");
        }
    }
}