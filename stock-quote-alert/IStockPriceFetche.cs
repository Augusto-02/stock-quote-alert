namespace stock_quote_alert;

public interface IStockPriceFetche
{
    decimal GetPrice(string symbol);
}