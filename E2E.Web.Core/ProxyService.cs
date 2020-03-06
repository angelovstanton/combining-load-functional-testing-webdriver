// <copyright file="ProxyService.cs" company="Automate The Planet Ltd.">
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
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using E2E.Core;
using E2E.Load.Core.Configuration;
using E2E.Load.Core.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Http;
using Titanium.Web.Proxy.Models;

namespace E2E.Load.Core.Services
{
    public class ProxyService : IDisposable
    {
        private readonly ConcurrentDictionary<string, string> _redirectUrls;
        private readonly ConcurrentBag<string> _blockUrls;

        public ProxyService()
        {
            IsEnabled = ConfigurationService.Instance.GetWebProxySettings().IsEnabled;
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                IsEnabled = false;
            }

            if (IsEnabled)
            {
                ProxyServer = new ProxyServer(false);
            }

            _redirectUrls = new ConcurrentDictionary<string, string>();
            _blockUrls = new ConcurrentBag<string>();
            RequestsHistory = new ConcurrentDictionary<int, MeasuredRequest>();
            ResponsesHistory = new ConcurrentDictionary<int, Response>();
        }

        public ProxyServer ProxyServer { get; set; }

        public ConcurrentDictionary<int, MeasuredRequest> RequestsHistory { get; set; }

        public ConcurrentDictionary<int, Response> ResponsesHistory { get; set; }

        public bool IsEnabled { get; set; }

        public int Port { get; set; }

        public void Dispose() => ProxyServer?.Stop();

        public void Start()
        {
            if (IsEnabled)
            {
                Port = GetFreeTcpPort();
                var explicitEndPoint = new ExplicitProxyEndPoint(IPAddress.Any, Port);

                ProxyServer.AddEndPoint(explicitEndPoint);

                ProxyServer.Start();

                ProxyServer.BeforeRequest += OnRequestCaptureTrafficEventHandler;
                ProxyServer.BeforeResponse += OnResponseCaptureTrafficEventHandler;
                ProxyServer.BeforeRequest += OnRequestBlockResourceEventHandler;
                ProxyServer.BeforeRequest += OnRequestRedirectTrafficEventHandler;
                ProxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
                ProxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;
            }
        }

        public void SetUrlToBeRedirectedTo(string originalUrl, string redirectUrl)
        {
            ShouldExecute();
            if (string.IsNullOrEmpty(originalUrl))
            {
                throw new ArgumentException("The original URL cannot be null or empty. Please specify a valid URL.");
            }

            if (string.IsNullOrEmpty(redirectUrl))
            {
                throw new ArgumentException("The redirect URL cannot be null or empty. Please specify a valid URL.");
            }

            _redirectUrls.GetOrAdd(originalUrl, redirectUrl);
        }

        public void ClearAllRedirectUrlPairs()
        {
            ShouldExecute();
            _redirectUrls.Clear();
        }

        public void SetUrlToBeBlocked(string url)
        {
            ShouldExecute();
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("The URL cannot be null or empty. Please specify a valid URL.");
            }

            _blockUrls.Add(url);
        }

        public void ClearAllBlockUrls()
        {
            ShouldExecute();
            while (_blockUrls.TryTake(out _) && !_blockUrls.IsEmpty)
            {
            }
        }

        public void AssertNoErrorCodes()
        {
            ShouldExecute();
            bool areThereErrorCodes = ResponsesHistory.Values.Any(r => r.StatusCode > 400 && r.StatusCode < 599);
            Assert.IsFalse(areThereErrorCodes);
        }

        public void AssertRequestMade(string url)
        {
            ShouldExecute();
            bool areRequestsMade =
                RequestsHistory.Values.ToList().Any(r => r.Request.RequestUri.ToString().Contains(url));
            Assert.IsTrue(areRequestsMade);
        }

        public void AssertRequestNotMade(string url)
        {
            ShouldExecute();
            bool areRequestsMade =
                RequestsHistory.Values.ToList().Any(r => r.Request.RequestUri.ToString().Contains(url));
            Assert.IsFalse(areRequestsMade);
        }

        public void AssertNoLargeImagesRequested(int contentLength = 40000)
        {
            ShouldExecute();
            bool areThereLargeImages = RequestsHistory.Values.Any(r =>
                r.Request.ContentType != null && r.Request.ContentType.StartsWith("image", StringComparison.Ordinal) &&
                r.Request.ContentLength < contentLength);
            Assert.IsFalse(areThereLargeImages);
        }

        private int GetFreeTcpPort()
        {
            Thread.Sleep(100);
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint) l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        private async Task OnRequestCaptureTrafficEventHandler(object sender, SessionEventArgs e) => await Task.Run(
            () =>
            {
                if (!RequestsHistory.ContainsKey(e.HttpClient.Request.GetHashCode()) && e.HttpClient != null &&
                    e.HttpClient.Request != null)
                {
                    var measuredRequest = new MeasuredRequest(DateTime.Now, e.HttpClient.Request);
                    RequestsHistory.GetOrAdd(e.HttpClient.Request.GetHashCode(), measuredRequest);
                }
            });

        private async Task OnRequestBlockResourceEventHandler(object sender, SessionEventArgs e)
            => await Task.Run(
                () =>
                {
                    if (_blockUrls.Count > 0)
                    {
                        foreach (var urlToBeBlocked in _blockUrls)
                        {
                            if (e.HttpClient.Request.RequestUri.ToString().Contains(urlToBeBlocked))
                            {
                                string customBody = string.Empty;
                                e.Ok(Encoding.UTF8.GetBytes(customBody));
                            }
                        }
                    }
                });

        private async Task OnRequestRedirectTrafficEventHandler(object sender, SessionEventArgs e)
        {
            if (_redirectUrls.Keys.Count > 0)
            {
                foreach (var redirectUrlPair in _redirectUrls)
                {
                    if (e.HttpClient.Request.RequestUri.AbsoluteUri.Contains(redirectUrlPair.Key))
                    {
                        e.Redirect(redirectUrlPair.Value);
                    }
                }
            }
        }

        private async Task OnResponseCaptureTrafficEventHandler(object sender, SessionEventArgs e) => await Task.Run(
            () =>
            {
                if (!ResponsesHistory.ContainsKey(e.HttpClient.Response.GetHashCode()) &&
                    e.HttpClient?.Response != null)
                {
                    ResponsesHistory.GetOrAdd(e.HttpClient.Response.GetHashCode(), e.HttpClient.Response);
                }
            });

        private void ShouldExecute()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new NotSupportedException(
                    "Bellatrix Web capture HTTP traffic feature is available only on Windows.");
            }

            if (!IsEnabled)
            {
                throw new ArgumentException(
                    "ProxyService is not enabled. To use open testFramework.json and set isEnabled = true of webProxySettings");
            }
        }

        private Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        {
            if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                e.IsValid = true;
            }

            return Task.FromResult(0);
        }

        private Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        {
            return Task.FromResult(0);
        }
    }
}