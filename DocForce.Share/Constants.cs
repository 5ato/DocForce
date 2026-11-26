namespace DocForce.Share;

public static class Constants
{
    public const string DocPostgresConnectionString = "DocPostgresConnectionString";

    #region User
    public const int UsernameMaxLength = 64;
    public const int PasswordMaxLength = 20;
    #endregion

    #region Outbox Constants
    public const int EntityTypeMaxLength = 255;
    public const int EntityIdMaxLength = 255;
    public const int EventTypeMaxLength = 255;
    public const int StatusMaxLength = 32;
    public const int MaxOutboxRetries = 5;
    public const int OutboxBatchSize = 50;
    public const int OutboxProcessingDelayMs = 10_000;
    #endregion
}
