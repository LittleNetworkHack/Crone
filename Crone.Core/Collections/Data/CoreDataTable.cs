namespace Crone;

public sealed class CoreDataTable : Collection<CoreDataRow>
{
    public CoreDataTable() { }
    public CoreDataTable(IList<CoreDataRow> list) : base(list) { }
}
