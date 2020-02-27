using System;
using RestSharp;

namespace E2E.Load.Core
{
    public interface IMeasuredResponse : IRestResponse
    {
        TimeSpan ExecutionTime { get; set; }
    }
}