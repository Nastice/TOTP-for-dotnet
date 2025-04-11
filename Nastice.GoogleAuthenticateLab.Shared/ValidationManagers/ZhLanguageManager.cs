using FluentValidation.Resources;

namespace Nastice.GoogleAuthenticateLab.Shared.ValidationManagers;

public class ZhLanguageManager : LanguageManager
{
    public ZhLanguageManager()
    {
        var culture = "zh-TW";
        AddTranslation(culture, "EmailValidator", "{PropertyName}不是有效的信箱格式。");
        AddTranslation(culture, "GreaterThanOrEqualValidator", "{PropertyName}必須大於等於 {ComparisonValue}。");
        AddTranslation(culture, "GreaterThanValidator", "{PropertyName}必須大於 {ComparisonValue}。");
        AddTranslation(culture, "LengthValidator", "{PropertyName}的長度必須在 {MinLength} 到 {MaxLength} 個文字，您輸入了 {TotalLength} 個文字。");
        AddTranslation(culture, "MinimumLengthValidator", "{PropertyName}必須大於或等於 {MinLength} 個文字。您輸入了 {TotalLength} 個字符。");
        AddTranslation(culture, "MaximumLengthValidator", "{PropertyName}必須小於或等於 {MaxLength} 個文字。您輸入了 {TotalLength} 個字符。");
        AddTranslation(culture, "LessThanOrEqualValidator", "{PropertyName}必須小於或等於 {ComparisonValue}。");
        AddTranslation(culture, "LessThanValidator", "{PropertyName}必須小於 {ComparisonValue}。");
        AddTranslation(culture, "NotEmptyValidator", "{PropertyName}不能為空。");
        AddTranslation(culture, "NotEqualValidator", "{PropertyName}的值不能和 {ComparisonValue} 相等。");
        AddTranslation(culture, "NotNullValidator", "{PropertyName}不能為 null。");
        AddTranslation(culture, "PredicateValidator", "{PropertyName}不符合指定的條件。");
        AddTranslation(culture, "AsyncPredicateValidator", "{PropertyName}不符合指定的條件。");
        AddTranslation(culture, "RegularExpressionValidator", "{PropertyName}的格式不正確。");
        AddTranslation(culture, "EqualValidator", "{PropertyName}的資料應該和 {ComparisonValue} 相等。");
        AddTranslation(culture, "ExactLengthValidator", "{PropertyName}必須是 {MaxLength} 個字符，您輸入了 {TotalLength} 字符。");
        AddTranslation(culture, "InclusiveBetweenValidator", "{PropertyName}必須在 {From} (包含)和 {To} (包含)之間， 您輸入了 {PropertyValue}。");
        AddTranslation(culture, "ExclusiveBetweenValidator", "{PropertyName}必須在 {From} (不包含)和 {To} (不包含)之間， 您輸入了 {PropertyValue}。");
        AddTranslation(culture, "CreditCardValidator", "{PropertyName}不是有效的信用卡號碼。");
        AddTranslation(culture,
                       "ScalePrecisionValidator",
                       "{PropertyName}總位數不能超過 {ExpectedPrecision} 位，其中小數部份 {ExpectedScale} 位。您共計輸入了 {Digits} 位數字，其中小數部份 {ActualScale} 位。");
        AddTranslation(culture, "EmptyValidator", "{PropertyName}必須為空字串。");
        AddTranslation(culture, "NullValidator", "{PropertyName}必須為 null。");
    }
}
