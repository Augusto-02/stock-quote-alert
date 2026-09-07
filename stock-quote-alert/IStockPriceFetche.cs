namespace stock_quote_alert;

public interface IStockPriceFetcher
{
    Task<decimal> GetPriceAsync(string symbol);
}