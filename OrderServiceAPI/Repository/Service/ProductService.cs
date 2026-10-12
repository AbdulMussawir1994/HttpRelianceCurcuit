using OrderServiceAPI.Entity.DTOs;
using OrderServiceAPI.Repository.Interface;
using System.Net;

namespace OrderServiceAPI.Repository.Service;


public class ProductService(HttpClient httpClient) : IProductService
{
    public async Task<ProductDto?> GetByIdAsync(int productId, CancellationToken token)
    {
        using var response = await httpClient.GetAsync($"api/Products/GetById/{productId}", HttpCompletionOption.ResponseHeadersRead, token);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProductDto>(token);
    }
}