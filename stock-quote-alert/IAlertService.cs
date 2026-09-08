namespace stock_quote_alert;

public interface IAlertService
{
    Task SendAlertAsync(
        string ticker,
        decimal actualPrice,
        string alertType);
}