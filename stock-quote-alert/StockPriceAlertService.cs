namespace stock_quote_alert;

public class StockPriceAlertService
{
    private readonly IStockPriceFetcher _priceFetcher;
    private readonly int _pollingIntervalSeconds;
    private readonly IAlertService _alertService;

    public StockPriceAlertService(IStockPriceFetcher priceFetcher, IAlertService alertService,
        int pollingIntervalSeconds)
    {
        _priceFetcher = priceFetcher;
        _pollingIntervalSeconds = pollingIntervalSeconds;
        _alertService = alertService;
    }

    internal async Task CheckAlertAsync(List<StockPriceAlert> alerts)
    {
        foreach (var alert in alerts)
        {
            decimal actualPrice;
            try
            {
                actualPrice =
                    await _priceFetcher.GetPriceAsync(alert.Ticker);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }

            if (actualPrice <= alert.BuyPriceReference)
            {
                if (!alert.AlreadySentAlert)
                {
                    await SendAlertAndMarkAsync(alert, actualPrice, "BUY");
                }
            }
            else if (actualPrice >= alert.SellPriceReference)
            {
                if (!alert.AlreadySentAlert)
                {
                    await SendAlertAndMarkAsync(alert, actualPrice, "SELL");
                }
            }

            else
            {
                alert.ResetAlert();
            }
        }
    }

    private async Task SendAlertAndMarkAsync(StockPriceAlert alert, decimal actualPrice, string alertType)
    {
        try
        {
            await _alertService.SendAlertAsync(alert.Ticker, actualPrice, alertType);
            alert.MarkAlertAsSent();
        }
        catch (Exception error)
        {
            Console.WriteLine(
                $"Failed to send the {alertType} alert for " +
                $"ticker {alert.Ticker}: {error.Message}");
        }
    }

    public async Task MonitorAlertsAsync(List<StockPriceAlert> alerts)
    {
        using PeriodicTimer timer =
            new(TimeSpan.FromSeconds(_pollingIntervalSeconds));

        await CheckAlertAsync(alerts);

        while (await timer.WaitForNextTickAsync())
        {
            await CheckAlertAsync(alerts);
        }
    }
}
