namespace Crone;
public sealed class CoreDataDynamic : CoreComponent
{
	public CoreDataDynamic() { }
	public CoreDataDynamic(ICoreObject data) : base(data) { }
	public CoreDataDynamic(int capacity) : base(capacity) { }
	public CoreDataDynamic(IEnumerable<KeyValuePair<string, object>> collection) : base(collection) { }
}