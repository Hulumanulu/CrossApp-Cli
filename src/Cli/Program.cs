using Core.Domain;

Console.WriteLine("=== СЦЕНАРІЙ 1: Успішна робота з доменною моделлю ===");

Order order = Order.Create("O-2026-001", "CUST-777");
Console.WriteLine($"Створено: {order}");

order.AddLine("P-101", "Ноутбук Lenovo IdeaPad", 18500.50m, 1);
order.AddLine("P-102", "Миша безпровідна Logitech", 650.00m, 2);

Console.WriteLine("\nДодано товари:");
foreach (var line in order.Lines)
{
    Console.WriteLine($"  * {line}");
}

Console.WriteLine($"\nЗагальна вартість: {order.TotalPrice:F2} грн");

order.Confirm();
Console.WriteLine($"\nПісля підтвердження: {order}");

Console.WriteLine("\n" + new string('=', 60) + "\n");

Console.WriteLine("=== СЦЕНАРІЙ 2: Перевірка захисту інваріантів (помилки) ===");

TryDo("Спроба створити замовлення з порожнім ID", () => Order.Create("", "CUST-100"));

TryDo("Спроба додати товар з від'ємною ціною", () => {
    Order temp = Order.Create("O-TEST", "CUST-100");
    temp.AddLine("P-01", "Монітор", -500.00m, 1);
});

TryDo("Спроба додати товар з нульовою кількістю", () => {
    Order temp = Order.Create("O-TEST", "CUST-100");
    temp.AddLine("P-01", "Монітор", 5000.00m, 0);
});

TryDo("Спроба підтвердити порожнє замовлення", () => {
    Order emptyOrder = Order.Create("O-EMPTY", "CUST-100");
    emptyOrder.Confirm();
});

TryDo("Спроба додати новий товар у вже підтверджене замовлення", () => {
    order.AddLine("P-103", "Клавіатура", 1200.00m, 1);
});

// Допоміжна локальна функція для чистого виводу повідомлень про помилки
static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($" ! {title}: ВИНОК НЕ СПРАЦЮВАВ (Інваріант відсутній!)");
    }
    catch (Exception ex)
    {
        Console.WriteLine($" ✓ {title}:");
        Console.WriteLine($"   [{ex.GetType().Name}] {ex.Message}");
    }
}