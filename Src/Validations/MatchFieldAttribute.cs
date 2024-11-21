using System.ComponentModel.DataAnnotations;

namespace dating_course_api.Src.Validations
{
    public class MatchFieldAttribute(string comparisonProperty) : ValidationAttribute
    {
        private readonly string _comparisonProperty = comparisonProperty;

        protected override ValidationResult? IsValid(
            object? value,
            ValidationContext validationContext
        )
        {
            var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (comparisonProperty == null)
                return new ValidationResult($"Property '{_comparisonProperty}' not found.");

            var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance);

            if (!Equals(value, comparisonValue))
                return new ValidationResult(
                    ErrorMessage ?? $"The field must match {_comparisonProperty}."
                );

            return ValidationResult.Success;
        }
    }
}
