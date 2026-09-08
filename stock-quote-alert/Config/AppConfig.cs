namespace stock_quote_alert.Config;

public class AppConfig
{
    public required ApiConfig Api { get; init; }
    public required SmtpSettings Smtp { get; init; }
}