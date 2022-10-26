namespace Crone;

public static partial class CoreLib
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Create<T>()
    {
        return Activator.CreateInstance<T>();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TInterface Create<TInterface>(Type implementation)
    {
        return (TInterface)Activator.CreateInstance(implementation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static object Create(Type type)
    {
        return Activator.CreateInstance(type);
    }

    public static object GetDefaultValue(Type type)
    {
        if (type == null || !type.IsValueType || type == typeof(void))
            return null;

        return Create(type);
    }

    public static Type GetNullableType(Type type)
    {
        var defaultValue = GetDefaultValue(type);
        if (defaultValue == null)
            return type;

        return typeof(Nullable<>).MakeGenericType(type);
    }

    public static Dictionary<string, PropertyInfo> GetPropertiesMap<T>()
    {
        return GetPropertiesMap(typeof(T), true, true, true);
    }

    public static Dictionary<string, PropertyInfo> GetPropertiesMap(Type type)
    {
        return GetPropertiesMap(type, true, true, true);
    }

    public static Dictionary<string, PropertyInfo> GetPropertiesMap(Type type, bool ignoreCase, bool ignoreReadOnly, bool ignoreIndexer)
    {
        var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        var items = type.GetProperties()
                        .WhereIf(ignoreReadOnly, p => p.CanWrite)
                        .WhereIf(ignoreIndexer, e => e.GetIndexParameters().Length == 0);
        var result = items.ToDictionary(e => e.Name, comparer);
        return result;
    }
}
