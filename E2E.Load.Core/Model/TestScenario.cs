﻿// <copyright file="TestScenario.cs" company="Automate The Planet Ltd.">
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

using System.Collections.Concurrent;

namespace E2E.Load.Core.Model
{
    public class TestScenario
    {
        public TestScenario(string testName, ConcurrentBag<HttpRequestDto> requests)
        {
            TestName = testName;
            Requests = requests;
        }

        public string TestName { get; set; }
        public ConcurrentBag<HttpRequestDto> Requests { get; set; }
        public int Weight { get; set; } = 1;
        public int TimesToBeExecuted { get; set; }
        public int TimesExecuted { get; set; }
        public double Factor => (TimesToBeExecuted + 1) * Weight / TimesExecuted + 1;
    }
}