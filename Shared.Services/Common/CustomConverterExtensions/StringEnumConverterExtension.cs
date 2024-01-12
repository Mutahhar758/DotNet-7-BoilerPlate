using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using Demo.WebApi.Shared.Services.Common.Extensions;

namespace Demo.WebApi.Shared.Services.Common.CustomConverterExtensions;
public class StringEnumConverterExtension : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum; // Add anything additional here such as typeToConvert.IsEnumWithDescription() to check for description attributes.

    public override System.Text.Json.Serialization.JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        (System.Text.Json.Serialization.JsonConverter)Activator.CreateInstance(typeof(CustomStringEnumConverter<>).MakeGenericType(typeToConvert))!;
}

public class CustomStringEnumConverter<T> : System.Text.Json.Serialization.JsonConverter<T>
        where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetInt32()!.GetEnumValue<T>()!;

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WriteRawValue(value.GetObject());
}

public class CustomStringToObjectEnumConverter : Newtonsoft.Json.JsonConverter
{
    Type _objectType;
    public override bool CanConvert(Type objectType)
    {
        _objectType = objectType;
        Type u = Nullable.GetUnderlyingType(objectType);
        return ((u != null) && u.IsEnum) || objectType.IsEnum;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Integer)
        {
            return (int?)reader.Value;
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
    {
        string? lookupResponse = ((Enum?)value).GetObject();
        writer.WriteRawValue(lookupResponse);
    }
}