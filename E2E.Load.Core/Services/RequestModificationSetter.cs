﻿// <copyright file="RequestModificationSetter.cs" company="Automate The Planet Ltd.">
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