using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using E2E.Core;
using E2E.Load.Core.Configuration;
using Titanium.Web.Proxy.EventArguments;

namespace E2E.Load.Core
{
    public class LoadTestingWorkflowPluginContext
    {
        public static readonly ConcurrentDictionary<string, ConcurrentBag<HttpRequestDto>> HttpRequestsPerTest;
        public static bool IsLoadTestingEnabled;
        public static string CurrentTestName;
        public static bool ShouldFilterByHost;
        public static string FilterHost;
        private static readonly bool _isProxyEnabled;
        public static HttpRequestDto _previousHttpRequestDto;
        private static string _requestsFilePath;

        private readonly List<string> _ignoreUrlRequestsPatterns;

        ////private readonly ProxyService _proxyService;
        private readonly HttpRequestDtoFactory _httpRequestDtoFactory;
        private readonly JsonSerializer _jsonSerializer;

        static LoadTestingWorkflowPluginContext()
        {
            HttpRequestsPerTest = new ConcurrentDictionary<string, ConcurrentBag<HttpRequestDto>>();
            _isProxyEnabled = ConfigurationService.Instance.GetWebProxySettings().IsEnabled;
        }

        public LoadTestingWorkflowPluginContext()
        {
            IsLoadTestingEnabled = ConfigurationService.Instance.GetLoadTestingSettings().IsEnabled;

            _ignoreUrlRequestsPatterns =
                ConfigurationService.Instance.GetLoadTestingSettings().IgnoreUrlRequestsPatterns;

            if (IsLoadTestingEnabled && !_isProxyEnabled)
            {
                throw new ArgumentException(
                    "To use the BELLATRIX Load Testing module, you need to enable the web proxy. To do so in the testFrameworkSettings.json file set webProxySettings.isEnabled = true and at the Browser attribute set shouldCaptureHttpTraffic to true. ");
            }
            else if (IsLoadTestingEnabled && _isProxyEnabled)
            {
                _httpRequestDtoFactory = new HttpRequestDtoFactory();
                _jsonSerializer = new JsonSerializer();
                ////_proxyService = ServiceContainer.Provider.Resolve<ProxyService>();
                _requestsFilePath =
                    NormalizeRequestFilePath(
                        ConfigurationService.Instance.GetLoadTestingSettings().RequestsFileLocation);
                ////_proxyService.ProxyServer.BeforeRequest += OnRequestCaptureTrafficEventHandler;
            }
        }

        public void PreTestInit()
        {
            IsLoadTestingEnabled = ConfigurationService.Instance.GetLoadTestingSettings().IsEnabled;

            if (IsLoadTestingEnabled && !_isProxyEnabled)
            {
                throw new ArgumentException(
                    "To use the BELLATRIX Load Testing module, you need to enable the web proxy. To do so in the testFrameworkSettings.json file set webProxySettings.isEnabled = true and at the Browser attribute set shouldCaptureHttpTraffic to true. ");
            }

            if (IsLoadTestingEnabled)
            {
                if (!HttpRequestsPerTest.ContainsKey(CurrentTestName))
                {
                    HttpRequestsPerTest.GetOrAdd(CurrentTestName, new ConcurrentBag<HttpRequestDto>());
                }
            }
        }

        public void PostTestCleanup()
        {
            if (IsLoadTestingEnabled)
            {
                try
                {
                    string requestFileDirectory = Path.GetDirectoryName(_requestsFilePath);
                    if (!Directory.Exists(requestFileDirectory))
                    {
                        Directory.CreateDirectory(requestFileDirectory);
                    }

                    ConcurrentBag<HttpRequestDto> httpRequestDto = HttpRequestsPerTest[CurrentTestName];

                    var jsonContent = _jsonSerializer.Serialize(httpRequestDto);
                    var testFilePath = Path.Combine(_requestsFilePath, $"{CurrentTestName}.json");
                    File.WriteAllText(testFilePath, jsonContent);
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new UnauthorizedAccessException("Cannot save load test requests.", ex);
                }
                catch (PathTooLongException ex)
                {
                    throw new PathTooLongException("Cannot save load test requests.", ex);
                }
                catch (IOException ex)
                {
                    throw new IOException("Cannot save load test requests.", ex);
                }
            }

            _previousHttpRequestDto = null;
            CurrentTestName = null;
        }

        private string NormalizeRequestFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }
            else if (path.StartsWith("AssemblyFolder", StringComparison.Ordinal))
            {
                var executionFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                path = path.Replace("AssemblyFolder", executionFolder);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    path = path.Replace('\\', '/');
                }
            }

            return path;
        }

        public async Task OnRequestCaptureTrafficEventHandler(object sender, SessionEventArgs e) => await Task.Run(
            () =>
            {
                if (IsLoadTestingEnabled && CurrentTestName != null)
                {
                    if (HttpRequestsPerTest.ContainsKey(CurrentTestName) && e.HttpClient != null &&
                        e.HttpClient.Request != null)
                    {
                        var htpRequestDto =
                            _httpRequestDtoFactory.Create(e.HttpClient.Request, _previousHttpRequestDto);
                        if (e.HttpClient.Request.Method == "POST" || e.HttpClient.Request.Method == "PUT" ||
                            e.HttpClient.Request.Method == "PATCH")
                        {
                            try
                            {
                                htpRequestDto.Body = e.GetRequestBodyAsString().Result;
                            }
                            catch
                            {
                                // Ignore if doesn't have body
                            }
                        }

                        if (((ShouldFilterByHost &&
                              htpRequestDto.Headers.Any(x => x.Contains($"Host: {FilterHost}"))) ||
                             !ShouldFilterByHost) && !ShouldFilterRequest(htpRequestDto.Url))
                        {
                            if (_previousHttpRequestDto == null || !_previousHttpRequestDto.Equals(htpRequestDto))
                            {
                                HttpRequestsPerTest[CurrentTestName].Add(htpRequestDto);
                                _previousHttpRequestDto = htpRequestDto;
                            }
                        }
                    }
                }
            });

        private bool ShouldFilterRequest(string url)
        {
            bool shouldFilter = false;
            foreach (var currentPattern in _ignoreUrlRequestsPatterns)
            {
                var m = Regex.Match(url, currentPattern, RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    shouldFilter = true;
                    break;
                }
            }

            return shouldFilter;
        }
    }
}