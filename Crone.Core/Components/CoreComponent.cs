namespace Crone;

public abstract class CoreComponent : ICoreComponent
{
    #region Properties

    protected internal ICoreObject DataObject { get; internal set; }

    public int Count => DataObject.Count;
    public IReadOnlyList<string> Keys => DataObject.Keys;
    public IReadOnlyList<object> Values => DataObject.Values;

    public virtual object this[string key]
    {
        get => DataObject[key];
        set => DataObject[key] = value;
    }

    public virtual object this[int index]
    {
        get => DataObject[index];
        set => DataObject[index] = value;
    }

    public virtual IEqualityComparer<string> Comparer => DataObject.Comparer;

    #endregion Properties

    #region Constructors

    public CoreComponent(ICoreObject data)
    {
        DataObject = data ?? throw new ArgumentNullException(nameof(data));
    }

    public CoreComponent()
    {
        DataObject = new CoreObject();
    }
    public CoreComponent(int capacity)
    {
        DataObject = new CoreObject(capacity);
    }
    public CoreComponent(IEnumerable<KeyValuePair<string, object>> collection)
    {
        DataObject = new CoreObject(collection);
    }

    #endregion Constructors

    #region Getter/Setter

    public T Getter<T>([CallerMemberName] string name = null)
    {
        return GetValue<T>(name);
    }

    public void Setter<T>(object value, [CallerMemberName] string name = null)
    {
        SetValue<T>(name, value);
    }

    #endregion Getter/Setter

    #region Get/Set 

    public virtual T GetValue<T>(int index)
    {
        return DataObject.GetValue<T>(index);
    }
    public virtual T GetValue<T>(string key)
    {
        return DataObject.GetValue<T>(key);
    }
    public virtual void SetValue<T>(string key, object value)
    {
        DataObject.SetValue<T>(key, value);
    }

    #endregion Get/Set

    #region TryGetValue

    public virtual bool TryGetValue(string key, out object value)
    {
        return DataObject.TryGetValue(key, out value);
    }
    public virtual bool TryGetValue<T>(string key, out T value)
    {
        return DataObject.TryGetValue(key, out value);
    }

    #endregion TryGetValue

    #region Add/Insert

    public virtual void Add(string key, object value)
    {
        DataObject.Add(key, value);
    }

    public virtual void Insert(int index, string key, object value)
    {
        DataObject.Insert(index, key, value);
    }

    #endregion Add/Insert

    #region Clear/Remove

    public virtual void Clear()
    {
        DataObject.Clear();
    }

    public virtual bool Remove(string key)
    {
        return DataObject.Remove(key);
    }
    public virtual void RemoveAt(int index)
    {
        DataObject.RemoveAt(index);
    }

    #endregion Clear/Remove

    #region Key/Index

    public virtual bool ContainsKey(string key)
    {
        return DataObject.ContainsKey(key);
    }
    public virtual int IndexOf(string key)
    {
        return DataObject.IndexOf(key);
    }

    #endregion Methods

    #region Misc

    public int EnsureCapacity(int capacity)
    {
        return DataObject.EnsureCapacity(capacity);
    }

    public ICoreObject GetDataObject()
    {
        return DataObject;
    }

    public void SetDataObject(ICoreObject dataObject)
    {
        DataObject = dataObject;
    }

    #endregion Misc

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator() => DataObject.GetEnumerator();
    public virtual IEnumerator<KeyValuePair<string, object>> GetEnumerator() => DataObject.GetEnumerator();

    #endregion IEnumerable
}