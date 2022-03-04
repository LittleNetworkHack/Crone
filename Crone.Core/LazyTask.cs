using System.Threading;

namespace Crone
{
	public class LazyTask<T> : Lazy<Task<T>>
	{
		public LazyTask(Task<T> value) : base(value) { }
		public LazyTask(Func<Task<T>> valueFactory) : base(valueFactory) { }
	}
}
