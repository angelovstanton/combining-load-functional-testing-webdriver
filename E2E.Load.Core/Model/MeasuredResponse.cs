using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace E2E.Load.Core
{
    public class MeasuredResponse : IMeasuredResponse
    {
        private readonly IRestResponse _restResponse;

        public MeasuredResponse(IRestResponse restResponse, TimeSpan executionTime)
        {
            _restResponse = restResponse;
            ExecutionTime = executionTime;
        }

        public TimeSpan ExecutionTime { get; set; }
        public IList<RestResponseCookie> Cookies => _restResponse.Cookies;
        public bool IsSuccessful => _restResponse.IsSuccessful;
        public IList<Parameter> Headers => _restResponse.Headers;

        public IRestRequest Request
        {
            get => _restResponse.Request;
            set => _restResponse.Request = value;
        }

        public string ContentType
        {
            get => _restResponse.ContentType;
            set => _restResponse.ContentType = value;
        }

        public long ContentLength
        {
            get => _restResponse.ContentLength;
            set => _restResponse.ContentLength = value;
        }

        public string ContentEncoding
        {
            get => _restResponse.ContentEncoding;
            set => _restResponse.ContentEncoding = value;
        }

        public string Content
        {
            get => _restResponse.Content;
            set => _restResponse.Content = value;
        }

        public HttpStatusCode StatusCode
        {
            get => _restResponse.StatusCode;
            set => _restResponse.StatusCode = value;
        }

        public string StatusDescription
        {
            get => _restResponse.StatusDescription;
            set => _restResponse.StatusDescription = value;
        }

        public byte[] RawBytes
        {
            get => _restResponse.RawBytes;
            set => _restResponse.RawBytes = value;
        }

        public Uri ResponseUri
        {
            get => _restResponse.ResponseUri;
            set => _restResponse.ResponseUri = value;
        }

        public string Server
        {
            get => _restResponse.Server;
            set => _restResponse.Server = value;
        }

        public ResponseStatus ResponseStatus
        {
            get => _restResponse.ResponseStatus;
            set => _restResponse.ResponseStatus = value;
        }

        public string ErrorMessage
        {
            get => _restResponse.ErrorMessage;
            set => _restResponse.ErrorMessage = value;
        }

        public Exception ErrorException
        {
            get => _restResponse.ErrorException;
            set => _restResponse.ErrorException = value;
        }

        public Version ProtocolVersion
        {
            get => _restResponse.ProtocolVersion;
            set => _restResponse.ProtocolVersion = value;
        }
    }
}