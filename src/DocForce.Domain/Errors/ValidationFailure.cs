namespace DocForce.Domain.Errors;

public record class ValidationFailure(string Field, string Message);
