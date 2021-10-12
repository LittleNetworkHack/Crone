using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone.Demo.Database
{
	public class ExamplePersonRecord : CoreDataRecord
	{
		public string FirstName
		{
			get => GetProperty<string>();
			set => SetProperty<string>(value);
		}

		public ExamplePersonRecord() : base() { }
		public ExamplePersonRecord(OrderedDictionary properties) : base(properties) { }
	}
}
