namespace stock_quote_alert.Tests;

using Xunit;

public class StockPriceAlertParserTests
{
    [Theory]
    [InlineData("", false, 0)]
    [InlineData("PETR4 22.67", false, 0)]
    [InlineData("PETR4 22.67 22.59 VALE3", false, 0)]
    [InlineData("PETR4 abc 22.59", false, 0)]
    [InlineData("PETR4 0 22.59", false, 0)]
    [InlineData("PETR4 22.67 22.59", true, 1)]
    [InlineData("PETR4 22.67 22.59 VALE3 60.10 59.90", true, 2)]
    public void ShouldValidateInputCorrectly(
        string input,
        bool shouldBeValid,
        int expectedAlertCount)
    {
        if (!shouldBeValid)
        {
            Assert.Throws<ArgumentException>(() =>
                StockPriceAlertParser.Validate(input));

            return;
        }

        List<StockPriceAlert> result =
            StockPriceAlertParser.Validate(input);

        Assert.Equal(expectedAlertCount, result.Count);
    }
}