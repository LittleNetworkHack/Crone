namespace Crone
{
	public class CoreDataRecordJsonConverter : JsonConverter<CoreDataRecord>
	{
		private readonly Type recordType = typeof(CoreDataRecord);

		public override bool CanConvert(Type typeToConvert)
			=> recordType.IsAssignableFrom(typeToConvert);

		public override void Write(Utf8JsonWriter writer, CoreDataRecord value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			foreach (DictionaryEntry item in value.Properties)
			{
				writer.WritePropertyName((string)item.Key);
				JsonSerializer.Serialize(writer, item.Value, item.Value?.GetType(), options);
			}
			writer.WriteEndObject();
		}

		public override CoreDataRecord Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (!recordType.IsAssignableFrom(typeToConvert))
				throw new NotSupportedException();

			if (reader.TokenType != JsonTokenType.StartObject)
				throw new JsonException();

			var item = (CoreDataRecord)Activator.CreateInstance(typeToConvert);
			var properties = PropertyKey.GetKeys(typeToConvert);
			while (reader.Read())
			{
				if (reader.TokenType == JsonTokenType.EndObject)
					break;

				if (reader.TokenType != JsonTokenType.PropertyName)
					throw new JsonException();

				var propertyName = reader.GetString();
				if (string.IsNullOrWhiteSpace(propertyName))
					throw new JsonException();

				var flag = properties.TryGetValue(propertyName, out var key);
				var name = flag ? key.Name : propertyName;
				var type = flag ? key.PropertyType : typeof(string);
				var value = JsonSerializer.Deserialize(ref reader, type, options);
				item.SetValue(name, value);
			}

			return item;
		}
	}

    public static class CoreDataRecordExtensions
    {
		public static OrderedDictionary Copy(this OrderedDictionary dictionary)
		{
			var result = new OrderedDictionary(dictionary.Count);
			foreach (DictionaryEntry entry in dictionary)
				result.Add(entry.Key, entry.Value);
			return result;
		}

		public static OrderedDictionary GetProperties(this CoreDataRecord record)
		{
			return Copy(record.Properties);
		}

		public static TRecord LoadProperties<TRecord>(this TRecord record, OrderedDictionary dictionary)
			where TRecord : CoreDataRecord
		{
			record.Properties = Copy(dictionary);
			return record;
		}

		public static TRecord ConvertTo<TRecord>(this CoreDataRecord record)
			where TRecord : CoreDataRecord, new()
		{
			var instance = ObjectActivator.Create<TRecord>();
			instance.Properties = GetProperties(record);
			return instance;
		}

		public static string AsJson(this CoreDataRecord record)
		{
			return JsonSerializer.Serialize(record);
		}
	}
}
