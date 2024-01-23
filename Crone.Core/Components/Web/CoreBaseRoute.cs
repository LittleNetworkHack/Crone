using System.Web;

namespace Crone;

[InterpolatedStringHandler]
public sealed class CoreRouteBuilder
{
	private readonly StringBuilder _builder = new StringBuilder();

	public CoreRouteBuilder(int literalLength = 0, int formattedCount = 0) { }

	public static implicit operator CoreRouteBuilder(string value)
	{
		var result = new CoreRouteBuilder();
		result.AppendLiteral(value);
		return result;
	}

	public static CoreRouteBuilder operator +(CoreRouteBuilder a, CoreRouteBuilder b)
	{
		var result = new CoreRouteBuilder();
		result.AppendFormatted(a);
		result.AppendFormatted(b);
		return result;
	}

	public static CoreRouteBuilder operator /(CoreRouteBuilder a, CoreRouteBuilder b)
	{
		var result = new CoreRouteBuilder();
		result.AppendFormatted(a);
		result.AppendLiteral("/");
		result.AppendFormatted(b);
		return result;
	}

	public void AppendLiteral(string s)
	{
		_builder.Append(s);
	}

	public void AppendFormatted(CoreRouteBuilder value, string format = default, [CallerArgumentExpression(nameof(value))] string name = default)
	{
		_builder.Append(value.ToString());
	}

	public void AppendFormatted<T>(T value, string format = default, [CallerArgumentExpression(nameof(value))] string name = default)
	{
		var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
		if (type == typeof(int))
		{
			name = name + ":int";
		}
		_builder.Append('{').Append(name).Append('}');
	}

	public string Generate(IEnumerable<KeyValuePair<string, object>> routeValues)
	{
		var result = _builder.ToString();
		foreach (var (key, value) in routeValues)
		{
			var name = "{" + key + (value is int ? ":int" : string.Empty) + "}";
			var text = CoreLib.ConvertTo<string>(value);
			result = result.Replace(name, text);
		}
		return result;
	}

	public override string ToString()
	{
		return _builder.ToString();
	}
}

public abstract class CoreBaseRoute : CoreComponent
{
	public virtual bool IsPublic => false;
	public virtual bool IsProtected => false;

	public string Lookup => nameof(Lookup);
	public string Callback => nameof(Callback);

	public virtual string Controller { get; }
	public virtual string Action { get; }
	public virtual string Element { get; }
	public virtual string RuleCode
	{
		get
		{
			if (Controller is null || Action is null)
			{
				return null;
			}

			if (Element is null)
			{
				return $"{Controller}/{Action}";
			}

			return $"{Controller}/{Action}#{Element}";
		}
	}

	public OrderedDictionary<string, object> QueryParameters { get; } = new OrderedDictionary<string, object>();

	public virtual CoreRouteBuilder Template => string.Empty;
	public virtual CoreRouteBuilder Argument => string.Empty;
	public CoreBaseRoute() { }
	public CoreBaseRoute(Dictionary<string, CoreBaseRoute> routes) => routes.Add(RuleCode, this);
	public string Generate()
	{
		var result = Template.Generate(this) + GetQuery();
		return result;
	}
	public string Generate(string query)
	{
		var result = Generate() + query;
		return result;
	}
	public string WithAttribute(string name)
	{
		return RuleCode + "-" + name;
	}

	public CoreBaseRoute WithQuery(object value, [CallerArgumentExpression("value")] string name = null)
	{
		QueryParameters.Add(name, value);
		return this;
	}

	public string GetQuery()
	{
		if (QueryParameters.Count == 0)
		{
			return string.Empty;
		}
		var list = new List<string>();
		foreach (var (key, value) in QueryParameters)
		{
			var text = CoreLib.ConvertTo<string>(value);
			var encoded = HttpUtility.UrlEncode(text);
			list.Add($"{key}={encoded}");
		}
		var result = "?" + string.Join('&', list);
		return result;
	}
}