namespace stock_quote_alert.Tests;

using System.Net;
using Moq;
using Xunit;

public class StockPriceAlertServiceTests
{
    [Fact]
    public async Task ShouldNotSendAlertWhenApiFails()
    {
        var fetcher = new Mock<IStockPriceFetcher>();
        var alertService = new Mock<IAlertService>();

        fetcher
            .Setup(x => x.GetPriceAsync("PETR4"))
            .ThrowsAsync(new HttpRequestException("API unavailable"));

        var alert = CreateAlert("PETR4");

        var service = CreateService(fetcher, alertService);

        await service.CheckAlertAsync([alert]);

        alertService.Verify(
            x => x.SendAlertAsync(
                It.IsAny<string>(),
                It.IsAny<decimal>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Theory]
    [InlineData(18, "BUY")]
    [InlineData(32, "SELL")]
    [InlineData(25, "NONE")]
    public async Task ShouldProcessPriceCorrectly(
        decimal actualPrice,
        string expectedAlertType)
    {
        var fetcher = new Mock<IStockPriceFetcher>();
        var alertService = new Mock<IAlertService>();

        fetcher
            .Setup(x => x.GetPriceAsync("PETR4"))
            .ReturnsAsync(actualPrice);

        var alert = CreateAlert("PETR4");

        var service = CreateService(fetcher, alertService);

        await service.CheckAlertAsync([alert]);

        if (expectedAlertType == "NONE")
        {
            alertService.Verify(
                x => x.SendAlertAsync(
                    It.IsAny<string>(),
                    It.IsAny<decimal>(),
                    It.IsAny<string>()),
                Times.Never);

            return;
        }

        alertService.Verify(
            x => x.SendAlertAsync(
                "PETR4",
                actualPrice,
                expectedAlertType),
            Times.Once);
    }

    [Fact]
    public async Task ShouldProcessBuySellAndInsideRange()
    {
        var fetcher = new Mock<IStockPriceFetcher>();
        var alertService = new Mock<IAlertService>();

        fetcher
            .Setup(x => x.GetPriceAsync("PETR4"))
            .ReturnsAsync(18m);

        fetcher
            .Setup(x => x.GetPriceAsync("VALE3"))
            .ReturnsAsync(32m);

        fetcher
            .Setup(x => x.GetPriceAsync("ABEV3"))
            .ReturnsAsync(25m);

        var alerts = new List<StockPriceAlert>
        {
            CreateAlert("PETR4"),
            CreateAlert("VALE3"),
            CreateAlert("ABEV3")
        };

        var service = CreateService(fetcher, alertService);

        await service.CheckAlertAsync(alerts);

        alertService.Verify(
            x => x.SendAlertAsync("PETR4", 18m, "BUY"),
            Times.Once);

        alertService.Verify(
            x => x.SendAlertAsync("VALE3", 32m, "SELL"),
            Times.Once);

        alertService.Verify(
            x => x.SendAlertAsync(
                "ABEV3",
                25m,
                It.IsAny<string>()),
            Times.Never);
    }

    private static StockPriceAlert CreateAlert(string ticker)
    {
        return new StockPriceAlert
        {
            Ticker = ticker,
            BuyPriceReference = 20m,
            SellPriceReference = 30m
        };
    }

    private static StockPriceAlertService CreateService(
        Mock<IStockPriceFetcher> fetcher,
        Mock<IAlertService> alertService)
    {
        return new StockPriceAlertService(
            fetcher.Object,
            alertService.Object,
            30);
    }
}