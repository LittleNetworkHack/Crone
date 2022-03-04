namespace Crone
{
	[JsonConverter(typeof(CoreDataRecordJsonConverter))]
	public class CoreDataRecord : CoreComponent
	{
		#region Properties

		protected internal OrderedDictionary Properties { get; set; } = new OrderedDictionary();

		#endregion Properties

		#region Constructors

		public CoreDataRecord() => Properties = new OrderedDictionary();
		public CoreDataRecord(OrderedDictionary properties) => Properties = properties ?? new OrderedDictionary();

		#endregion Constructors

		#region Get/Set Core

		protected override string GetNameOverride(string name) => name?.ToUpperInvariant();

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
