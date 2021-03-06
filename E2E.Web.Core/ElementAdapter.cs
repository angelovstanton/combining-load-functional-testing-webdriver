﻿// <copyright file="ElementAdapter.cs" company="Automate The Planet Ltd.">
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
using E2E.Load.Core.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace E2E.Web.Core
{
    public class ElementAdapter : IElement
    {
        private readonly IWebDriver _webDriver;
        private readonly IWebElement _webElement;

        public ElementAdapter(IWebDriver webDriver, IWebElement webElement, By by)
        {
            _webDriver = webDriver;
            _webElement = webElement;
            By = by;
            Console.WriteLine(By.ToString());
        }

        public By By { get; }

        public string Text => _webElement?.Text;

        public bool? Enabled => _webElement?.Enabled;

        public bool? Displayed => _webElement?.Displayed;

        public void Click()
        {
            WaitToBeClickable(By);
            _webElement?.Click();
        }

        public IElement FindElement(By locator)
        {
            return new ElementAdapter(_webDriver, _webElement?.FindElement(locator), locator);
        }

        public void TypeText(string text)
        {
            _webElement?.Clear();
            _webElement?.SendKeys(text);
            RequestModificationSetter.SetRequestModification(text);
        }

        public void EnsuredTextIs(string value)
        {
            var webDriverWait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(30));
            webDriverWait.Until(
                SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(_webElement, value));
            ResponseAssertionSetter.AddResponseAssertionToHttpRequest(GetLocatorInfo().Item1, GetLocatorInfo().Item2,
                value);
        }

        private (string, string) GetLocatorInfo()
        {
            string[] locatorParts = By.ToString().Split(':');
            return (locatorParts[0], locatorParts[1].TrimStart());
        }

        private void WaitToBeClickable(By by)
        {
            var webDriverWait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(30));
            webDriverWait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(by));
        }
    }
}