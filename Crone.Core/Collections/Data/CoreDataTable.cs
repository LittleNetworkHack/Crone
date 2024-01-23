namespace Crone;

public class CoreDataTable<TRow> : Collection<TRow>
{
	public CoreDataTable() { }
	public CoreDataTable(IList<TRow> list) : base(list) { }
}

public sealed class CoreDataTable : CoreDataTable<CoreDataRow>
{
    public CoreDataTable() { }
    public CoreDataTable(IList<CoreDataRow> list) : base(list) { }
}
