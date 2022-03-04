namespace Crone
{
	public abstract class CoreComponent : IDisposable
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

		protected abstract bool GetValueCore(int index, out object value);
		protected abstract bool SetValueCore(int index, object value);

		protected abstract bool GetValueCore(string name, out object value);
		protected abstract bool SetValueCore(string name, object value);

		#endregion Get/Set Core

		#region Get/Set Default

		protected virtual object GetValueDefault(int index) => null;
		protected virtual void SetValueDefault(int index, object value) { }

		protected virtual object GetValueDefault(string name) => null;
		protected virtual void SetValueDefault(string name, object value) { }

		#endregion Get/Set Default

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

		#endregion Get/Set Value

		#region Get/Set Property

		protected virtual T GetProperty<T>([CallerMemberName] string name = null)
		{
			var value = GetValue(name);
			return ValueConverter.ConvertTo<T>(value);
		}
		protected virtual void SetProperty<T>(object value, [CallerMemberName] string name = null)
		{
			value = ValueConverter.ConvertTo<T>(value);
			SetValue(name, value);
		}

		#endregion Get/Set Property

		#region Events

		protected void InvokeEmpty(EventHandler handler) => handler?.Invoke(this, EventArgs.Empty);

		#endregion Events

		#region IDisposable

		public event EventHandler Disposed;

		~CoreComponent() => Dispose(false);
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
				return;

			lock (this)
			{
				InvokeEmpty(Disposed);
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
