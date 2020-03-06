// <copyright file="LoadTestingSettings.cs" company="Automate The Planet Ltd.">
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

using System.Collections.Generic;

namespace E2E.Load.Core.Configuration
{
    public class LoadTestingSettings
    {
        public LoadTestingSettings()
        {
            CertificatePaths = new List<string>();
            IgnoreUrlRequestsPatterns = new List<string>();
        }

        public string RequestsFileLocation { get; set; }
        public bool IsEnabled { get; set; }
        public bool ShouldRecordHostRequestsOnly { get; set; }
        public List<string> IgnoreUrlRequestsPatterns { get; set; }
        public string DefaultHost { get; set; }
        public List<string> CertificatePaths { get; set; }
    }
}