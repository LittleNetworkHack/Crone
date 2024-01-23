//namespace Crone;

//public class CoreObjectConverter : JsonConverterFactory
//{
//    public override bool CanConvert(Type typeToConvert) => typeof(ICoreObject).IsAssignableFrom(typeToConvert);

//    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
//    {
//        var converterType = typeof(CoreObjectConverter<>).MakeGenericType(typeToConvert);
//        var instance = CoreLib.Create<JsonConverter>(converterType);
//        return instance;
//    }
//}

//public class CoreObjectConverter<T> : JsonConverter<T>
//    where T : ICoreObject
//{
//    public override bool CanConvert(Type typeToConvert) => typeof(ICoreObject).IsAssignableFrom(typeToConvert);

//    public override void Write(Utf8JsonWriter writer, T item, JsonSerializerOptions options)
//    {
//        writer.WriteStartObject();
//        foreach (var (key, value) in item)
//        {
//            writer.WritePropertyName(key);
//            var type = value?.GetType() ?? typeof(object);
//            JsonSerializer.Serialize(writer, value, type, options);
//        }
//        writer.WriteEndObject();
//    }
//    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        if (reader.TokenType != JsonTokenType.StartObject)
//            throw new JsonException();

//        var item = CoreLib.Create<T>(typeToConvert);
//        var properties = CoreLib.GetPropertiesMap(typeToConvert);
//        while (reader.Read())
//        {
//            if (reader.TokenType == JsonTokenType.EndObject)
//                break;

//            if (reader.TokenType != JsonTokenType.PropertyName)
//                throw new JsonException();

//            var propertyName = reader.GetString();
//            if (string.IsNullOrWhiteSpace(propertyName))
//                throw new JsonException();

//            var flag = properties.TryGetValue(propertyName, out var key);
//            var name = flag ? key.Name : propertyName;
//            var type = flag ? key.PropertyType : typeof(string);
//            var value = JsonSerializer.Deserialize(ref reader, type, options);
//            item[name] = value;
//        }
//        return item;
//    }
//}
