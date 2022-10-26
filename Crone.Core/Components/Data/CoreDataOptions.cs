namespace Crone;

public class CoreDataOptions : CoreComponent
{
    public string ConnectionString
    {
        get => Getter<string>();
        set => Setter<string>(value);
    }
}
