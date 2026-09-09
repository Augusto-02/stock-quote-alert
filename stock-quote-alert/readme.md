# Stock Quote Alert

Stock Quote Alert is a .NET 8 console application that monitors stock prices and notifies the user when a configured buy or sell reference price is reached. It uses short polling to periodically fetch Brazilian stock quotes from the [brapi.dev](https://brapi.dev/) API, compare them with the configured references, and send an email alert when a condition is met.

## How to use

### Prerequisites

- .NET 8 SDK
- A brapi.dev API token
- An SMTP account for sending email notifications

### Configuration

1. Copy `stock-quote-alert/Config/.env.example` to `stock-quote-alert/Config/.env`.
2. Fill in the required environment variables.
3. Run the application from the project directory:

```bash
dotnet run
```

4. Enter one or more stock alerts in the following format:

```text
TICKER SELL_PRICE BUY_PRICE
```

For multiple stocks, append additional groups of three values:

```text
PETR4 40.00 35.00 VALE3 75.00 65.00
```

The application validates the complete input before starting the monitoring process. If any value is invalid, the application reports all validation errors and does not start polling.

The first price check runs immediately. Subsequent checks run according to `POLLING_INTERVAL_SECONDS`.

## Environment variables

The application loads variables from `Config/.env` using `DotNetEnv`. Do not commit the real `.env` file or expose its secrets.

```env
PRICE_API_URL=https://brapi.dev/
POLLING_INTERVAL_SECONDS=30
API_TOKEN=your-brapi-token

SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-google-app-password
SMTP_FROM=your-email@gmail.com
SMTP_TO=recipient@gmail.com
```

### Variable descriptions

| Variable | Description |
| --- | --- |
| `PRICE_API_URL` | Base URL used to request stock quotes. |
| `POLLING_INTERVAL_SECONDS` | Number of seconds between polling cycles. |
| `API_TOKEN` | Authentication token used with brapi.dev. |
| `SMTP_HOST` | SMTP server hostname. |
| `SMTP_PORT` | SMTP server port, usually `587` for TLS. |
| `SMTP_USERNAME` | SMTP account username. |
| `SMTP_PASSWORD` | SMTP password or provider-specific app password. |
| `SMTP_FROM` | Email address used as the sender. |
| `SMTP_TO` | Email address that receives alerts. |

For Gmail, use an App Password instead of your regular account password.

## Architecture

The project follows a lightweight layered architecture with dependency inversion:

```text
Program
  ├── loads and validates configuration
  ├── composes dependencies
  └── starts the monitoring service

StockPriceAlertService
  ├── controls the polling cycle
  ├── compares current prices with reference prices
  └── triggers notifications

IStockPriceFetcher
  └── BrapiStockPriceFetcher
      ├── calls the external brapi.dev API
      ├── handles HTTP failures and retries
      └── deserializes the JSON response

IAlertService
  └── EmailAlertService
      └── sends notifications through SMTP
```
