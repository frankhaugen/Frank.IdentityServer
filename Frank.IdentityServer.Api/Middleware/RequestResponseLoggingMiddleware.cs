using System.Text;
using Microsoft.AspNetCore.Http.Extensions; // For GetDisplayUrl
using Microsoft.Extensions.Logging; // Required for ILogger
using Microsoft.AspNetCore.Http; // Required for HttpContext, RequestDelegate, IHeaderDictionary
using System.IO; // Required for MemoryStream, StreamReader
using System.Threading.Tasks; // Required for Task
using Microsoft.AspNetCore.Builder; // Required for IApplicationBuilder

namespace Frank.IdentityServer.Api.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log Request
            context.Request.EnableBuffering(); // Ensure the request body can be read multiple times

            var requestHeaders = FormatHeaders(context.Request.Headers);
            string requestBodyText = string.Empty;
            if (context.Request.Body.CanRead && context.Request.ContentLength > 0)
            {
                context.Request.Body.Position = 0;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true)) // leaveOpen = true
                {
                    requestBodyText = await reader.ReadToEndAsync();
                }
                context.Request.Body.Position = 0; // Reset stream position for next middleware
            }

            _logger.LogInformation($"Incoming Request:\n" +
                                 $"Method: {{RequestMethod}}\n" +
                                 $"Url: {{RequestUrl}}\n" +
                                 $"Headers: \n{{RequestHeaders}}\n" +
                                 $"Body: {{RequestBody}}",
                                 context.Request.Method,
                                 context.Request.GetDisplayUrl(),
                                 requestHeaders,
                                 requestBodyText);

            // Capture Response
            var originalResponseBodyStream = context.Response.Body;
            using var responseBodyMemoryStream = new MemoryStream();
            context.Response.Body = responseBodyMemoryStream;

            await _next(context);

            // Log Response
            var responseHeaders = FormatHeaders(context.Response.Headers);
            string responseBodyText = string.Empty;
            if (responseBodyMemoryStream.CanRead && responseBodyMemoryStream.Length > 0)
            {
                responseBodyMemoryStream.Position = 0;
                using (var reader = new StreamReader(responseBodyMemoryStream, Encoding.UTF8, true, 1024, true)) // leaveOpen = true
                {
                    responseBodyText = await reader.ReadToEndAsync();
                }
                responseBodyMemoryStream.Position = 0; // Reset stream position for copying
            }
            
            _logger.LogInformation($"Outgoing Response:\n" +
                                 $"StatusCode: {{ResponseStatusCode}}\n" +
                                 $"Headers: \n{{ResponseHeaders}}\n" +
                                 $"Body: {{ResponseBody}}",
                                 context.Response.StatusCode,
                                 responseHeaders,
                                 responseBodyText);

            await responseBodyMemoryStream.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream; // Restore original stream
        }

        private static string FormatHeaders(IHeaderDictionary headers)
        {
            var builder = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                builder.AppendLine($"  {key}: {value}");
            }
            return builder.ToString().TrimEnd();
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
