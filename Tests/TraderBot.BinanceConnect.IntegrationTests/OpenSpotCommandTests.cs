using System.Globalization;
using Microsoft.AspNetCore.Mvc.Testing;
using TraderBot.BinanceConnect.Commands;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;
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
        const decimal quantity = 1m;
        const decimal price = 100000m;
        var client = _factory.CreateClient();
        var parameters = new OpenSpotRequest
        {
            TradingSymbol = symbol,
            OrderSide = orderSide == "SELL" ? OrderSideType.Sell : OrderSideType.Buy,
            Quantity = quantity.ConvertToTypesProtoDecimal(),
            Price = price.ConvertToTypesProtoDecimal()
        };
        var command = _factory.Services.GetRequiredService<OpenSpotCommand>();

        // Act
        var result = await command.OpenSpotAsync(parameters);

        // Assert
        Assert.Equal("NEW", result.Status);
        Assert.Equal(quantity.ToString(CultureInfo.InvariantCulture), result.OrigQty);
        Assert.Equal(symbol, result.Symbol);
    }
}