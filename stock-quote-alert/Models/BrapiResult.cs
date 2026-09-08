namespace stock_quote_alert.Models;

public class BrapiResult
{
    public string Symbol { get; set; } = string.Empty;
    public BrapiQuoteData? Data { get; set; }
}