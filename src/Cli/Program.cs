using Core.Dto;
using Core.Import;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

ImportResult<OrderDto> result = OrderCsvImporter.Load(path);

Console.WriteLine($"Завантажено замовлень: {result.Items.Count}");
foreach (OrderDto order in result.Items.Take(5))
{
    Console.WriteLine($" {order.Id,-6} {order.Name,-26} {order.Price,8:F2} грн");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine($"\nПропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
    {
        Console.WriteLine($" ! {e}");
    }
}

return 0;