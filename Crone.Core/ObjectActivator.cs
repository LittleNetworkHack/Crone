using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public interface IObjectActivator
	{
		object DefaultValue { get; }
		Type TargetType { get; }

		object InvokeConstructor();
	}

	public interface IObjectActivator<T>
	{
		T DefaultValue { get; }
		T InvokeConstructor();
	}

	public sealed class ObjectActivator<T> : IObjectActivator<T>, IObjectActivator
	{
		private Func<T> TargetCtor;

		public T DefaultValue { get; } = default(T);
		public Type TargetType { get; } = typeof(T);

		public ObjectActivator()
		{
			ConstructorInfo info = TargetType.GetConstructor(Type.EmptyTypes);
			if (info != null)
			{
				NewExpression expr = Expression.New(info);
				TargetCtor = Expression.Lambda<Func<T>>(expr).Compile();
			}
		}

		public T InvokeConstructor() => TargetCtor != null ? TargetCtor() : DefaultValue;

		object IObjectActivator.DefaultValue => DefaultValue;
		object IObjectActivator.InvokeConstructor() => InvokeConstructor();
	}

	public static class ObjectActivator
	{
		#region Helpers

		public static Type AsGeneric(Type type, params Type[] typeArguments)
		{
			return typeArguments?.Length > 0 ? type.MakeGenericType(typeArguments) : type;
		}

		public static object GetDefaultOrNull(Type type) => EnsureCache(type).DefaultValue;
		public static object EnsureTypeSafety(this object value, Type type)
		{
			if (value.IsNullOrDBNull())
				return GetDefaultOrNull(type);

			if (type.IsAssignableFrom(value.GetType()))
				return value;

			return GetDefaultOrNull(type);
		}

		#endregion Helpers

		#region Cache

		private static Dictionary<Type, IObjectActivator> Cache = new Dictionary<Type, IObjectActivator>();

		private static IObjectActivator<T> EnsureCache<T>() => (IObjectActivator<T>)EnsureCache(typeof(T));
		private static IObjectActivator EnsureCache(Type type)
		{
			if (Cache.TryGetValue(type, out var creator))
				return creator;

			creator = (IObjectActivator)CreateDirty(AsGeneric(typeof(ObjectActivator<>), type));
			Cache[type] = creator;
			return creator;
		}

		#endregion Cache

		#region Create

		public static TCast CreateDirtyAs<TCast>(Type type, params Type[] typeArguments) => (TCast)CreateDirty(type, typeArguments);
		public static object CreateDirty(Type type, params Type[] typeArguments)
		{
			type = AsGeneric(type, typeArguments);
			return Activator.CreateInstance(type);
		}

		public static TCast CreateAs<TCast>(Type type, params Type[] typeArguments) => (TCast)Create(type, typeArguments);
		public static T Create<T>() => EnsureCache<T>().InvokeConstructor();
		public static object Create(Type type, params Type[] typeArguments)
		{
			type = AsGeneric(type, typeArguments);
			return EnsureCache(type).InvokeConstructor();
		}

		#endregion Create
	}
}
