using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public delegate void PropertySetter<TClass, TMember>(TClass instance, TMember value);
	public delegate TMember PropertyGetter<TClass, TMember>(TClass instance);

	public interface IPropertyKey
	{
		string Name { get; }
		Type ClassType { get; }
		Type PropertyType { get; }
		PropertyInfo Info { get; }

		bool IsReadOnly { get; }
		IEqualityComparer EqualityComparer { get; }
		object DefaultValue { get; }

		object GetBoxedValue(object instance);
		void SetBoxedValue(object instance, object value);
	}

	public interface IPropertyKey<TProperty> : IPropertyKey
	{
		new IEqualityComparer<TProperty> EqualityComparer { get; }
		new TProperty DefaultValue { get; }

		TProperty GetValue(object instance);
		void SetValue(object instance, TProperty value);
	}

	public interface IPropertyKey<TClass, TProperty> : IPropertyKey<TProperty>, IPropertyKey
	{
		TProperty GetValue(TClass instance);
		void SetValue(TClass instance, TProperty value);
	}

	public sealed class PropertyKey<TClass, TProperty> :
		IPropertyKey, IPropertyKey<TProperty>, IPropertyKey<TClass, TProperty>,
		IEquatable<IPropertyKey>, IEqualityComparer<TClass>
	{
		#region Fields

		private PropertyInfo propertyInfo;
		private PropertyGetter<TClass, TProperty> getter;
		private PropertySetter<TClass, TProperty> setter;

		#endregion Fields

		#region Properties

		public string Name => propertyInfo.Name;
		public Type ClassType => typeof(TClass);
		public Type PropertyType => typeof(TProperty);
		public PropertyInfo Info => propertyInfo;

		public PropertyGetter<TClass, TProperty> GetMethod => getter;
		public PropertySetter<TClass, TProperty> SetMethod => setter;

		public bool IsReadOnly => setter == null;
		public TProperty DefaultValue => default(TProperty);

		public ObjectComparer<TProperty> EqualityComparer => ObjectComparer<TProperty>.Comparer;

		#endregion Properties

		#region Constructors

		public PropertyKey(string name)
		{
			propertyInfo = typeof(TClass).GetProperty(name);
			InitializeDelegates();
		}

		public PropertyKey(PropertyInfo info)
		{
			this.propertyInfo = info;
			if (info.GetIndexParameters().Length != 0)
				return;
			InitializeDelegates();
		}

		private void InitializeDelegates()
		{
			MethodInfo mi_get = Info.GetGetMethod();
			MethodInfo mi_set = Info.GetSetMethod();

			if (mi_get != null)
				getter = (PropertyGetter<TClass, TProperty>)Delegate.CreateDelegate(typeof(PropertyGetter<TClass, TProperty>), mi_get);

			if (mi_set != null)
				setter = (PropertySetter<TClass, TProperty>)Delegate.CreateDelegate(typeof(PropertySetter<TClass, TProperty>), mi_set);
		}

		#endregion Constructors

		#region IPropertyKey

		IEqualityComparer IPropertyKey.EqualityComparer => EqualityComparer;
		object IPropertyKey.DefaultValue => DefaultValue;

		public object GetBoxedValue(object instance) => getter((TClass)instance);

		public void SetBoxedValue(object instance, object value) => setter((TClass)instance, (TProperty)value);

		#endregion IPropertyKey

		#region IPropertyKey<TProperty>

		IEqualityComparer<TProperty> IPropertyKey<TProperty>.EqualityComparer => EqualityComparer;
		TProperty IPropertyKey<TProperty>.DefaultValue => DefaultValue;

		public TProperty GetValue(object instance) => getter((TClass)instance);

		public void SetValue(object instance, TProperty value) => setter((TClass)instance, value);

		#endregion IPropertyKey<TProperty>

		#region IPropertyKey<TClass, TProperty>

		public TProperty GetValue(TClass instance) => getter(instance);

		public void SetValue(TClass instance, TProperty value) => setter(instance, value);

		#endregion IPropertyKey<TClass, TProperty>

		#region IEquatable<IPropertyKey>

		public bool Equals(IPropertyKey other)
		{
			if (other == null)
				return false;

			if (other is PropertyKey<TClass, TProperty> casted)
				return Info == casted.Info;

			return false;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj as IPropertyKey);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		#endregion IEquatable<IPropertyKey>

		#region IEqualityComparer<TClass>

		public bool Equals(TClass x, TClass y)
		{
			if (getter == null)
				return true;

			TProperty vx = GetValue(x);
			TProperty vy = GetValue(y);

			return EqualityComparer.Equals(vx, vy);
		}

		int IEqualityComparer<TClass>.GetHashCode(TClass value)
		{
			return value?.GetHashCode() ?? 0;
		}

		#endregion IEqualityComparer<TClass>

		#region Methods

		public override string ToString()
		{
			return $"PropertyKey<{ClassType.Name}, {PropertyType.Name}>[Name: \"{Name}\"]";
		}

		#endregion Methods
	}

	public sealed class PropertyKeyCollection : KeyedCollection<string, IPropertyKey>
	{
		public PropertyKeyCollection() { }
		public PropertyKeyCollection(PropertyKeyCollection collection) => AddRange(collection);

		protected override string GetKeyForItem(IPropertyKey item) => item?.Name;

		public void AddRange(IEnumerable<IPropertyKey> collection)
		{
			foreach (var key in collection)
				Add(key);
		}
	}

	public static class PropertyKey
	{
		private static readonly Type GenericKey = typeof(PropertyKey<,>);
		private static Dictionary<Type, PropertyKeyCollection> Cache = new Dictionary<Type, PropertyKeyCollection>();

		public static IPropertyKey CreateKey<TClass>(string name)
		{
			PropertyInfo info = typeof(TClass).GetProperty(name);
			return CreateKey(info);
		}
		public static IPropertyKey CreateKey(PropertyInfo info)
		{
			if (info == null)
				return null;

			Type keyType = ObjectActivator.AsGeneric(GenericKey, info.DeclaringType, info.PropertyType);
			return (IPropertyKey)Activator.CreateInstance(keyType, info);
		}

		private static PropertyKeyCollection EnsureCache(Type type)
		{
			if (Cache.TryGetValue(type, out var cached))
				return cached;

			cached = new PropertyKeyCollection();
			foreach (var info in type.GetProperties())
			{
				if (info.GetIndexParameters().Length > 0)
					continue;

				var key = CreateKey(info);
				cached.Add(key);
			}

			Cache[type] = cached;
			return cached;
		}

		public static PropertyKeyCollection GetKeys<T>() => GetKeys(typeof(T));
		public static IPropertyKey GetKey<T>(string name) => GetKey(typeof(T), name);
		public static PropertyKeyCollection GetKeys(Type type) => new PropertyKeyCollection(EnsureCache(type));
		public static IPropertyKey GetKey(Type type, string name) => EnsureCache(type).TryGetValue(name, out var key) ? key : null;
	}
}
