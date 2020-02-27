﻿// <copyright file="LoadTestAssertions.cs" company="Automate The Planet Ltd.">
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
using System.Collections.Generic;
using E2E.Load.Core.Model.Assertions;
using E2E.Load.Core.Model.Ensures;
using E2E.Load.Core.Model.Locators;

namespace E2E.Load.Core.Model
{
    public class LoadTestAssertions
    {
        private readonly List<LoadTestAssertionHandler> _loadTestAssertionHandlers;
        private readonly List<LoadTestLocator> _loadTestLocators;
        private readonly List<LoadTestEnsureHandler> _loadTestEnsureHandler;

        public LoadTestAssertions(List<LoadTestAssertionHandler> loadTestAssertionHandlers, List<LoadTestLocator> loadTestLocators, List<LoadTestEnsureHandler> loadTestEnsureHandler)
        {
            _loadTestAssertionHandlers = loadTestAssertionHandlers;
            _loadTestLocators = loadTestLocators;
            _loadTestEnsureHandler = loadTestEnsureHandler;
        }

        public void AssertAllRequestStatusesAreSuccessful()
        {
            _loadTestAssertionHandlers.Add(new SuccessStatusLoadTestAssertionHandler());
        }

        public void AssertAllRecordedEnsureAssertions()
        {
            _loadTestAssertionHandlers.Add(new EnsuresLoadTestAssertionHandler(_loadTestLocators, _loadTestEnsureHandler));
        }
    }
}