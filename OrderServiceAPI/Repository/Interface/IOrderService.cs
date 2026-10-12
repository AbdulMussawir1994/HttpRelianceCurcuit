using OrderServiceAPI.Entity.DTOs;

namespace OrderServiceAPI.Repository.Interface;

public interface IOrderService
{
    Task<OrderDto?> GetQuoteAsync(int productId, CancellationToken cancellationToken);
}
