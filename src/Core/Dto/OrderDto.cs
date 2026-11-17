namespace Core.Dto;

public record OrderLineDto(string ProductId, string Name, decimal Price, int Quantity);

public record OrderDto(
    string Id,
    string Name,
    decimal Price,
    string? Note = null,
    bool IsConfirmed = false,
    IReadOnlyList<OrderLineDto>? Lines = null);