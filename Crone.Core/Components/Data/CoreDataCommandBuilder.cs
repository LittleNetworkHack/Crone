using Oracle.ManagedDataAccess.Types;
using System.Globalization;

namespace Crone;
[InterpolatedStringHandler]
public sealed class CoreDataCommandBuilder : CoreComponent, ICoreDataCommand
{
    private readonly StringBuilder _builder = new StringBuilder();
    private readonly CoreDataParameters _parameters = new CoreDataParameters();

    public string Text => _builder.ToString();
    public CoreDataParameters Parameters => _parameters;
    public bool BindByName => true;
    public bool IsProcedure => false;
    public bool DeriveParameters => false;

    public CoreDataCommandBuilder(int literalLength, int formattedCount) { }

    public void AppendLiteral(string s)
    {
        _builder.Append(s);
    }
    public void AppendFormatted<T>(T value, string format = default, [CallerArgumentExpression(nameof(value))] string name = default)
    {
        name = CoreDataProvider.FixExpressionName(name);
        var parameter = CoreDataProvider.FormatCommandExpression(value, format);
        _parameters[name] = parameter;
        _builder.Append(CoreDataProvider.ParameterPlaceholder).Append(name);
    }

    public void AppendFormatted(string value, string format = default, [CallerArgumentExpression(nameof(value))] string name = default)
    {
        if (format?.ToUpperInvariant() == "RAW")
        {
            _builder.Append(value ?? string.Empty);
        }
        else
        {
            AppendFormatted<string>(value, format, name);
        }
    }

    public void AppendFormatted(string[] value, string format = default, [CallerArgumentExpression(nameof(value))] string name = default)
    {
        var result = JoinCsvString(value, true);
        _builder.Append(result);
    }

    public void AppendFormatted(int[] value, string format = "", [CallerArgumentExpression(nameof(value))] string name = default)
    {
        var result = JoinCsvValue(value, false, format);
        _builder.Append(result);
    }

    public void AppendFormatted(decimal[] value, string format = "", [CallerArgumentExpression(nameof(value))] string name = default)
    {
        var result = JoinCsvValue(value, false, format);
        _builder.Append(result);
    }

    public void AppendFormatted(DateTime[] value, string format = "o", [CallerArgumentExpression(nameof(value))] string name = default)
    {
        var result = JoinCsvValue(value, true, format);
        _builder.Append(result);
    }

    private string JoinCsvString(IEnumerable<string> value, bool quote)
    {
        var result = quote ? $"'{string.Join("', '", value)}'" : $"{string.Join(", ", value)}";
        return result;
    }

    private string JoinCsvValue<T>(IEnumerable<T> value, bool quote, string format) where T : IFormattable
    {
        var items = value.Select(e => e.ToString(format, CultureInfo.InvariantCulture)).ToArray();
        var result = JoinCsvString(items, quote);
        return result;
    }

    //public void AppendFormatted(OracleRefCursor value, string format = default, [CallerArgumentExpression(nameof(value))] string name = default)
    //{

    //}
}
