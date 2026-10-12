using OrderServiceAPI.ConfigurationSettings;
using OrderServiceAPI.Repository.Interface;
using OrderServiceAPI.Repository.Service;

namespace OrderServiceAPI.Extensions;

public static class ServicesDependencyInjection
{
    public static IServiceCollection AddOrderApiServices(this IServiceCollection services, IConfiguration configuration)
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
            .ValidateOnStart();

        services.AddScoped<IOrderService, OrderService>();

        services.AddHttpClient();

        return services;
    }
}
