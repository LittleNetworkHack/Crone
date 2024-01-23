namespace Crone;

public static partial class CoreLib
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Truncate(this string value, int max)
	{
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }
        if (value.Length <= max)
        {
            return value;
        }
		var result = value.Substring(0, max);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string NormalizeLineBreaks(string value)
    {
        var result = Regex.Replace(value, @"\r\n?|\n", Environment.NewLine);
        return result;
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMinByteLength(this string value, int min)
    {
        var len = value is not null ? Encoding.UTF8.GetByteCount(value) : 0;
        return len >= min;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsMaxByteLength(this string value, int max)
    {
        var len = value is not null ? Encoding.UTF8.GetByteCount(value) : 0;
        return len <= max;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteIf(this TextWriter writer, bool condition, string value)
    {
        if (condition)
        {
            writer.Write(value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteLineIf(this TextWriter writer, bool condition, string value)
    {
        if (condition)
        {
            writer.WriteLine(value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncloseString(this string @this, char value)
    {
        return value + @this + value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncloseString(this string @this, string value)
    {
        return value + @this + value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncloseString(this string @this, char before, char after)
    {
        return before + @this + after;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string EncloseString(this string @this, string before, string after)
    {
        return before + @this + after;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SingleQuote(this string @this)
    {
        return EncloseString(@this, "'");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DoubleQuote(this string @this)
    {
        return EncloseString(@this, '"');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string BracketQuote(this string @this)
    {
        return EncloseString(@this, '{', '}');
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

    private static readonly Regex regex_for_tokens = new Regex(@"{(\w+)}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string ReplaceTokensConverter(ICoreObject record, Match match)
    {
        if (match is null)
        {
            return string.Empty;
        }
        if (match.Groups.Count == 0)
        {
            return string.Empty;
        }
        var key = match.Groups[1].Value;
        var value = record.GetValue<object>(key);
        if (IsNullOrDBNull(value))
        {
            return string.Empty;
        }
        if (value is string text)
        {
            return text.Trim();
        }
        if (value is int[] array)
        {
            var joined = string.Join(',', array.Select(e => e.ToString().SingleQuote()));
            return joined;
		}
        var result = value.ToString()?.Trim();
		return result;
	}

	public static string ReplaceTokens(this string template, ICoreObject record)
    {
        var result = regex_for_tokens.Replace(template, m => ReplaceTokensConverter(record, m));
        return result;
    }
}
