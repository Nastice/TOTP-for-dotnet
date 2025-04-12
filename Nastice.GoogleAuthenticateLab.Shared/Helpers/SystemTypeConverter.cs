using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nastice.GoogleAuthenticateLab.Shared.Helpers;

public class SystemTypeConverter : JsonConverter<Type>
{
    public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Type.GetType(reader.GetString() ?? string.Empty);
    }

    public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.FullName);
    }
}
