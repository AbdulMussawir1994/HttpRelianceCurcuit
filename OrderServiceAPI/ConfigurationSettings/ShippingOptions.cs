using System.ComponentModel.DataAnnotations;

namespace OrderServiceAPI.ConfigurationSettings;

public class ShippingOptions
{
    public const string SectionName = "ShippingApi";

    [Required]
    [Url]
    public required string BaseUrl { get; init; }

    [Range(1, 300)]
    public int TimeoutSeconds { get; init; } = 20;

    [Required]
    public string QuotePathTemplate { get; init; } = "api/Shipping/GetQuote/{productId}";
}
