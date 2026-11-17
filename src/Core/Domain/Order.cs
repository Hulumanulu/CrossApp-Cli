using Core.Dto;

namespace Core.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public string Id { get; }
    public string CustomerId { get; }
    public bool IsConfirmed { get; private set; }

    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    public decimal TotalPrice => _lines.Sum(l => l.LineTotal);

    private Order(string id, string customerId, bool isConfirmed = false)
    {
        Id = id;
        CustomerId = customerId;
        IsConfirmed = isConfirmed;
    }

    public static Order Create(string id, string customerId)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Номер замовлення не може бути порожнім", nameof(id));

        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Ідентифікатор клієнта не може бути порожнім", nameof(customerId));

        return new Order(id.Trim().ToUpperInvariant(), customerId.Trim());
    }

    public void AddLine(string productId, string name, decimal price, int quantity)
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Замовлення {Id} вже підтверджене, додавання нових рядків заборонено");

        OrderLine line = OrderLine.Create(productId, name, price, quantity);
        _lines.Add(line);
    }

    public void Confirm()
    {
        if (IsConfirmed)
            throw new InvalidOperationException($"Замовлення {Id} вже підтверджене");

        if (_lines.Count == 0)
            throw new InvalidOperationException($"Неможливо підтвердити порожнє замовлення {Id}");

        IsConfirmed = true;
    }

    public OrderDto ToDto() => new(
        Id, CustomerId, TotalPrice,
        IsConfirmed: IsConfirmed,
        Lines: _lines.Select(l => new OrderLineDto(l.ProductId, l.Name, l.Price, l.Quantity)).ToList());

    public static Order FromDto(OrderDto dto)
    {
        Order order = Create(dto.Id, dto.Name);

        if (dto.Lines is { Count: > 0 })
        {
            foreach (var l in dto.Lines)
                order.AddLine(l.ProductId, l.Name, l.Price, l.Quantity);
        }
        else if (dto.Price > 0)
        {
            order.AddLine("P-AUTO", "Позиція за замовчуванням", dto.Price, 1);
        }

        if (dto.IsConfirmed) order.Confirm();
        return order;
    }

    public override string ToString() =>
        $"Замовлення [{Id}] Клієнт: {CustomerId} | Позицій: {_lines.Count} | Сума: {TotalPrice:F2} грн | Статус: {(IsConfirmed ? "Підтверджено" : "Чернетка")}";
}