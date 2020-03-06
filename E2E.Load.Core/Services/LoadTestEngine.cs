// <copyright file="LoadTestEngine.cs" company="Automate The Planet Ltd.">
// Copyright 2020 Automate The Planet Ltd.
// Licensed under the Apache License, Version 2.0 (the "License");
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <author>Anton Angelov</author>

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using E2E.Core;
using E2E.Load.Core.Configuration;
using E2E.Load.Core.Model;
using E2E.Load.Core.Model.Assertions;
using E2E.Load.Core.Model.Ensures;
using E2E.Load.Core.Model.Locators;
using E2E.Load.Core.Model.Results;
using E2E.Load.Core.Report;

namespace E2E.Load.Core
{
    public class LoadTestEngine
    {
        private readonly TestScenarioExecutor _testScenarioExecutor;
        private readonly ConcurrentDictionary<string, ConcurrentBag<HttpRequestDto>> _testsRequests;
        private TestScenarioMixtureDeterminer _testScenarioMixtureDeterminer;
        private List<ITestScenarioMixtureDeterminer> _testScenarioMixtureDeterminers;
        private List<LoadTestEnsureHandler> _loadTestEnsureHandler;
        private List<LoadTestLocator> _loadTestLocators;
        private ConcurrentBag<TestScenario> _testScenarios;

        public LoadTestEngine()
        {
            _testsRequests = new ConcurrentDictionary<string, ConcurrentBag<HttpRequestDto>>();
            var loadTestAssertionHandlers = new List<LoadTestAssertionHandler>();
            InitializeEnsureHandlers();
            InitializeLoadTestLocators();
            Customizations = new LoadTestCustomizations(_loadTestLocators, _loadTestEnsureHandler,
                _testScenarioMixtureDeterminers);

            // for all tests or mention to be executed only for specific? for beginning for all.
            Assertions = new LoadTestAssertions(loadTestAssertionHandlers, _loadTestLocators, _loadTestEnsureHandler);
            Settings = new LoadTestSettings();
            LoadTestsRequestsFromFiles();

            _testScenarioExecutor = new TestScenarioExecutor(loadTestAssertionHandlers);
        }

        public LoadTestSettings Settings { get; }
        public LoadTestCustomizations Customizations { get; }
        public LoadTestAssertions Assertions { get; }

        public void Execute(string resultsFilePath)
        {
            InitializeTestScenarios();
            InitializeTestScenarioMixtureDeterminers();
            var loadTestExecutionWatch = Stopwatch.StartNew();
            var loadTestRunResults = new LoadTestRunResults();
            var loadTestService = new LoadTestService();
            var testScenarioMixtureDeterminer = _testScenarioMixtureDeterminer.GetTestScenarioMixtureDeterminer();

            if (Settings.LoadTestType == LoadTestType.ExecuteForTime)
            {
                loadTestService.ExecuteForTime(Settings.NumberOfProcesses, Settings.PauseBetweenStartSeconds,
                    Settings.SecondsToBeExecuted, ExecuteTestScenario);
            }
            else if (Settings.LoadTestType == LoadTestType.ExecuteNumberOfTimes)
            {
                loadTestService.ExecuteNumberOfTimes(Settings.NumberOfProcesses, Settings.PauseBetweenStartSeconds,
                    Settings.TimesToBeExecuted, ExecuteTestScenario);
            }

            void ExecuteTestScenario()
            {
                var testScenarioResults = new TestScenarioResults();
                var testScenario = testScenarioMixtureDeterminer.GetTestScenario();

                if (loadTestRunResults.TestScenarioResults.ContainsKey(testScenario.TestName))
                {
                    testScenarioResults = loadTestRunResults.TestScenarioResults[testScenario.TestName];
                }
                else
                {
                    testScenarioResults.TestName = testScenario.TestName;
                    loadTestRunResults.TestScenarioResults.GetOrAdd(testScenario.TestName, testScenarioResults);
                }

                _testScenarioExecutor.Execute(testScenario, testScenarioResults,
                    Settings.ShouldExecuteRecordedRequestPauses, Settings.IgnoreUrlRequestsPatterns);
            }

            loadTestExecutionWatch.Stop();

            loadTestRunResults.TotalExecutionTime = loadTestExecutionWatch.Elapsed;

            var loadTestReportGenerator = new LoadTestReportGenerator();
            loadTestReportGenerator.GenerateReport(loadTestRunResults, resultsFilePath);
        }

