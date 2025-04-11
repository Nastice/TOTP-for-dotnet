using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;

namespace Nastice.GoogleAuthenticateLab.Shared.Extensions;

public static class FluentValidatorExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithDisplayName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException("Expression must be a member expression");
        }

        var displayName = memberExpression.Member.GetCustomAttribute<DisplayAttribute>();

        return ruleBuilder.WithName(displayName?.Name ?? memberExpression.Member.Name);
    }
}
