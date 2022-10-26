namespace Crone;

public sealed class CoreDataRow : CoreDataRecord
{
    public CoreDataRow() { }
    public CoreDataRow(ICoreObject data) : base(data) { }
    public CoreDataRow(int capacity) : base(capacity) { }
    public CoreDataRow(IEnumerable<KeyValuePair<string, object>> collection) : base(collection) { }

    public void Output<T>(out T value, [CallerArgumentExpression(nameof(value))] string name = default)
    {
        name = CoreDataProvider.FixOutputName(name);
        value = GetValue<T>(name);
    }
}