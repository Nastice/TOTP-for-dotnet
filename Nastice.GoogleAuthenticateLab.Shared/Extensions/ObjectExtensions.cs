using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Nastice.GoogleAuthenticateLab.Shared.Extensions;

public static class ObjectExtensions
{
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
}
