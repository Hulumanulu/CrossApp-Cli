using System.Globalization;
using Core.Dto;

namespace Core.Import;

public static class OrderCsvImporter
{
    private const char Separator = ';';

    public static ImportResult<OrderDto> Load(string path)
    {
        var items = new List<OrderDto>();
        var errors = new List<string>();

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            if (number == 1 && line.StartsWith("id", StringComparison.OrdinalIgnoreCase))
                continue;

            switch (ParseLine(line))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {number}: {failed.Reason}");
                    break;
            }
        }

        return new ImportResult<OrderDto>(items, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            { Length: < 3 } =>
                new ParseFailed($"очікую 3 колонки, отримав {parts.Length}"),

            [_, "", _] =>
                new ParseFailed("Назва товару/замовлення порожня"),

            [_, _, var priceStr] when !decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal p) || p < 0 =>
                new ParseFailed($"ціна '{priceStr}' не є некоректним додатним числом"),

            [var id, var name, var priceStr] =>
                new ParseOk(new OrderDto(id, name, decimal.Parse(priceStr, CultureInfo.InvariantCulture))),

            _ => new ParseFailed($"занадто багато колонок: {parts.Length}")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseOk(OrderDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}