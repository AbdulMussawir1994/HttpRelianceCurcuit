using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using OrderServiceAPI.ConfigurationSettings;
using OrderServiceAPI.Repository.Interface;
using OrderServiceAPI.Repository.Service;
using Polly;
using System.Net.Http.Headers;

namespace OrderServiceAPI.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient<IProductService, ProductService>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<ProductOptions>>().Value;
                    client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })

            .AddResilienceHandler("product-api-resilience", (pipeline, context) =>
                {
                    var services = context.ServiceProvider;
                    var settings = services.GetRequiredService<IOptions<ResilienceSettings>>().Value;
                    var apiOptions = services.GetRequiredService<IOptions<ProductOptions>>().Value;

                    pipeline.AddTimeout(TimeSpan.FromSeconds(apiOptions.TimeoutSeconds));
                    pipeline.AddRetry(new HttpRetryStrategyOptions
                    {
                        MaxRetryAttempts = settings.RetryCount,
                        Delay = TimeSpan.FromMilliseconds(settings.RetryDelayMilliseconds),
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                        ShouldHandle = args => ValueTask.FromResult(HttpTransientErrorDetector.IsTransient(args.Outcome))
                    });
                    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                    {
                        FailureRatio = settings.CircuitFailureRatio,
                        MinimumThroughput = settings.CircuitMinimumThroughput,
                        SamplingDuration = TimeSpan.FromSeconds(settings.CircuitSamplingSeconds),
                        BreakDuration = TimeSpan.FromSeconds(settings.CircuitBreakSeconds),
                        ShouldHandle = args => ValueTask.FromResult(HttpTransientErrorDetector.IsTransient(args.Outcome))
                    });
                    pipeline.AddTimeout(TimeSpan.FromSeconds(settings.AttemptTimeoutSeconds));
                });

        return services;
    }
}
