using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using TraderBot.BinanceConnectProto;
using TraderBot.OrderController.Commands;
using TraderBot.OrderController.Infrastructure;
using TraderBot.OrderControllerProto;
using TraderBot.RavenDb.MailBoxDomain;
using TraderBot.RavenDb.OrderDomain;
using TraderBot.TypesProto;

namespace TraderBot.OrderController.UnitTests;

public class CreateOrderTests
{
    [Fact]
    public async Task CreateOrderPositive()
    {
        // Arrange
        const string symbol = "BTCUSDT";
        const string from = "ALLOW";
        var dalMock = new Mock<IOrderDal>();
        var mailBox = new MailBoxSettingRecord("Name", "username", "password", "binanceKey", "binanceSecret",
            Array.Empty<string>());
        var mailBoxDalStub = new StubMailBoxDal();
        await mailBoxDalStub.UpsertMailBoxAsync(mailBox);
        var getExchangeStepSizeMock = new Mock<IGetExchangeStepSize>();
        getExchangeStepSizeMock.Setup(s => s.GetExchangeSymbolInfoAsync(symbol))
            .Returns(Task.FromResult(new ExchangeSymbolInfo(0.001m, 0.001m, 1.00m)));
        var grpcMock = new Mock<SpotGrpc.SpotGrpcClient>();
        var response =
            grpcMock.Setup(s => s.GetUsdtBalanceAsync(It.IsAny<GetUsdtBalanceRequest>(), null, null, CancellationToken.None))
                .Returns(CallHelpers.CreateAsyncUnaryCall(new GetUsdtBalanceResponse
                {
                    Balance = 100m.ConvertToTypesProtoDecimal(),
                    Result = true
                }));
        grpcMock.Setup(s => s.OpenSpotAsync(It.IsAny<OpenSpotRequest>(), null, null, CancellationToken.None))
            .Returns(CallHelpers.CreateAsyncUnaryCall(new OpenSpotResponse
            {
                Result = true
            }));
        var telegramMock = new Mock<ITelegramService>();
        var followList = new[] { "from" };
        var tradingOptions = new TradingOptions
        {
            {
                symbol, new TradingOptions.TradingSymbolOptions
                {
                    Rate = 0.5m
                }
            }
        };
        var command = new CreateOrder(dalMock.Object, mailBoxDalStub, getExchangeStepSizeMock.Object, grpcMock.Object,
            telegramMock.Object, new OptionsWrapper<TradingOptions>(tradingOptions),
            new NullLogger<CreateOrder>());
        var request = new CreateOrderRequest
        {
            From = from,
            OrderSide = OrderSideType.Buy,
            Price = 0.1m.ConvertToTypesProtoDecimal(),
            TradingSymbol = symbol,
            Mailbox = "Name"
        };

        // Act
        await command.CreateOrderAsync(request);

        // Assert
    }

    [Fact]
    public async Task CreateOrderDisallowed()
    {
        // Arrange
        var dalMock = new Mock<IOrderDal>();
        var mailBox = new MailBoxSettingRecord("Name", "username", "password", "binanceKey", "binanceSecret",
            Array.Empty<string>());
        var mailBoxDalStub = new StubMailBoxDal();
        await mailBoxDalStub.UpsertMailBoxAsync(mailBox);
        var getExchangeStepSizeMock = new Mock<IGetExchangeStepSize>();
        var grpcMock = new Mock<SpotGrpc.SpotGrpcClient>();

        var telegramMock = new Mock<ITelegramService>();
       
        var command = new CreateOrder(dalMock.Object, mailBoxDalStub, getExchangeStepSizeMock.Object, grpcMock.Object,
            telegramMock.Object, new OptionsWrapper<TradingOptions>(new TradingOptions()),
            new NullLogger<CreateOrder>());
        var request = new CreateOrderRequest
        {
            From = "not-allowed",
            OrderSide = OrderSideType.Buy,
            Price = 0.1m.ConvertToTypesProtoDecimal(),
            TradingSymbol = "BTCUSDT",
            Mailbox = "Name"
        };
        // Act

        await command.CreateOrderAsync(request);

        // Assert
    }
}