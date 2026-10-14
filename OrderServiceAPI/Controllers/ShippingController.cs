using Microsoft.AspNetCore.Mvc;
using OrderServiceAPI.Repository.Interface;

namespace OrderServiceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShippingController(IShippingService shippingService) : ControllerBase
{
    [HttpGet("GetQuote/{productId:int}")]
    public async Task<IActionResult> GetQuote(int productId, CancellationToken token)
    {
        if (productId <= 0)
            return BadRequest("Product ID must be greater than zero.");

        var quote = await shippingService.GetQuoteAsync(productId, token);
        return quote is null ? NotFound("No shipping quote available.") : Ok(quote);
    }
}
