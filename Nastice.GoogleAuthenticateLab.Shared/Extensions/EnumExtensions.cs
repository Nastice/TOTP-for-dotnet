using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Nastice.GoogleAuthenticateLab.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var result = enumValue.GetAttribute<DisplayAttribute>();

        return result?.Name ?? enumValue.ToString();
    }

    private static TAttribute? GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        var type = value.GetType().GetMember(value.ToString())[0].GetCustomAttribute<TAttribute>();
        return type;
    }
}
