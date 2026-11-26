namespace DocForce.Domain.Common;

public abstract class BaseError
{
    public string Code { get; }
    public string Description { get; }
    public ErrorType ErrorType { get; }
    public IReadOnlyDictionary<string, object?>? Metadata { get; protected init; }

    public BaseError(string code, string description, ErrorType errorType)
    {
        Code = code;
        Description = description;
        ErrorType = errorType;
    }

    public override string ToString()
    {
        return $"{Code}: {Description}";
    }
}
