namespace Core.Services;

using Core.Abstractions;
using Core.Domain;
using System;
using System.Collections.Generic;

public sealed class OrderService(IOrderStore store)
{
    // Вираз ?? throw для захисту від порожньої залежності
    private readonly IOrderStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public Order CreateOrder(string id, string customerId)
    {
        var order = Order.Create(id, customerId); // інваріанти перевіряє сутність
        _store.Add(order);
        return order;
    }

    public void AddLine(string orderId, string productId, string name, decimal price, int quantity)
    {
        // Вираз ?? throw, якщо запису немає
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Немає запису з id={orderId}.");

        order.AddLine(productId, name, price, quantity); // метод сутності
        _store.Update(order);
    }

    public void Confirm(string orderId)
    {
        var order = _store.GetById(orderId)
            ?? throw new InvalidOperationException($"Немає запису з id={orderId}.");

        order.Confirm();
        _store.Update(order);
    }

    public IReadOnlyList<Order> All() => _store.List();
    public Order? Find(string id) => _store.GetById(id);
}