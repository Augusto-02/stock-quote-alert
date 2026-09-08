namespace stock_quote_alert.Config;

public class SmtpSettings
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
}