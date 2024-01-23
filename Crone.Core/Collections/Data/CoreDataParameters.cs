namespace Crone;
public sealed class CoreDataParameters : CoreDataRecord
{
	public CoreDataParameters() { }
	public CoreDataParameters(ICoreObject data) : base(data) { }
	public CoreDataParameters(int capacity) : base(capacity) { }
	public CoreDataParameters(IEnumerable<KeyValuePair<string, object>> collection) : base(collection) { }

	public void Add(object value, [CallerArgumentExpression(nameof(value))] string name = default)
	{
		name = CoreDataProvider.FixExpressionName(name);
		Add(name, value);
	}

	internal void AddCore(string key, object value)
	{
		base.Add(key, value);
	}

	public void Output<T>(out T value, [CallerArgumentExpression(nameof(value))] string name = default)
	{
		name = CoreDataProvider.FixOutputName(name);
		value = GetValue<T>(name);
	}
}