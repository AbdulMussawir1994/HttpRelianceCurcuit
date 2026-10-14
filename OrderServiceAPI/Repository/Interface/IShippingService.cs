using OrderServiceAPI.Entity.DTOs;

namespace OrderServiceAPI.Repository.Interface;

public interface IShippingService
{
    Task<ShippingQuoteDto?> GetQuoteAsync(int productId, CancellationToken token);
}
