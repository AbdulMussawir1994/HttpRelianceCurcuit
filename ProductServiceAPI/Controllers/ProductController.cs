using Microsoft.AspNetCore.Mvc;
using ProductServiceAPI.Models;

namespace ProductServiceAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController() : ControllerBase
{
    private static readonly Product[] Products =
     [
         new() { Id = 1, Name = "Laptop", Price = 1200.00m },
        new() { Id = 2, Name = "Mouse", Price = 50.00m },
        new() { Id = 3, Name = "Keyboard", Price = 150.00m },
        new() { Id = 4, Name = "Monitor", Price = 500.00m },
        new() { Id = 5, Name = "Headphones", Price = 250.00m }
     ];

    [HttpGet("GetById/{id:int}")]
    public ActionResult<Product> GetByIdArray(int id, CancellationToken token)
    {
        if (id <= 0)
            return BadRequest("Product ID must be greater than zero.");

        var product = Array.Find(Products, p => p.Id == id);

        if (product is null)
            return NotFound($"Product with ID {id} not found.");

        return Ok(product);
    }
}
