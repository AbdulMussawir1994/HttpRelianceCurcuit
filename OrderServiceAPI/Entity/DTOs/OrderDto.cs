namespace OrderServiceAPI.Entity.DTOs;

public record OrderDto(int ProductId, string ProductName, decimal UnitPrice, decimal Tax, decimal Total);
