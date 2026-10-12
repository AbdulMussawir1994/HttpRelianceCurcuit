using OrderServiceAPI.Entity.DTOs;
using OrderServiceAPI.Repository.Interface;

namespace OrderServiceAPI.Repository.Service;

public class OrderService(IProductService products) : IOrderService
{
    public async Task<OrderDto?> GetQuoteAsync(int productId, CancellationToken token)
    {
        var product = await products.GetByIdAsync(productId, token);

        if (product is null)
            return null;

        var tax = decimal.Round(product.Price * 0.06m, 2);

        return new OrderDto(product.Id, product.Name, product.Price, tax, product.Price + tax);
    }
}
