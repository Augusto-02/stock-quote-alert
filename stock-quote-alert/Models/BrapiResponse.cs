namespace stock_quote_alert.Models;

public class BrapiResponse
{
    public List<BrapiResult> Results { get; set; } = new();
}