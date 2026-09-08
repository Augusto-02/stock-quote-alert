namespace stock_quote_alert.Config;

using System.Globalization;

public static class EnvironmentSettingsLoader
{
    public static AppConfig LoadSettings()
    {
        string priceApiUrl = GetRequired("PRICE_API_URL");
        string pollingIntervalSeconds =
            GetRequired("POLLING_INTERVAL_SECONDS");
        string apiToken = GetRequired("API_TOKEN");

        string smtpHost = GetRequired("SMTP_HOST");
        string smtpPort = GetRequired("SMTP_PORT");
        string smtpUsername = GetRequired("SMTP_USERNAME");
        string smtpPassword = GetRequired("SMTP_PASSWORD");
        string smtpFrom = GetRequired("SMTP_FROM");
        string smtpTo = GetRequired("SMTP_TO");

        int pollingInterval =
            ParsePositiveInt(
                pollingIntervalSeconds,
                "POLLING_INTERVAL_SECONDS");

        int port =
            ParsePositiveInt(
                smtpPort,
                "SMTP_PORT");

        ApiConfig apiConfig = new ApiConfig
        {
            PriceApiUrl = priceApiUrl,
            PollingIntervalSeconds = pollingInterval,
            ApiToken = apiToken
        };

        SmtpSettings smtpSettings = new SmtpSettings
        {
            Host = smtpHost,
            Port = port,
            Username = smtpUsername,
            Password = smtpPassword,
            From = smtpFrom,
            To = smtpTo
        };

        return new AppConfig
        {
            Api = apiConfig,
            Smtp = smtpSettings
        };
    }

    private static string GetRequired(string key)
    {
        string? value =
            Environment.GetEnvironmentVariable(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"The environment variable {key} has not been set.");
        }

        return value;
    }

    private static int ParsePositiveInt(
        string value,
        string variableName)
    {
        if (!int.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out int parsedValue) ||
            parsedValue <= 0)
        {
            throw new InvalidOperationException(
                $"{variableName} has to be a positive integer.");
        }

        return parsedValue;
    }
}