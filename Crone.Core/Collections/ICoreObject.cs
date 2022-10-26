namespace Crone;

public interface ICoreObject : IEnumerable<KeyValuePair<string, object>>
{
    #region Properties

    object this[int index] { get; set; }
    object this[string key] { get; set; }

    int Count { get; }
    IReadOnlyList<string> Keys { get; }
    IReadOnlyList<object> Values { get; }
    IEqualityComparer<string> Comparer { get; }

    #endregion Properties

    #region Methods

    int EnsureCapacity(int capacity);

    ICoreObject GetDataObject();
    void SetDataObject(ICoreObject dataObject);

    #endregion Methods

    #region Get/Set

    T GetValue<T>(int index);
    T GetValue<T>(string key);
    void SetValue<T>(string key, object value);

    #endregion Get/Set

    #region TryGetValue

    bool TryGetValue(string key, out object value);
    bool TryGetValue<T>(string key, out T value);

    #endregion TryGetValue

    #region Add/Insert

    void Add(string key, object value);

    void Insert(int index, string key, object value);

    #endregion Add/Insert

    #region Clear/Remove

    void Clear();
    bool Remove(string key);
    void RemoveAt(int index);

    #endregion Clear/Remove

    #region Key/Index

    bool ContainsKey(string key);
    
    int IndexOf(string key);

    #endregion Key/Index
}