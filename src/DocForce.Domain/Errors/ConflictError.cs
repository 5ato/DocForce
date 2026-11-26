using DocForce.Domain.Common;

namespace DocForce.Domain.Errors;

public class ConflictError(string code, string description) : BaseError(code, description, ErrorType.Conflict);
