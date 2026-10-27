namespace Core.Storage;

using Core.Abstractions;
using Core.Domain;
using Core.Dto; // Використовуємо DTO для серіалізації
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public sealed class FileOrderStore(string path) : IOrderStore
{
    // static readonly поле для налаштувань JSON
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };
    private readonly Dictionary<string, Order> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _path = Path.GetFullPath(path);
    private bool _loaded;

    private void EnsureLoaded()
    {
        if (_loaded) return;
        if (File.Exists(_path))
        {
            var dtos = JsonSerializer.Deserialize<List<OrderDto>>(File.ReadAllText(_path)) ?? [];
            foreach (var dto in dtos)
            {
                var p = Order.FromDto(dto);
                _cache[p.Id] = p;
            }
        }
        _loaded = true;
    }

    private void Flush()
    {
        // Оператор прощення null (!) для імені каталогу
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        // Записуємо DTO на диск (Select - проєкція послідовності)
        File.WriteAllText(_path, JsonSerializer.Serialize(_cache.Values.Select(p => p.ToDto()).ToList(), Options));
    }

    public IReadOnlyList<Order> List() { EnsureLoaded(); return _cache.Values.ToList(); }

    public Order? GetById(string id) { EnsureLoaded(); return _cache.GetValueOrDefault(id); }

    public void Update(Order item) { EnsureLoaded(); _cache[item.Id] = item; Flush(); }

    public bool Remove(string id) { EnsureLoaded(); if (!_cache.Remove(id)) return false; Flush(); return true; }

    public void Add(Order item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureLoaded();

        if (_cache.ContainsKey(item.Id))
            throw new InvalidOperationException($"Запис з id={item.Id} уже існує.");

        _cache.Add(item.Id, item);
        Flush();
    }
}