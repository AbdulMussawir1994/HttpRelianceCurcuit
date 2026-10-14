namespace OrderServiceAPI.Entity.DTOs;

public record ShippingQuoteDto(int ProductId, decimal ShippingCost, int EstimatedDeliveryDays);
