namespace Crone;

public static partial class CoreLib
{
    public static Exception AsDataException(this Exception ex, [CallerMemberName] string name = default)
    {
        return new DataException($"DataException thrown in '{name}', check inner exception.", ex);
    }
}
