using Microsoft.AspNetCore.Mvc;
using OrderServiceAPI.Repository.Interface;

namespace OrderServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("OrderApi/{id:int}")]
        public async Task<IActionResult> GetOrder(int id, CancellationToken token)
        {
            if (id <= 0)
                return BadRequest("Invalid Order ID.");

            var response = await _orderService.GetQuoteAsync(id, token);

            if (response is null)
                return NotFound("No data available.");

            return Ok(response);
        }
    }
}
