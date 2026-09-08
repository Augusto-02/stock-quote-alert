namespace stock_quote_alert.Config;


public class ApiConfig
{
    public required string PriceApiUrl { get; init; }
    public required int PollingIntervalSeconds { get; init; } = 30;
    
    public required string ApiToken { get; init; }
}