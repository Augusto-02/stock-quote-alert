# Stock Quote Alert

Stock Quote Alert is a .NET 8 console application that monitors stock prices and notifies the user when a configured buy or sell reference price is reached. It uses short polling to periodically fetch Brazilian stock quotes from the [brapi.dev](https://brapi.dev/) API, compare them with the configured references, and send an email alert when a condition is met.

Short polling is used because the free brapi.dev API may update stock prices with a delay of approximately 5 to 15 minutes. The application therefore periodically checks the API instead of relying on real-time price updates.

## How to use

### Prerequisites

- .NET 8 SDK
- A brapi.dev API token
- An SMTP account for sending email notifications

### Configuration

1. Open a terminal in the project directory, the directory containing the solution and project files. If necessary, navigate to the inner `stock-quote-alert` folder:

```bash
cd path/to/stock-quote-alert/stock-quote-alert
```

2. Copy `Config/.env.example` to `Config/.env`.
3. Fill in the required environment variables.
4. Run the application from this directory:

```bash
dotnet run
```

5. Enter one or more stock alerts in the following format:

```text
TICKER SELL_PRICE BUY_PRICE
```

For multiple stocks, append additional groups of three values:

```text
PETR4 40.00 35.00 VALE3 75.00 65.00
```

The application validates the complete input before starting the monitoring process. If any value is invalid, the application reports all validation errors and does not start polling.

The first price check runs immediately. Subsequent checks run according to `POLLING_INTERVAL_SECONDS`.

## Unit tests

The project includes a separate `stock-quote-alert.Tests` project with xUnit unit tests. The tests cover input validation and the main alert service scenarios, including API failures, buy alerts, sell alerts, prices within the configured range, and multiple tickers.

The external API and SMTP service are replaced with mocks, so the tests do not require internet access or send real emails. Run them from the solution directory with:

```bash
dotnet test
```

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

## Use of AI

AI tools were used during development to assist with C# syntax, review the code, and write the unit tests. The models used were GPT-5.6 Luna and GPT-5.6 Sol.
