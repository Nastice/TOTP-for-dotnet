using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;

namespace Nastice.GoogleAuthenticateLab.Shared.Extensions;

public static class FluentValidatorExtensions
{
    public static void WithDescription<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException("Expression must be a member expression");
        }

        var description = memberExpression.Member.GetCustomAttribute<DescriptionAttribute>();

        ruleBuilder.WithName(description?.Description ?? memberExpression.Member.Name);
    }
}
