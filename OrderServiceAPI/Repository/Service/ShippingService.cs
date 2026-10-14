using Microsoft.Extensions.Options;
using OrderServiceAPI.ConfigurationSettings;
using OrderServiceAPI.Entity.DTOs;
using OrderServiceAPI.Repository.Interface;
using System.Globalization;
using System.Net;

namespace OrderServiceAPI.Repository.Service;

public class ShippingService(HttpClient httpClient, IOptions<ShippingOptions> options) : IShippingService
{
    public async Task<ShippingQuoteDto?> GetQuoteAsync(int productId, CancellationToken token)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(productId);

        var path = options.Value.QuotePathTemplate.Replace(
            "{productId}", productId.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);

        using var response = await httpClient.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, token);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ShippingQuoteDto>(token);
    }
}
