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