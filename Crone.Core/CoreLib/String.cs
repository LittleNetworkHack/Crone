namespace Crone;

public static partial class CoreLib
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMinLength(this string value, int min)
    {
        return (value?.Length ?? 0) >= min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMaxLength(this string value, int max)
    {
        return (value?.Length ?? 0) <= max;
    }

    public static void WriteIf(this TextWriter writer, bool condition, string value)
    {
        if (condition)
        {
            writer.Write(value);
        }
    }

    public static void WriteLineIf(this TextWriter writer, bool condition, string value)
    {
        if (condition)
        {
            writer.WriteLine(value);
        }
    }

    public static string PrependLines(this string @this, string value, string terminator = null)
    {
        return value + @this.Replace(Environment.NewLine, Environment.NewLine + value) + (string.IsNullOrEmpty(terminator) ? string.Empty : terminator);
    }

    public static string UnindentLines(this string @this, string newline = "\r\n")
    {
        var lines = @this.Split(new string[] { newline }, StringSplitOptions.None);
        var minIndent = lines.Where(e => e.Trim() != string.Empty).Select(e => e.TakeWhile(ch => ch == ' ').Count()).Min();
        var formatted = lines.Select(e => e.Trim() == string.Empty ? string.Empty : e.Substring(minIndent));
        var result = string.Join(Environment.NewLine, formatted);
        return result;
    }

    public static string ToCsvString<TSource>(this IEnumerable<TSource> source, string separator, Func<TSource, string> selector)
    {
        return source.Select(selector).ToCsvString(separator);
    }

    public static string ToCsvString(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source);
    }
}
