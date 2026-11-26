using DocForce.Domain.Common;

namespace DocForce.Domain.Errors;

public class BadRequestError(string code, string description) : BaseError(code, description, ErrorType.BadRequest);
