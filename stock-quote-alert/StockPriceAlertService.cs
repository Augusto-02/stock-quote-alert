namespace stock_quote_alert;

public class StockPriceAlertService
{
    private readonly IStockPriceFetcher _priceFetcher;
    private readonly int _pollingIntervalSeconds;

    public StockPriceAlertService(IStockPriceFetcher priceFetcher, int  pollingIntervalSeconds)
    {
        this._priceFetcher = priceFetcher;
        this._pollingIntervalSeconds = pollingIntervalSeconds;
    }

    private async Task CheckAlertAsync(List<StockPriceAlert> requestList)
    {
        foreach (var request in requestList)
        {
            decimal actualPrice =
                await _priceFetcher.GetPriceAsync(request.Ticker);
            if (actualPrice <= request.BuyPriceReference)
            {
                if (!request.AlreadySentAlert)
                {
                    Console.WriteLine(
                        $"Time to buy price changed to {actualPrice} and the reference is {request.BuyPriceReference}");
                    request.MarkAlertSent(true);
                }

                Console.WriteLine("BUY");
            }
            else if (actualPrice >= request.SellPriceReference)
            {
                if (!request.AlreadySentAlert)
                {
                    Console.WriteLine(
                        $"Time to sell price changed to {actualPrice} and the reference is {request.SellPriceReference}");
                    request.MarkAlertSent(true);
                }

                Console.WriteLine("SELL");
            }

            else
            {
                request.MarkAlertSent(false);
            }
        }
    }

    public async Task MonitorAlertsAsync(List<StockPriceAlert> requestList)
    {
        using PeriodicTimer timer =
            new(TimeSpan.FromSeconds(_pollingIntervalSeconds));

        await CheckAlertAsync(requestList);
        
        while (await timer.WaitForNextTickAsync())
        {
            await CheckAlertAsync(requestList);
        }
    }
}