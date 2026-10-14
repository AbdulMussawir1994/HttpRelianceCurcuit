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
    public static IServiceCollection AddThirdPartyApiClients(this IServiceCollection services)
    {
        services.AddProductApiClient();
        services.AddShippingApiClient();
        return services;
    }

    public static IServiceCollection AddProductApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<IProductService, ProductService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ProductOptions>>().Value;
            ConfigureClient(client, options.BaseUrl);
        })
        .AddResilienceHandler("product-api-resilience", (pipeline, context) =>
        {
            var serviceProvider = context.ServiceProvider;
            var settings = serviceProvider.GetRequiredService<IOptions<ResilienceSettings>>().Value;
            var options = serviceProvider.GetRequiredService<IOptions<ProductOptions>>().Value;
            ConfigurePipeline(pipeline, settings, options.TimeoutSeconds);
        });

        return services;
    }

    public static IServiceCollection AddShippingApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<IShippingService, ShippingService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<ShippingOptions>>().Value;
            ConfigureClient(client, options.BaseUrl);
        })
        .AddResilienceHandler("shipping-api-resilience", (pipeline, context) =>
        {
            var serviceProvider = context.ServiceProvider;
            var settings = serviceProvider.GetRequiredService<IOptions<ResilienceSettings>>().Value;
            var options = serviceProvider.GetRequiredService<IOptions<ShippingOptions>>().Value;
            ConfigurePipeline(pipeline, settings, options.TimeoutSeconds);
        });

        return services;
    }

    private static void ConfigureClient(HttpClient client, string baseUrl)
    {
        client.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private static void ConfigurePipeline(ResiliencePipelineBuilder<HttpResponseMessage> pipeline, ResilienceSettings settings,
        int totalTimeoutSeconds)
    {
        // Strategies execute from outermost to innermost. Each client has its own state.
        pipeline.AddTimeout(TimeSpan.FromSeconds(totalTimeoutSeconds));

        // A retry strategy requires at least one retry. Zero disables it entirely.
        if (settings.RetryCount > 0)
        {
            pipeline.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = settings.RetryCount,
                Delay = TimeSpan.FromMilliseconds(settings.RetryDelayMilliseconds),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                ShouldHandle = args => ValueTask.FromResult(HttpTransientErrorDetector.IsTransient(args.Outcome))
            });
        }

        pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = settings.CircuitFailureRatio,
            MinimumThroughput = settings.CircuitMinimumThroughput,
            SamplingDuration = TimeSpan.FromSeconds(settings.CircuitSamplingSeconds),
            BreakDuration = TimeSpan.FromSeconds(settings.CircuitBreakSeconds),
            ShouldHandle = args => ValueTask.FromResult(HttpTransientErrorDetector.IsTransient(args.Outcome))
        });
        pipeline.AddTimeout(TimeSpan.FromSeconds(settings.AttemptTimeoutSeconds));
    }
}
