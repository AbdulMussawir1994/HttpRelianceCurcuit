using OrderServiceAPI.ConfigurationSettings;
using OrderServiceAPI.Repository.Interface;
using OrderServiceAPI.Repository.Service;

namespace OrderServiceAPI.Extensions;

public static class ServicesDependencyInjection
{
    public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<ProductOptions>()
            .Bind(configuration.GetSection(ProductOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddOptions<ResilienceSettings>()
            .Bind(configuration.GetSection(ResilienceSettings.SectionName))
            .ValidateDataAnnotations()
            .Validate(settings => settings.CircuitSamplingSeconds >= 2 * settings.AttemptTimeoutSeconds,
                "CircuitSamplingSeconds must be at least twice AttemptTimeoutSeconds.")
            .ValidateOnStart();

        services
            .AddOptions<ShippingOptions>()
            .Bind(configuration.GetSection(ShippingOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var uri)
                && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps),
                "ShippingApi:BaseUrl must be an absolute HTTP or HTTPS URL.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.QuotePathTemplate)
                && options.QuotePathTemplate.Contains("{productId}", StringComparison.Ordinal)
                && !options.QuotePathTemplate.StartsWith('/')
                && !Uri.TryCreate(options.QuotePathTemplate, UriKind.Absolute, out _),
                "ShippingApi:QuotePathTemplate must be a relative path containing {productId}.")
            .ValidateOnStart();

        services.AddScoped<IOrderService, OrderService>();

        services.AddThirdPartyApiClients();

        return services;
    }
}
