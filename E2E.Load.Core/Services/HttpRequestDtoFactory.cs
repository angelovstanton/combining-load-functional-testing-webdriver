using System;
using Titanium.Web.Proxy.Http;

namespace E2E.Load.Core
{
    public class HttpRequestDtoFactory
    {
        public HttpRequestDto Create(Request request, HttpRequestDto previousHttpRequestDto)
        {
            var httpRequestDto = new HttpRequestDto
            {
                Url = request.Url,
                Method = request.Method,
                ContentType = request.ContentType,
                CreationTime = DateTime.Now,
            };
            if (previousHttpRequestDto != null)
            {
                httpRequestDto.MillisecondsPauseAfterPreviousRequest =
                    (httpRequestDto.CreationTime - previousHttpRequestDto.CreationTime).Milliseconds;
            }

            foreach (var item in request.Headers)
            {
                httpRequestDto.Headers.Add(item.ToString());
            }

            return httpRequestDto;
        }
    }
}