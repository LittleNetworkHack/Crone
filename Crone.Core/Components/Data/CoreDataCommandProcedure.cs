namespace Crone;

public sealed class CoreDataCommandProcedure : CoreComponent, ICoreDataCommand
{
    public string Text
    {
        get => Getter<string>();
        set => Setter<string>(value);
    }
    public CoreDataParameters Parameters
    {
        get => Getter<CoreDataParameters>();
        set => Setter<CoreDataParameters>(value);
    }
    public bool BindByName
    {
        get => Getter<bool>();
        set => Setter<bool>(value);
    }
    public bool DeriveParameters
    {
        get => Getter<bool>();
        set => Setter<bool>(value);
    }
    public bool IsProcedure => true;
    public CoreDataCommandProcedure(string text = null, CoreDataParameters parameters = null)
    {
        Text = text;
        Parameters = parameters ?? new CoreDataParameters();
    }
    public void Add(object value, [CallerArgumentExpression(nameof(value))] string name = default)
    {
        Parameters.Add(name, value);
    }
}
