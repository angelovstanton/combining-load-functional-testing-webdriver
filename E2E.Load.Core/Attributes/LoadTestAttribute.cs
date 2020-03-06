﻿// <copyright file="LoadTestAttribute.cs" company="Automate The Planet Ltd.">
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
using E2E.Core;
using E2E.Load.Core.Configuration;

namespace E2E.Load.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LoadTestAttribute : Attribute
    {
        public LoadTestAttribute()
        {
            ShouldRecordHostRequestsOnly =
                ConfigurationService.Instance.GetLoadTestingSettings().ShouldRecordHostRequestsOnly;
            Host = ConfigurationService.Instance.GetLoadTestingSettings().DefaultHost;
        }

        public LoadTestAttribute(bool shouldRecordHostRequestsOnly)
        {
            ShouldRecordHostRequestsOnly = shouldRecordHostRequestsOnly;
            Host = ConfigurationService.Instance.GetLoadTestingSettings().DefaultHost;
        }

        public LoadTestAttribute(bool shouldRecordHostRequestsOnly, string host)
        {
            ShouldRecordHostRequestsOnly = shouldRecordHostRequestsOnly;
            Host = host;
        }

        public LoadTestAttribute(string host, bool shouldRecordHostRequestsOnly)
        {
            Host = host;
            ShouldRecordHostRequestsOnly = shouldRecordHostRequestsOnly;
        }

        public string Host { get; }

        public bool ShouldRecordHostRequestsOnly { get; }
    }
}