using Polly;
using Polly.Timeout;
using System.Net;

namespace OrderServiceAPI.Extensions;

public static class HttpTransientErrorDetector
{
    public static bool IsTransient(Outcome<HttpResponseMessage> outcome)
    {
        if (outcome.Exception is HttpRequestException or TimeoutRejectedException)
        {
            return true;
        }

        if (outcome.Result is not { } response)
        {
            return false;
        }

        return response.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests || (int)response.StatusCode >= 500;
    }
}
