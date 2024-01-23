namespace Crone;

public static partial class CoreLib
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> keyValue, out TKey key, out TValue value)
    {
        key = keyValue.Key;
        value = keyValue.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TryDispose(this object value)
    {
        TryDispose(value as IDisposable);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void TryDispose(this IDisposable value)
    {
        try
        {
            value?.Dispose();
        }
        catch { }
    }
}
