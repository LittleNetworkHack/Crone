using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crone
{
	public class CoreDataRecord : CoreComponent
	{
		#region Properties

		protected OrderedDictionary Properties { get; set; } = new OrderedDictionary();

		#endregion Properties

		#region Constructors

		public CoreDataRecord() => Properties = new OrderedDictionary();
		public CoreDataRecord(OrderedDictionary properties) => Properties = properties ?? new OrderedDictionary();

		#endregion Constructors

		#region Get/Set Core

		protected override bool GetValueCore(int index, out object value)
		{
			value = null;
			if (Properties == null)
				return false;

			value = Properties.Count.IndexInRange(index) ? Properties[index] : null;
			return true;
		}
		protected override bool SetValueCore(int index, object value)
		{
			if (Properties == null)
				return false;

			if (!Properties.Count.IndexInRange(index))
				return false;

			Properties[index] = value;
			return true;
		}

		protected override bool GetValueCore(string name, out object value)
		{
			value = null;
			if (Properties == null)
				return false;

			value = Properties.Contains(name) ? Properties[name] : null;
			return true;
		}
		protected override bool SetValueCore(string name, object value)
		{
			if (Properties == null)
				return false;

			Properties[name] = value;
			return true;
		}

		#endregion Get/Set Core
	}
}
