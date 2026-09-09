namespace stock_quote_alert;

using stock_quote_alert.Config;
using System.Net.Http;
using System.Net.Http.Headers;

class Program
{
    static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load("Config/.env");
        AppConfig environmentVariables;
        try
        {
            environmentVariables = EnvironmentSettingsLoader.LoadSettings();
        }
        catch (InvalidOperationException error)
        {
            Console.WriteLine(error.Message);
            return;
        }

        string request = args.Length > 0
            ? string.Join(" ", args)
            : Console.ReadLine() ?? string.Empty;

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

        using HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(environmentVariables.Api.PriceApiUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", environmentVariables.Api.ApiToken);
        IStockPriceFetcher fetcher =
            new BrapiStockPriceFetcher(httpClient);
        IAlertService alertService = new EmailAlertService(environmentVariables.Smtp);
        StockPriceAlertService service =
            new StockPriceAlertService(fetcher, alertService, environmentVariables.Api.PollingIntervalSeconds);
        await service.MonitorAlertsAsync(requestData);
    }
}