using static eDrazba.CoreDataProvider;
namespace Crone;
[InterpolatedStringHandler]
public sealed class CoreDataCommandBuilder : CoreComponent, ICoreDataCommand
{
	private static readonly string _arrayValueDefault = "NULL";

	private readonly StringBuilder _builder = new StringBuilder();
	private readonly CoreDataParameters _parameters = new CoreDataParameters();

	public string Text => _builder.ToString();
	public CoreDataParameters Parameters => _parameters;
	public bool BindByName { get; set; } = false;
	public bool IsProcedure => false;
	public bool DeriveParameters => false;

	public CoreDataCommandBuilder(int literalLength = 0, int formattedCount = 0) { }

	public static implicit operator CoreDataCommandBuilder(string value)
	{
		var result = new CoreDataCommandBuilder();
		result.AppendLiteral(value);
		return result;
	}

	public void AppendLiteral(string s)
	{
		_builder.Append(s);
	}

	public void AppendFormatted<T>(T value, string format = default, [CallerArgumentExpression(nameof(value))] string name = default)
	{
		name = FixExpressionName(name);
		var parameter = FormatCommandExpression(value, format);

		if (BindByName)
		{
			_parameters[name] = parameter;
			_builder.Append(ParameterPlaceholder).Append(name);
		}
		else
		{
			_parameters.AddCore($"p{_parameters.Count:00}_{name}", parameter);
			_builder.Append(ParameterPlaceholder);
		}
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
		if (format == "LEN")
		{
			var len = value?.Length ?? 0;
			AppendFormatted<int>(len, null, name);
			return;
		}

		var result = JoinCsvValue(value, false, format);
		_builder.Append(result);
	}

	public void AppendFormatted(int?[] value, string format = "", [CallerArgumentExpression(nameof(value))] string name = default)
	{
		if (format == "LEN")
		{
			var len = value?.Length ?? 0;
			AppendFormatted<int>(len, null, name);
			return;
		}
		//Debugger.Break(); // Are you sure you want nullable int array in sql?
		var array = value?.Where(e => e is int).Select(e => e.Value);
		var result = JoinCsvValue(array, false, format);
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

	internal static string JoinCsvString(IEnumerable<string> value, bool quote)
	{
		if (value is null || !value.Any())
		{
			return _arrayValueDefault;
		}
		var result = quote ? $"'{string.Join("', '", value)}'" : $"{string.Join(", ", value)}";
		return result;
	}

	internal static string JoinCsvValue<T>(IEnumerable<T> value, bool quote, string format) where T : IFormattable
	{
		if (value is null || !value.Any())
		{
			return _arrayValueDefault;
		}
		var items = value.Select(e => e?.ToString(format, CultureInfo.InvariantCulture) ?? "NULL").ToArray();
		var result = JoinCsvString(items, quote);
		return result;
	}
}
