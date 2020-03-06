﻿// <copyright file="ByClassLoadTestLocator.cs" company="Automate The Planet Ltd.">
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;

namespace E2E.Load.Core.Model.Locators
{
    public class ByClassLoadTestLocator : LoadTestLocator
    {
        public override string LocatorType => "By.Class";

        public override LoadTestElement LocateElement(HtmlDocument htmlDoc, string locatorValue)
        {
            var htmlNodes = default(IEnumerable<HtmlNode>);
            htmlNodes = htmlDoc.DocumentNode.SelectNodes($"//*[@class='{locatorValue}']")?.Nodes();
            ThrowNewNotFoundElementException(htmlNodes, locatorValue);

            return new LoadTestElement(htmlNodes.FirstOrDefault(), LocatorType, locatorValue);
        }
    }
}