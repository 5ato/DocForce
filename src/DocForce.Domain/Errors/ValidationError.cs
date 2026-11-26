using DocForce.Domain.Common;

namespace DocForce.Domain.Errors;

public class ValidationError : BaseError
{
    public IReadOnlyList<ValidationFailure> Failures { get; }

    public ValidationError(IReadOnlyList<ValidationFailure> failures)
        : base("Validation.Failed", "One or more validation errors occurred", ErrorType.Validation)
    {
        Failures = failures;
        Metadata = new Dictionary<string, object?>
        {
            ["errors"] = failures
                .GroupBy(f => f.Field)
                .ToDictionary(g => g.Key, g => (object?)g.Select(f => f.Message).ToArray())
        };
    }
}
