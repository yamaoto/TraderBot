using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using TraderBot.BinanceConnectProto;
using TraderBot.OrderController.Commands;
using TraderBot.OrderController.Infrastructure;
using TraderBot.OrderControllerProto;
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
        var dalMock = new Mock<IOrderDal>();
        var getExchangeStepSizeMock = new Mock<IGetExchangeStepSize>();
        getExchangeStepSizeMock.Setup(s => s.GetExchangeStepSizeAsync(symbol))
            .Returns(Task.FromResult(0.001m));
        var grpcMock = new Mock<SpotGrpc.SpotGrpcClient>();
        var response =
            grpcMock.Setup(s => s.GetUsdtBalanceAsync(It.IsAny<Empty>(), null, null, CancellationToken.None))
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
        var followOptions = new FollowOptions
        {
            Allowed = new[] { "from" }
        };
        var tradingOptions = new TradingOptions
        {
            {
                symbol, new TradingOptions.TradingSymbolOptions
                {
                    Rate = 0.5m
                }
            }
        };
        var command = new CreateOrder(dalMock.Object, getExchangeStepSizeMock.Object, grpcMock.Object,
            telegramMock.Object, new OptionsWrapper<TradingOptions>(tradingOptions),
            new OptionsWrapper<FollowOptions>(followOptions), new NullLogger<CreateOrder>());
        var request = new CreateOrderRequest
        {
            From = "from",
            OrderSide = OrderSideType.Buy,
            Price = 0.1m.ConvertToTypesProtoDecimal(),
            TradingSymbol = symbol
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
        var getExchangeStepSizeMock = new Mock<IGetExchangeStepSize>();
        var grpcMock = new Mock<SpotGrpc.SpotGrpcClient>();

        var telegramMock = new Mock<ITelegramService>();
        var followOptions = new FollowOptions
        {
            Allowed = new[] { "allowed" }
        };
        var command = new CreateOrder(dalMock.Object, getExchangeStepSizeMock.Object, grpcMock.Object,
            telegramMock.Object, new OptionsWrapper<TradingOptions>(new TradingOptions()),
            new OptionsWrapper<FollowOptions>(followOptions), new NullLogger<CreateOrder>());
        var request = new CreateOrderRequest
        {
            From = "not-allowed",
            OrderSide = OrderSideType.Buy,
            Price = 0.1m.ConvertToTypesProtoDecimal(),
            TradingSymbol = "BTCUSDT"
        };
        // Act

        await command.CreateOrderAsync(request);

        // Assert
    }
}