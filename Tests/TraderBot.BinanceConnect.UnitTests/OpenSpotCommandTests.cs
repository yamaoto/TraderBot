using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using TraderBot.BinanceConnect.Commands;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;
using TraderBot.RavenDb.MailBoxDomain;
using TraderBot.TypesProto;

namespace TraderBot.BinanceConnect.UnitTests;

public class OpenSpotCommandTests
{
    [Fact]
    public async Task OpenSpotTest()
    {
        // Arrange
        const string symbol = "BTCUSDT";
        const string orderSide = "SELL";
        const decimal quantity = 1m;
        const decimal price = 100000m;
        var testData = new
        {
            orderId = 3255555655,
            symbol,
            status = "NEW",
            clientOrderId = "HtjEeA8MMOrabphk2EZwOP",
            price = "100000",
            avgPrice = "0.00000",
            origQty = "0.100",
            executedQty = "0",
            cumQty = "0",
            cumQuote = "0",
            timeInForce = "GTC",
            type = "LIMIT",
            reduceOnly = false,
            closePosition = false,
            side = orderSide,
            positionSide = "BOTH",
            stopPrice = "0",
            workingType = "CONTRACT_PRICE",
            priceProtect = false,
            origType = "LIMIT",
            updateTime = 1669274238162
        };
        var mailBox = new MailBoxSettingRecord("Name", "username", "password", "binanceKey", "binanceSecret",
            Array.Empty<string>());
        var mailBoxDalStub = new StubMailBoxDal();
        await mailBoxDalStub.UpsertMailBoxAsync(mailBox);
        var mockHttp = new MockHttpMessageHandler();
        var method = mockHttp
            .When("https://testnet.binancefuture.com/fapi/v1/order")
            .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(testData));
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("https://testnet.binancefuture.com");
        var command = new OpenSpotCommand(mailBoxDalStub, client, new BinanceMetrics(), new OptionsWrapper<BinanceOptions>(new BinanceOptions
        {
            UseTestnet = true
        }));

        var parameters =
            new OpenSpotRequest
            {
                TradingSymbol = symbol,
                OrderSide = orderSide == "SELL" ? OrderSideType.Sell : OrderSideType.Buy,
                Quantity = quantity.ConvertToTypesProtoDecimal(),
                Price = price.ConvertToTypesProtoDecimal(),
                Mailbox = "Name"
            };

        // Act
        var result = await command.OpenSpotAsync(parameters);

        // Assert
        Assert.Equal(1, mockHttp.GetMatchCount(method));
        Assert.Equal(testData.orderId, result.OrderId);
        Assert.Equal(testData.symbol, result.Symbol);
        Assert.Equal(testData.status, result.Status);
        Assert.Equal(testData.clientOrderId, result.ClientOrderId);
        Assert.Equal(testData.price, result.Price);
        Assert.Equal(testData.avgPrice, result.AvgPrice);
        Assert.Equal(testData.origQty, result.OrigQty);
        Assert.Equal(testData.executedQty, result.ExecutedQty);
        Assert.Equal(testData.cumQty, result.CumQty);
        Assert.Equal(testData.cumQuote, result.CumQuote);
        Assert.Equal(testData.timeInForce, result.TimeInForce);
        Assert.Equal(testData.type, result.Type);
        Assert.Equal(testData.reduceOnly, result.ReduceOnly);
        Assert.Equal(testData.closePosition, result.ClosePosition);
        Assert.Equal(testData.side, result.Side);
        Assert.Equal(testData.positionSide, result.PositionSide);
        Assert.Equal(testData.stopPrice, result.StopPrice);
        Assert.Equal(testData.workingType, result.WorkingType);
        Assert.Equal(testData.priceProtect, result.PriceProtect);
        Assert.Equal(testData.origType, result.OrigType);
        Assert.Equal(testData.updateTime, result.UpdateTime);
    }
}