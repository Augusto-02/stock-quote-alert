namespace stock_quote_alert;

public class StockPriceAlertService
{
    private readonly IStockPriceFetcher _priceFetcher;

    public StockPriceAlertService(IStockPriceFetcher priceFetcher)
    {
        this._priceFetcher = priceFetcher;
    }

    public async Task MonitorAlertsAsync(List<StockPriceAlert> requestList)
    {
        using PeriodicTimer timer =
            new(TimeSpan.FromSeconds(30));

        while (await timer.WaitForNextTickAsync())
        {
            foreach (var request in requestList)
            {
                decimal actualPrice = await this._priceFetcher.GetPriceAsync(request.Ticker);
                if (actualPrice <= request.BuyPriceReference)
                {
                    if (!request.AlreadySentAlert)
                    {
                        Console.WriteLine(
                            $"Time to buy price changed to {actualPrice} and the reference is {request.BuyPriceReference}");
                        request.MarkAlertSent(true);
                    }
                }
                else if (actualPrice >= request.SellPriceReference)
                {
                    if (!request.AlreadySentAlert)
                    {
                        Console.WriteLine(
                            $"Time to sell price changed to {actualPrice} and the reference is {request.SellPriceReference}");
                        request.MarkAlertSent(true);
                    }
                }

                else
                {
                    request.MarkAlertSent(false);
                }
            }
        }
    }
}