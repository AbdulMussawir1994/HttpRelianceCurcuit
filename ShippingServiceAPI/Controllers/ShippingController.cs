using Microsoft.AspNetCore.Mvc;

namespace ShippingServiceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShippingController : ControllerBase
{
    [HttpGet("GetQuote/{productId:int}")]
    public IActionResult GetQuote(int productId)
    {
        if (productId <= 0)
            return BadRequest("Product ID must be greater than zero.");

        if (productId > 5)
            return NotFound("No shipping quote available.");

        return Ok(new
        {
            ProductId = productId,
            ShippingCost = 12.50m,
            EstimatedDeliveryDays = 3
        });
    }
}
