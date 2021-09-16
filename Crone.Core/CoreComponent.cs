using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public abstract class CoreComponent : IComponent, IDisposable
	{
		#region Properties

		public object this[int index]
		{
			get => GetValue(index);
			set => SetValue(index, value);
		}
		public object this[string name]
		{
			get => GetValue(name);
			set => SetValue(name, value);
		}

		#endregion Properties

		#region Get/Set Core

		protected virtual string GetNameOverride(string name) => name;

		protected virtual object GetValueDefault(int index) => null;
		protected virtual void SetValueDefault(int index, object value) { }

		protected virtual object GetValueDefault(string name) => null;
		protected virtual void SetValueDefault(string name, object value) { }

		protected abstract bool GetValueCore(int index, out object value);
		protected abstract bool SetValueCore(int index, object value);

		protected abstract bool GetValueCore(string name, out object value);
		protected abstract bool SetValueCore(string name, object value);

		#endregion Get/Set Core

		#region Get/Set Value

		public T GetValue<T>(int index) => ValueConverter.ConvertTo<T>(GetValue(index));
		public T GetValue<T>(string name) => ValueConverter.ConvertTo<T>(GetValue(name));

		public object GetValue(int index)
		{
			if (GetValueCore(index, out var value))
				return value;

			return GetValueDefault(index);
		}
		public void SetValue(int index, object value)
		{
			if (SetValueCore(index, value))
				return;

			SetValueDefault(index, value);
		}

		public object GetValue(string name)
		{
			name = GetNameOverride(name);
			if (GetValueCore(name, out var value))
				return value;

			return GetValueDefault(name);
		}
		public void SetValue(string name, object value)
		{
			name = GetNameOverride(name);
			if (SetValueCore(name, value))
				return;

			SetValueDefault(name, value);
		}

		protected T GetProperty<T>([CallerMemberName] string name = null)
		{
			var value = GetValue(name);
			return ValueConverter.ConvertTo<T>(value);
		}
		protected void SetProperty<T>(object value, [CallerMemberName] string name = null)
		{
			value = ValueConverter.ConvertTo<T>(value);
			SetValue(name, value);
		}

		#endregion Get/Set Value

		#region IComponent/IDisposable

		public ISite Site { get; set; }
		public IContainer Container => Site?.Container;

		public event EventHandler Disposed;

		~CoreComponent() => Dispose(false);
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			lock (this)
			{
				Site?.Container?.Remove(this);
				this.InvokeEmpty(Disposed);
			}
		}
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion IComponent/IDisposable
	}
}
