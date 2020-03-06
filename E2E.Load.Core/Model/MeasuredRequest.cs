using System;
using System.Collections.Generic;
using System.Text;
using Titanium.Web.Proxy.Http;

namespace E2E.Load.Core.Model
{
    public class MeasuredRequest
    {
        public MeasuredRequest(DateTime creationTime, Request request)
        {
            CreationTime = creationTime;
            Request = request;
        }

        public DateTime CreationTime { get; set; }
        public Request Request { get; set; }
    }
}