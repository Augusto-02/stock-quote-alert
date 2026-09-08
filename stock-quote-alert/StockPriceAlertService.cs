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

    private async Task CheckAlertAsync(List<StockPriceAlert> alerts)
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
                    Console.WriteLine(
                        $"Time to buy price changed to {actualPrice} and the reference is {alert.BuyPriceReference}");
                    alert.MarkAlertAsSent();
                    await _alertService.SendAlertAsync(alert.Ticker, actualPrice, "BUY");
                }
            }
            else if (actualPrice >= alert.SellPriceReference)
            {
                if (!alert.AlreadySentAlert)
                {
                    Console.WriteLine(
                        $"Time to sell price changed to {actualPrice} and the reference is {alert.SellPriceReference}");
                    alert.MarkAlertAsSent();
                    await _alertService.SendAlertAsync(alert.Ticker, actualPrice, "SELL");
                }
            }

            else
            {
                alert.ResetAlert();
            }
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