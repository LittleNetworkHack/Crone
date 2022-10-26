#if TODO
using StackExchange.Redis;

namespace Crone;
public static class RedisDatabase
{
    public static string ConnectionConfig { get; set; }

    private static ConnectionMultiplexer _connection;
    public static ConnectionMultiplexer Connection => _connection ?? (_connection = ConnectionMultiplexer.Connect(ConnectionConfig));

    public static TimeSpan DefaultCacheExpiry { get; set; } = TimeSpan.FromMinutes(20);
    public static TimeSpan DefaultSessionExpiry { get; set; } = TimeSpan.FromMinutes(20);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ConvertFromRedisValue<T>(RedisValue value)
    {
        return value.HasValue ? JsonSerializer.Deserialize<T>((string)value) : default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RedisValue ConvertToRedisValue<T>(T value)
    {
        return (RedisValue)JsonSerializer.Serialize(value);
    }

    private static T GetValueInternal<T>(RedisKey key, RedisValue field)
    {
        var cache = Connection.GetDatabase();
        var value = cache.HashGet(key, field);
        return ConvertFromRedisValue<T>(value);
    }

    private static void SetValueInternal<T>(RedisKey key, RedisValue field, T value, TimeSpan? expiry)
    {
        var cache = Connection.GetDatabase();
        var item = ConvertToRedisValue(value);
        cache.HashSet(key, field, item);
        cache.KeyExpire(key, expiry);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetCacheValue<T>(RedisKey key, RedisValue field)
    {
        return GetValueInternal<T>(key, field);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetCacheValue<T>(RedisKey key, RedisValue field, T value)
    {
        SetValueInternal(key, field, value, DefaultCacheExpiry);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetCacheValue<T>(RedisKey key, RedisValue field, T value, TimeSpan? expiry)
    {
        SetValueInternal(key, field, value, expiry);
    }

    public static void Clear(RedisKey key)
    {
        var cache = Connection.GetDatabase();
        cache.KeyDelete(key);
    }
}
#endif