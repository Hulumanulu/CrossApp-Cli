using Core.Abstractions;
using Core.Storage;
using Core.Services;
using System.IO;
using System;
using System.Linq;

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "orders.json");

IOrderStore store = useFile
    ? new FileOrderStore(dataPath)
    : new InMemoryOrderStore();

var service = new OrderService(store);
Console.WriteLine($"Сховище: {store.GetType().Name}\n");

try
{
    var created = service.CreateOrder("O-555", "CUST-10");
    service.AddLine(created.Id, "P-01", "Клавіатура", 1200.00m, 1);
    service.Confirm(created.Id);
}
catch (InvalidOperationException ex) when (ex.Message.Contains("уже існує")) { }

foreach (var o in service.All())
{
    Console.WriteLine($"{o.ToString()}");
}

Console.WriteLine("\n=== Сценарій відмови ===");
try
{
    service.AddLine("NON_EXISTENT_ID", "P-02", "Миша", 500m, 1);
}
catch (Exception ex)
{
    Console.WriteLine($"[ПОМИЛКА] {ex.Message}");
}