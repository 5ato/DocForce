using DocForce.Domain.Common;

namespace DocForce.Domain.Errors;

public class NotFoundError(string code, string description) : BaseError(code, description, ErrorType.NotFound);
