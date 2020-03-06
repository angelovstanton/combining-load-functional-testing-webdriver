using System;
using System.Collections.Generic;
using System.Text;

namespace E2E.Load.Core.Configuration
{
    public class WebProxySettings
    {
        public bool IsEnabled { get; set; }
        public string PathToSslCertificate { get; set; }
    }
}