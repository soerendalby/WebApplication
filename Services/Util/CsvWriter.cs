using System.Text;

namespace WebApplication.Services.Util;

public static class CsvWriter
{
    // Contract specifies: UTF-8 with header row, delimiter ',', quote '"', newline '\n'
    public static byte[] WriteWithHeader<T>(IEnumerable<T> rows, IReadOnlyList<string> headers, Func<T, IEnumerable<string?>> selector)
    {
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(",", headers.Select(Escape)));
        foreach (var row in rows)
        {
            var fields = selector(row).Select(Escape);
            sb.Append(string.Join(",", fields));
            sb.Append('\n');
        }
        // UTF-8 BOM for broad tool compatibility (e.g., Excel)
        var utf8bom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
        return utf8bom.GetBytes(sb.ToString());
    }

    private static string Escape(string? value)
    {
        var v = value ?? string.Empty;
        var needsQuotes = v.Contains('"') || v.Contains(',') || v.Contains('\n') || v.Contains('\r');
        if (v.Contains('"')) v = v.Replace("\"", "\"\"");
        return needsQuotes ? $"\"{v}\"" : v;
    }
}
