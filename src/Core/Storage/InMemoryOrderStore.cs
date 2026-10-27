namespace Core.Storage;

using Core.Abstractions;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class InMemoryOrderStore(IEnumerable<Order>? seed = null) : IOrderStore
{
    // Використовуємо ToDictionary та StringComparer.OrdinalIgnoreCase, як у методичці
    private readonly Dictionary<string, Order> _items =
        (seed ?? []).ToDictionary(p => p.Id, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<Order> List() => _items.Values.ToList();

    public Order? GetById(string id) => _items.GetValueOrDefault(id);

    public void Update(Order item) => _items[item.Id] = item;

    public bool Remove(string id) => _items.Remove(id);

    public void Add(Order item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (_items.ContainsKey(item.Id))
            throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");

        _items.Add(item.Id, item);
    }
}