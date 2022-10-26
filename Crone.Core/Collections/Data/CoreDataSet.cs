namespace Crone;

public sealed class CoreDataSet : CoreDataRecord
{
    public CoreDataSet() { }
    public CoreDataSet(ICoreObject data) : base(data) { }
    public CoreDataSet(int capacity) : base(capacity) { }
    public CoreDataSet(IEnumerable<KeyValuePair<string, object>> collection) : base(collection) { }

    public void Output<T>(out T value, [CallerArgumentExpression(nameof(value))] string name = default)
    {
        name = CoreDataProvider.FixOutputName(name);
        value = GetValue<T>(name);
    }
}