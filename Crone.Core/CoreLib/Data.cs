namespace Crone;

public static partial class CoreLib
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IndexInRange(this int count, int index)
    {
        return index >= 0 && index < count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool InRange<T>(this T value, T min, T max, bool exclusive = false) where T : IComparable<T>
    {
        // x.CompareTo(y):
        //	x > y => 1
        //	x = y => 0
        //  x < y => -1
        int c1 = value.CompareTo(min);
        int c2 = value.CompareTo(max);

        if (c1 == -1 || c1 == 0 && exclusive)
            return false;

        if (c2 == 1 || c2 == 0 && exclusive)
            return false;

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetValue<T>(this IDictionary item, string name)
    {
        var result = item.Contains(name) ? ConvertTo<T>(item[name]) : default(T);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetValue<T>(this OrderedDictionary item, string name)
    {
        var result = item.Contains(name) ? ConvertTo<T>(item[name]) : default(T);
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetValue<T>(this OrderedDictionary item, int index)
    {
        var result = item.Count.IndexInRange(index) ? ConvertTo<T>(item[index]) : default(T);
        return result;
    }

    public static IEnumerable<T> MapTo<T>(this IEnumerable<CoreObject> source)
    {
        var properties = GetPropertiesMap<T>();
        foreach (var row in source)
        {
            var item = Activator.CreateInstance<T>();
            foreach (var (name, prop) in properties)
            {
                if (!row.TryGetValue(name, out var value))
                    continue;

                value = ConvertTo(value, prop.PropertyType);
                prop.SetValue(item, value);
            }
            yield return item;
        }
    }

    //public static CoreDataTable ToCoreTable(this IEnumerable<CoreDataRow> source)
    //{
    //    var list = source.ToList();
    //    return new CoreDataTable(list);
    //}
}
