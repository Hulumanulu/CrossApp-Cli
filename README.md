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

## Лабораторна робота 2

### Структура проєкту
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

### Команди збірки, запуску та публікації
# Збірка рішення
dotnet build

# Запуск CLI проєкту
dotnet run --project src/Cli

# Публікація (Self-contained)
dotnet publish src/Cli -c Release -r win-x64 --self-contained true -f net10.0

# Публікація (Framework-dependent)
dotnet publish src/Cli -c Release -r win-x64 --self-contained false -f net10.0

### Порівняння режимів публікації

| RID | Режим | Розмір publish | Потрібен встановлений runtime |
| :--- | :--- | :--- | :--- |
| `win-x64` | self-contained | ~76.85 МБ | ні |
| `win-x64` | framework-dependent | ~0.19 МБ | так (.NET 10) |