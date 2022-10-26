namespace Crone;
public sealed class CoreObject : ICoreObject
{
    #region Properties

    private static IEqualityComparer<string> DefaultComparer => StringComparer.OrdinalIgnoreCase;

    private readonly OrderedDictionary<string, object> _data;

    public int Count => _data.Count;
    public IReadOnlyList<string> Keys => _data.Keys;
    public IReadOnlyList<object> Values => _data.Values;

    public object this[string key]
    {
        get => _data[key];
        set => _data[key] = value;
    }

    public object this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }

    public IEqualityComparer<string> Comparer => _data.Comparer;

    #endregion Properties

    #region Constructors

    public CoreObject()
    {
        _data = new OrderedDictionary<string, object>(DefaultComparer);
    }
    public CoreObject(int capacity)
    {
        _data = new OrderedDictionary<string, object>(capacity, DefaultComparer);
    }
    public CoreObject(IEnumerable<KeyValuePair<string, object>> collection)
    {
        _data = new OrderedDictionary<string, object>(collection, DefaultComparer);
    }

    #endregion Constructors

    #region Methods

    public int EnsureCapacity(int capacity)
    {
        return _data.EnsureCapacity(capacity);
    }

    public ICoreObject GetDataObject()
    {
        return this;
    }

    public void SetDataObject(ICoreObject dataObject)
    {
        throw new NotSupportedException();
    }

    #endregion Methods

    #region Get/Set

    public T GetValue<T>(int index)
    {
        if (!Count.IndexInRange(index))
            return default;

        return CoreLib.ConvertTo<T>(this[index]);
    }
    public T GetValue<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return default;
        }
        if (!_data.TryGetValue(key, out var value))
        {
            return default;
        }
        return CoreLib.ConvertTo<T>(value);
    }
    public void SetValue<T>(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        _data[key] = CoreLib.ConvertTo<T>(value);
    }

    #endregion Get/Set

    #region TryGetValue

    public bool TryGetValue(string key, out object value)
    {
        if (string.IsNullOrEmpty(key))
        {
            value = default;
            return false;
        }
        return _data.TryGetValue(key, out value);
    }
    public bool TryGetValue<T>(string key, out T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            value = default;
            return false;
        }
        if (!_data.TryGetValue(key, out var temp))
        {
            value = default;
            return false;
        }

        value = CoreLib.ConvertTo<T>(temp);
        return true;
    }

    #endregion TryGetValue

    #region Add/Insert

    public void Add(string key, object value)
    {
        _data.Add(key, value);
    }

    public void Insert(int index, string key, object value)
    {
        _data.Insert(index, key, value);
    }

    #endregion Add/Insert

    #region Clear/Remove

    public void Clear()
    {
        _data.Clear();
    }

    public bool Remove(string key)
    {
        return _data.Remove(key);
    }
    public void RemoveAt(int index)
    {
        _data.RemoveAt(index);
    }

    #endregion Clear/Remove

    #region Key/Index

    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }
    public int IndexOf(string key)
    {
        return _data.IndexOf(key);
    }

    #endregion Methods

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => _data.GetEnumerator();

    #endregion IEnumerable
}