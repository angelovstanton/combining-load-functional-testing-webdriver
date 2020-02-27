﻿// <copyright file="EnsuredAltIsNullHandler.cs" company="Automate The Planet Ltd.">
// Copyright 2019 Automate The Planet Ltd.
// Licensed under the Royalty-free End-user License Agreement, Version 1.0 (the "License");
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://bellatrix.solutions/licensing-royalty-free/
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// <author>Anton Angelov</author>
// <site>https://bellatrix.solutions/</site>
using E2E.Load.Core.Model.Results;

namespace E2E.Load.Core.Model.Ensures
{
    public class EnsuredAltIsNullHandler : LoadTestEnsureHandler
    {
        public override string EnsureType => "EnsuredAltIsNull";

        public override ResponseAssertionResults Execute(LoadTestElement loadTestElement, string expectedValue)
        {
            var responseAssertionResults = new ResponseAssertionResults();
            if (loadTestElement.HtmlNode.GetAttributeValue("alt", string.Empty) != string.Empty)
            {
                responseAssertionResults.AssertionType = $"{EnsureType}- {loadTestElement.Locator}={loadTestElement.LocatorValue} Expected = {expectedValue}";
                responseAssertionResults.Passed = false;
                responseAssertionResults.FailedMessage = $"Element with locator {loadTestElement.Locator}={loadTestElement.LocatorValue} alt wasn't null.";
            }

            return responseAssertionResults;
        }
    }
}
