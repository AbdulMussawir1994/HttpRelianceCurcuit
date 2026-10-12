using OrderServiceAPI.Entity.DTOs;

namespace OrderServiceAPI.Repository.Interface;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(int productId, CancellationToken token);
}
