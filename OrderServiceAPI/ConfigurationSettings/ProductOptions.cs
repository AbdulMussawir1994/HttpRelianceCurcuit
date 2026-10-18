using System.ComponentModel.DataAnnotations;

namespace OrderServiceAPI.ConfigurationSettings;

public class ProductOptions
{
    public const string SectionName = "ProductApi";

    [Required]
    [Url]
    public required string BaseUrl { get; init; }

    [Range(1, 300)]
    public int TimeoutSeconds { get; init; } = 0;
}
