# CrossApp 
Наскрізний проєкт з крос-платформного програмування. 
Предметна область: Замовлення. Сутності: customer, product, order, orderline. 
Призначення: оформлення замовлень і підрахунок сум. 
## Запуск 
dotnet build<br>
dotnet run --project src/Cli 
## Середовище 
.NET SDK 10.0, Windows 11 x64
---

# Лабораторна робота 2

## Структура проєкту
```text
CrossApp/
  CrossApp.slnx
  README.md
  .gitignore
  src/
    Core/
      Core.csproj
      EnvironmentInfo.cs
    Cli/
      Cli.csproj
      Program.cs
```

## Команди збірки, запуску та публікації
### Збірка
dotnet build

### Запуск CLI проєкту
dotnet run --project src/Cli

### Публікація (Self-contained)
dotnet publish src/Cli -c Release -r win-x64 --self-contained true -f net10.0

### Публікація (Framework-dependent)
dotnet publish src/Cli -c Release -r win-x64 --self-contained false -f net10.0

## Порівняння режимів публікації

| RID | Режим | Розмір publish | Потрібен встановлений runtime |
| :--- | :--- | :--- | :--- |
| `win-x64` | self-contained | ~76.85 МБ | ні |
| `win-x64` | framework-dependent | ~0.19 МБ | так (.NET 10) |

## Лабораторна робота 4: Доменна модель та інваріанти

### Перелік інваріантів домену Замовлення (Order + OrderLine)

1. **Обов'язкові ідентифікатори:** Номер замовлення (`id`), клієнта (`customerId`) та товару (`productId`) не можуть бути порожніми або складатися лише з пробілів (`ArgumentException`).
2. **Невід'ємність ціни:** Ціна товару (`price`) у рядку замовлення повинна бути >= 0 (`ArgumentOutOfRangeException`).
3. **Коректна кількість:** Кількість товару (`quantity`) у рядку має бути строго більшою за нуль (`> 0`) (`ArgumentOutOfRangeException`).
4. **Захист від підтвердження порожнього замовлення:** Неможливо підтвердити замовлення, яке не містить жодного рядка (`InvalidOperationException`).
5. **Незмінність підтвердженого замовлення:** Після підтвердження замовлення (`IsConfirmed = true`) заборонено додавати нові рядки позицій або підтверджувати його повторно (`InvalidOperationException`).