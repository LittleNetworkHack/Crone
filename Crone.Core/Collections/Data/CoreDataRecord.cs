namespace Crone;
public abstract class CoreDataRecord : CoreComponent
{
    protected CoreDataRecord() { }
    protected CoreDataRecord(ICoreObject data) : base(data) { }
    protected CoreDataRecord(int capacity) : base(capacity) { }
    protected CoreDataRecord(IEnumerable<KeyValuePair<string, object>> collection) : base(collection) { }
}
