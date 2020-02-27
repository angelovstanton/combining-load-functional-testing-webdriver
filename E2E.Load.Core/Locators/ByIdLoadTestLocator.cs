﻿// <copyright file="ByIdLoadTestLocator.cs" company="Automate The Planet Ltd.">
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
using HtmlAgilityPack;

namespace E2E.Load.Core.Model.Locators
{
    public class ByIdLoadTestLocator : LoadTestLocator
    {
        public override string LocatorType => "Bellatrix.Web.ById";

        public override LoadTestElement LocateElement(HtmlDocument htmlDoc, string locatorValue)
        {
            var htmlNode = htmlDoc.GetElementbyId(locatorValue);
            ThrowNewNotFoundElementException(htmlNode, locatorValue);

            return new LoadTestElement(htmlNode, LocatorType, locatorValue);
        }
    }
}