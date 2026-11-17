using Core.Abstractions;
using Core.Events;
using Core.Services;
using Core.Storage;

Console.OutputEncoding = System.Text.Encoding.UTF8;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "orders.json");

IOrderStore store = useFile
    ? new FileOrderStore(dataPath)
    : new InMemoryOrderStore();

var service = new OrderService(store);
Console.WriteLine($"Сховище: {store.GetType().Name}\n");

var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnOrderChanged(object? sender, OrderChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-9} {e.OrderId} позицій: {e.LinesCount} сума: {e.Total:F2} грн";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnOrderChanged;

int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine("=== Операції (підписка є) ===");
var order = service.CreateOrder("O-" + Guid.NewGuid().ToString("N")[..6], "CUST-10");
service.AddLine(order.Id, "P-01", "Ноутбук", 1200.00m, 1);
service.AddLine(order.Id, "P-02", "Мишка", 350.50m, 2);
service.Confirm(order.Id);
Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine("\n=== Відмова: виняток є, події немає ===");
Try("неіснуючий id", () => service.AddLine("NON_EXISTENT_ID", "P-03", "Шнур", 50m, 1));
Try("рядок у підтверджене", () => service.AddLine(order.Id, "P-04", "Кабель", 80m, 1));
Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine("\n=== Після відписки (-=) ===");
service.Changed -= OnOrderChanged;
var quiet = service.CreateOrder("O-" + Guid.NewGuid().ToString("N")[..6], "CUST-20");
service.AddLine(quiet.Id, "P-05", "Монітор", 8900m, 1);
Console.WriteLine($"Подій отримано: {changes}");    

Console.WriteLine("\n=== FindAll (Func): замовлення дорожче 1000 грн ===");
foreach (var x in service.FindAll(o => o.TotalPrice > 1000))
    Console.WriteLine(x);

static void Try(string title, Action action)
{
    try { action(); }
    catch (Exception ex) { Console.WriteLine($"[ПОМИЛКА] {title}: {ex.Message}"); }
}