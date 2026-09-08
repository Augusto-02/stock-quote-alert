namespace stock_quote_alert;

using System.Net;

public class BrapiStockPriceFetcher : IStockPriceFetcher
{
    private readonly HttpClient httpClient;
    private const int MaxRetries = 3;

    public BrapiStockPriceFetcher(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<decimal> GetPriceAsync(string symbol)
    {
        for (int index = 1; index <= MaxRetries; index++)
        {
            try
            {
                using HttpResponseMessage response = await httpClient.GetAsync(symbol);
                int responseStatusCode = (int)response.StatusCode;
                bool shouldRetry = responseStatusCode >= 500 || response.StatusCode == HttpStatusCode.TooManyRequests;
                if (response.IsSuccessStatusCode)
                {
                    return 10;
                }
                else if (shouldRetry && index < MaxRetries)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(index));
                    continue;
                }
                else if (shouldRetry && index == MaxRetries)
                {
                    throw new HttpRequestException($"\"All retries failed while getting ticker {symbol}.");
                }
                else if (!shouldRetry)
                {
                    throw new InvalidOperationException(
                        $"API ERROR on getting ticker: {symbol} status code {response.StatusCode}: {response.ReasonPhrase ?? "Not informed"}.");
                }
            }
            catch (HttpRequestException error)
            {
                if (index == MaxRetries)
                {
                    throw new HttpRequestException(
                        $"All retries failed while getting ticker: {symbol}.",
                        error);
                }
                await Task.Delay(
                    TimeSpan.FromSeconds(index));
            }
            catch (TaskCanceledException error)
            {
                if (index == MaxRetries)
                {
                    throw new HttpRequestException(
                        $"All retries failed while getting ticker: {symbol}.",
                        error);
                }
                await Task.Delay(TimeSpan.FromSeconds(index));
            }
        }


        throw new HttpRequestException($"\"All retries failed while getting ticker {symbol}.");
    }
}

