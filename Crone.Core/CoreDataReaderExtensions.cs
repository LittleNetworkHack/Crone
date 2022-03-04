namespace Crone
{
	public static class CoreDataReaderExtensions
	{
		public static IEnumerable<TRecord> AsEnumerable<TRecord>(this CoreDataReader reader)
			where TRecord : CoreDataRecord, new()
		{
			while (reader.Read())
			{
				var record = GetRecord<TRecord>(reader);
				yield return record;
			}
		}

		public static OrderedDictionary GetDictionary(this CoreDataReader reader)
		{
			var record = new OrderedDictionary(reader.Count);
			for (int i = 0; i < reader.Count; i++)
			{
				var name = reader.Names[i];
				if (string.IsNullOrEmpty(name))
					continue;

				record.Add(name, reader.GetValue(i));
			}
			return record;
		}

		public static CoreDataRecord GetRecord(this CoreDataReader reader)
		{
			var values = GetDictionary(reader);
			return new CoreDataRecord(values);
		}

		public static TRecord GetRecord<TRecord>(this CoreDataReader reader)
			where TRecord : CoreDataRecord, new()
		{
			var record = ObjectActivator.Create<TRecord>();
			var values = GetDictionary(reader);
			return record.LoadProperties(values);
		}

		public static TRecord GetRecord<TRecord>(this CoreDataReader reader, Func<OrderedDictionary, TRecord> constructor)
			where TRecord : CoreDataRecord
		{
			var values = GetDictionary(reader);
			return constructor(values);
		}

		public static List<OrderedDictionary> GetDictionaryList(this CoreDataReader reader)
		{
			var list = new List<OrderedDictionary>();
			while (reader.Read())
			{
				var record = GetDictionary(reader);
				list.Add(record);
			}
			return list;
		}

		public static List<CoreDataRecord> GetRecordList(this CoreDataReader reader)
		{
			var list = new List<CoreDataRecord>();
			while (reader.Read())
			{
				var record = GetRecord(reader);
				list.Add(record);
			}	
			return list;
		}

		public static List<TRecord> GetRecordList<TRecord>(this CoreDataReader reader)
			where TRecord : CoreDataRecord, new()
		{
			var list = new List<TRecord>();
			while (reader.Read())
			{
				var record = GetRecord<TRecord>(reader);
				list.Add(record);
			}
			return list;
		}

		public static List<TRecord> GetRecordList<TRecord>(this CoreDataReader reader, Func<OrderedDictionary, TRecord> constructor)
			where TRecord : CoreDataRecord
		{
			var list = new List<TRecord>();
			while (reader.Read())
			{
				var record = GetRecord(reader, constructor);
				list.Add(record);
			}
			return list;
		}
	}
}