        private void InitializeTestScenarioMixtureDeterminers()
        {
            _testScenarioMixtureDeterminers = new List<ITestScenarioMixtureDeterminer>()
            {
                new WeightTimesExecutedTestScenarioMixtureDeterminer(),
                new EqualTimesExecutedTestScenarioMixtureDeterminer(),
            };

            _testScenarioMixtureDeterminer =
                new TestScenarioMixtureDeterminer(_testScenarioMixtureDeterminers, Settings, _testScenarios);
        }

        private void InitializeTestScenarios()
        {
            _testScenarios = new ConcurrentBag<TestScenario>();

            foreach (var currentTestName in _testsRequests.Keys)
            {
                if (!ShouldFilterTestScenario(currentTestName))
                {
                    var testScenario = new TestScenario(currentTestName, _testsRequests[currentTestName]);
                    if (Settings.TestScenariosWeights.ContainsKey(currentTestName))
                    {
                        testScenario.Weight = Settings.TestScenariosWeights[currentTestName];
                    }

                    _testScenarios.Add(testScenario);
                }
            }
        }

        private bool ShouldFilterTestScenario(string testScenarioName)
        {
            bool shouldFilter = false;
            if (Settings.TestScenariosToBeExecutedPatterns.Count > 0)
            {
                shouldFilter = true;
                foreach (var currentPattern in Settings.TestScenariosToBeExecutedPatterns)
                {
                    var m = Regex.Match(testScenarioName, currentPattern, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        shouldFilter = false;
                        break;
                    }
                }
            }
            else
            {
                foreach (var currentPattern in Settings.TestScenariosNotToBeExecutedPatterns)
                {
                    var m = Regex.Match(testScenarioName, currentPattern, RegexOptions.IgnoreCase);
                    if (m.Success)
                    {
                        shouldFilter = true;
                        break;
                    }
                }
            }

            return shouldFilter;
        }

        private void InitializeEnsureHandlers()
        {
            _loadTestEnsureHandler = new List<LoadTestEnsureHandler>()
            {
                new EnsuredTextIsHandler(),
            };
        }

        private void InitializeLoadTestLocators()
        {
            _loadTestLocators = new List<LoadTestLocator>()
            {
                new ByClassLoadTestLocator(),
                new ByCssLoadTestLocator(),
                new ByIdLoadTestLocator(),
                new ByLinkTextLoadTestLocator(),
                new ByNameLoadTestLocator(),
                new ByTagLoadTestLocator(),
                new ByXPathLoadTestLocator(),
            };
        }

        private void LoadTestsRequestsFromFiles()
        {
            string allRequestsFilePath = ConfigurationService.Instance.GetLoadTestingSettings().RequestsFileLocation
                .NormalizeAppPath();
            if (!string.IsNullOrEmpty(allRequestsFilePath) || !Directory.Exists(allRequestsFilePath))
            {
                var jsonSerializer = new JsonSerializer();
                string[] allTestRequestFilePaths =
                    Directory.GetFiles(allRequestsFilePath, "*.json", SearchOption.TopDirectoryOnly);
                foreach (var testRequestFilePath in allTestRequestFilePaths)
                {
                    string content = File.ReadAllText(testRequestFilePath);
                    string testName = Path.GetFileName(testRequestFilePath);
                    try
                    {
                        var bagOfHttpRequestDtos = jsonSerializer.Deserialize<ConcurrentBag<HttpRequestDto>>(content);
                        _testsRequests.GetOrAdd(testName, bagOfHttpRequestDtos);
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException("There was a problem loading your requests file.", ex);
                    }
                }
            }
            else
            {
                throw new ArgumentException($"The requests files directory was not found- {allRequestsFilePath}");
            }
        }
    }
}