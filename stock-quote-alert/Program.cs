namespace stock_quote_alert;

using stock_quote_alert.Config;
using System.Net.Http;
using System.Net.Http.Headers;

class Program
{
    static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load("Config/.env");
        ApiConfig environmentVariables;
        try
        {
            environmentVariables = EnvironmentSettingsLoader.LoadApiSettings();
        }
        catch (InvalidOperationException error)
        {
            Console.WriteLine(error.Message);
            return;
        }

        string request = Console.ReadLine();
        List<StockPriceAlert> requestData = new List<StockPriceAlert>();
        try
        {
            requestData = StockPriceAlertParser.Validate(request);
        }
        catch (ArgumentException error)
        {
            Console.WriteLine(error.Message);
            return;
        }

        HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(environmentVariables.PriceApiUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", environmentVariables.ApiToken);
        IStockPriceFetcher fetcher =
            new BrapiStockPriceFetcher(httpClient);
        StockPriceAlertService service =
            new StockPriceAlertService(fetcher, environmentVariables.PollingIntervalSeconds);
        await service.MonitorAlertsAsync(requestData);
    }
}