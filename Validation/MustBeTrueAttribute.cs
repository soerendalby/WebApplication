using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication.Validation;

/// <summary>
/// Data annotation to require a checkbox (bool) to be true.
/// Avoids using Range on bool, which can misbehave with client validators.
/// </summary>
public sealed class MustBeTrueAttribute : ValidationAttribute, IClientModelValidator
{
    public MustBeTrueAttribute()
    {
        ErrorMessage = ErrorMessage ?? "This field must be checked.";
    }

    public override bool IsValid(object? value)
    {
        return value is bool b && b;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-mustbetrue", ErrorMessage ?? "This field must be checked.");
    }

    private static bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (attributes.ContainsKey(key)) return false;
        attributes.Add(key, value);
        return true;
    }
}
