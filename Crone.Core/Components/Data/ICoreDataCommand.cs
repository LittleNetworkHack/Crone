namespace Crone;
public interface ICoreDataCommand
{
    string Text { get; }
    CoreDataParameters Parameters { get; }
    public bool IsProcedure { get; }
    public bool BindByName { get; }
    public bool DeriveParameters { get; }
}
