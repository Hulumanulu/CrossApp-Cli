namespace Core.Services;

using Core.Abstractions;
using Core.Domain;
using Core.Events;

public sealed class OrderService(IOrderStore store)
{
    private readonly IOrderStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public event EventHandler<OrderChangedEventArgs>? Changed;

    private void OnChanged(Order o, OrderChangeKind kind) =>
        Changed?.Invoke(this, new OrderChangedEventArgs(o.Id, kind, o.Lines.Count, o.TotalPrice));

    public Order CreateOrder(string id, string customerId)
    {
        var order = Order.Create(id, customerId);
        _store.Add(order);
        OnChanged(order, OrderChangeKind.Created);  
        return order;
    }

    public void AddLine(string orderId, string productId, string name, decimal price, int quantity)
    {
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Немає запису з id={orderId}.");

        order.AddLine(productId, name, price, quantity);  
        _store.Update(order);
        OnChanged(order, OrderChangeKind.LineAdded);
    }

    public void Confirm(string orderId)
    {
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Немає запису з id={orderId}.");

        order.Confirm();
        _store.Update(order);
        OnChanged(order, OrderChangeKind.Confirmed);
    }

    public IReadOnlyList<Order> All() => _store.List();
    public Order? Find(string id) => _store.GetById(id);

    public IReadOnlyList<Order> FindAll(Func<Order, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}