using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nastice.GoogleAuthenticateLab.Shared.Helpers;
using QRCoder;

namespace Nastice.GoogleAuthenticateLab.Shared.Extensions;

public static class ObjectExtensions
{
    private static readonly ConcurrentDictionary<JavaScriptEncoder, JsonSerializerOptions> OptionsCache = new();
    public static string GetDisplayName(this object obj)
    {
        var result = obj.getAttribute<DisplayAttribute>();

        return result?.Name ?? nameof(obj);
    }

    /// <summary>
    /// 取得物件屬性
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="TAttribute"></typeparam>
    /// <returns></returns>
    private static TAttribute? getAttribute<TAttribute>(this object obj) where TAttribute : Attribute
    {
        var result = obj.GetType().GetCustomAttribute<TAttribute>(true);

        return result;
    }

    public static string ToJson(this object obj, JavaScriptEncoder? encoder = null)
    {
        var encoderToUse = encoder ?? JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        var options = OptionsCache.GetOrAdd(encoderToUse, enc => new JsonSerializerOptions
        {
            Encoder = enc,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.Preserve,
            Converters = { new SystemTypeConverter() }
        });

        return JsonSerializer.Serialize(obj, options);
    }
}
