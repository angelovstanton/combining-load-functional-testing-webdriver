// <copyright file="BaseTest.cs" company="Automate The Planet Ltd.">
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
using System.IO;
using System.Reflection;
using E2E.Core;
using E2E.Load.Core;
using E2E.Load.Core.Attributes;
using E2E.Load.Core.Configuration;
using E2E.Load.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace E2E.Web.Core
{
    [TestClass]
    public class BaseTest
    {
        private static readonly string
            _assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private LoadTestingWorkflowPluginContext _loadTestingWorkflowPluginContext;
        private ProxyService _proxyService;

        public TestContext TestContext { get; set; }
        public IDriver Driver { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            var chromeDriverService = ChromeDriverService.CreateDefaultService(_assemblyFolder);
            var chromeOptions = new ChromeOptions();

            if (ConfigurationService.Instance.GetLoadTestingSettings().IsEnabled &&
                ConfigurationService.Instance.GetWebProxySettings().IsEnabled)
            {
                _proxyService = new ProxyService();
                _loadTestingWorkflowPluginContext = new LoadTestingWorkflowPluginContext();
                _proxyService.ProxyServer.BeforeRequest +=
                    _loadTestingWorkflowPluginContext.OnRequestCaptureTrafficEventHandler;
                _proxyService.Start();
                var webDriverProxy = new Proxy
                {
                    HttpProxy = $"http://127.0.0.1:{_proxyService.Port}",
                    SslProxy = $"http://127.0.0.1:{_proxyService.Port}",
                };
                chromeOptions.Proxy = webDriverProxy;
            }

            Driver = new DriverAdapter(new ChromeDriver(chromeDriverService, chromeOptions));

            InitializeLoadTestingEngine();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _loadTestingWorkflowPluginContext.PostTestCleanup();
            Driver.Close();
            _proxyService.Dispose();
        }

        private void InitializeLoadTestingEngine()
        {
            var loadTestingAttribute = GetOverridenAttribute();
            if (loadTestingAttribute == null)
            {
                LoadTestingWorkflowPluginContext.IsLoadTestingEnabled = false;
            }
            else
            {
                LoadTestingWorkflowPluginContext.IsLoadTestingEnabled =
                    ConfigurationService.Instance.GetLoadTestingSettings().IsEnabled;
                LoadTestingWorkflowPluginContext.ShouldFilterByHost = loadTestingAttribute.ShouldRecordHostRequestsOnly;
                LoadTestingWorkflowPluginContext.FilterHost = loadTestingAttribute.Host;
            }

            LoadTestingWorkflowPluginContext.CurrentTestName =
                $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}";

            _loadTestingWorkflowPluginContext.PreTestInit();
        }

        private LoadTestAttribute GetOverridenAttribute()
        {
            var classAttribute = GetClassAttribute();
            var methodAttribute = GetMethodAttribute();
            if (methodAttribute != null)
            {
                return methodAttribute;
            }

            return classAttribute;
        }

        private LoadTestAttribute GetClassAttribute()
        {
            var classAttribute = GetCurrentExecutionTestClassType().GetCustomAttribute<LoadTestAttribute>(true);

            return classAttribute;
        }

        private LoadTestAttribute GetMethodAttribute()
        {
            var methodAttribute = GetCurrentExecutionMethodInfo()?.GetCustomAttribute<LoadTestAttribute>(true);

            return methodAttribute;
        }

        private MethodInfo GetCurrentExecutionMethodInfo()
        {
            var memberInfo = GetType().GetMethod(TestContext.TestName);
            return memberInfo;
        }

        private Type GetCurrentExecutionTestClassType()
        {
            string className = TestContext.FullyQualifiedTestClassName;
            var testClassType = GetType().Assembly.GetType(className);

            return testClassType;
        }
    }
}