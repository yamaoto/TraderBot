using Microsoft.AspNetCore.Mvc.Testing;
using TraderBot.BinanceConnect.Commands;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;
using TraderBot.RavenDb.MailBoxDomain;
using TraderBot.TypesProto;

namespace TraderBot.BinanceConnect.IntegrationTests;

public class OpenSpotCommandTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public OpenSpotCommandTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task OpenSpotOnTestNetTest()
    {
        // Arrange
        const string symbol = "BTCUSDT";
        const string orderSide = "SELL";
        const decimal quantity = 0.01m;
        const decimal price = 10000m;
        var client = _factory
            .WithWebHostBuilder(c => c
                .UseSetting("RavenDb:UseStub", "true")
                .UseSetting("Binance:UseTestnet", "true")
            )
            .CreateClient();
        var parameters = new OpenSpotRequest
        {
            TradingSymbol = symbol,
            OrderSide = orderSide == "SELL" ? OrderSideType.Sell : OrderSideType.Buy,
            Quantity = quantity.ConvertToTypesProtoDecimal(),
            Price = price.ConvertToTypesProtoDecimal(),
            Mailbox = "test"
        };
        var command = _factory.Services.GetRequiredService<OpenSpotCommand>();
        var mailBoxDal = _factory.Server.Services.GetRequiredService<IMailBoxDal>();
        var mailBox = new MailBoxSettingRecord("test", "username", "password",
            Environment.GetEnvironmentVariable("BINANCE_API_KEY")!,
            Environment.GetEnvironmentVariable("BINANCE_API_SECRET")!,
            Array.Empty<string>());
        await mailBoxDal.UpsertMailBoxAsync(mailBox);

        // Act
        var result = await command.OpenSpotAsync(parameters);

        // Assert
        Assert.Equal("NEW", result.Status);
        Assert.Equal("0.010", result.OrigQty);
        Assert.Equal(symbol, result.Symbol);
    }
}