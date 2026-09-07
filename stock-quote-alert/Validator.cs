namespace stock_quote_alert;

using System.Globalization;

public class StockPriceAlert
{
    public required string Ticker { get; init; }
    public required decimal BuyPriceReference { get; init; }
    public required decimal SellPriceReference { get; init; }

    public bool AlreadySentAlert { get; private set; } = false;

    public void MarkAlertSent(bool alert)
    {
        AlreadySentAlert = alert;
    }
}

public static class StockPriceAlertParser
{
    public static List<StockPriceAlert> Validate(string request)
    {
        string[] inputSplitted = request.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        int inputLength = inputSplitted.Length;
        List<string> errorsList = new List<string>();
        List<StockPriceAlert> requestList = new List<StockPriceAlert>();

        if (inputLength % 3 != 0 || inputLength == 0)
        {
            throw new ArgumentException(
                "The number of parameters must be a multiple of 3. Format Expected is: ticker sell price buy price"
            );
        }

        for (int index = 0; index < inputLength; index += 3)
        {
            bool errors = false;

            string ticker = inputSplitted[index];
            decimal sellPrice;
            decimal buyPrice;
            if (!ValidatePrices(inputSplitted[index + 1], out sellPrice))
            {
                errorsList.Add($"Reference sell price of ticker: {ticker} is not valid");
                errors = true;
            }

            if (!ValidatePrices(inputSplitted[index + 2], out buyPrice))
            {
                errorsList.Add($"Reference buy price of ticker: {ticker} is not valid");
                errors = true;
            }

            if (!errors)
            {
                requestList.Add(new StockPriceAlert
                {
                    Ticker = ticker,
                    SellPriceReference = sellPrice,
                    BuyPriceReference = buyPrice
                });
            }
        }

        if (errorsList.Count == 0)
        {
            return requestList;
        }

        string errorMessage = string.Join(
            Environment.NewLine,
            errorsList
        );
        throw new ArgumentException(
            $"Invalid input format:{Environment.NewLine}{errorMessage}"
        );
    }


    private static bool ValidatePrices(string priceText, out decimal price)
    {
        return decimal.TryParse(
            priceText,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out price) && price > 0;
    }
}