namespace stock_quote_alert.Config;
using System.Globalization;

public class EnvironmentSettingsLoader
{
    public static ApiConfig LoadApiSettings()
    {
        string priceApiUrl = GetRequired("PRICE_API_URL");
        string pollingIntervalSeconds = GetRequired("POLLING_INTERVAL_SECONDS");

        if (!int.TryParse(
                pollingIntervalSeconds,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int pollingInterval) ||
            pollingInterval <= 0)
        {
            throw new InvalidOperationException(
                "POLLING_INTERVAL_SECONDS has to be a positive integer.");
        }

        return new ApiConfig
        {
            PriceApiUrl = priceApiUrl,
            PollingIntervalSeconds = pollingInterval
                
        };
    }

    private static string GetRequired(string key)
    {
        string? value = Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"The environment variable {key} has not been set.");
        }

        return value;
    }
}
