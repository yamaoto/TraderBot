using Grpc.Core;
using TraderBot.Abstractions;
using TraderBot.BinanceConnect.Commands;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;

namespace TraderBot.BinanceConnect.Services;

public class SpotService : SpotGrpc.SpotGrpcBase
{
    private readonly ILogger<SpotService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SpotService(ILogger<SpotService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public override async Task<OpenSpotResponse> OpenSpot(OpenSpotRequest request, ServerCallContext context)
    {
        var openSpotCommand = _serviceProvider.GetRequiredService<OpenSpotCommand>();
        try
        {
            var result = await openSpotCommand.OpenSpotAsync(request);
            return new OpenSpotResponse
            {
                Result = true,
                OrderId = result.OrderId,
                ClientOrderId = result.ClientOrderId,
                ErrorCode = "",
                ErrorMessage = ""
            };
        }
        catch (AppException e)
        {
            _logger.LogError(e, e.Message);
            return new OpenSpotResponse
            {
                Result = false,
                ErrorCode = e.ErrorCode,
                ErrorMessage = e.Message
            };
        }
        catch (Exception e)
        {
            const string errorMessage = "Error when opening new spot. See exception message below.";
            _logger.LogError(e, errorMessage);
            return new OpenSpotResponse
            {
                Result = false,
                ErrorCode = "UNKNOWN",
                ErrorMessage = errorMessage + e
            };
        }
    }

    public override async Task<GetUsdtBalanceResponse> GetUsdtBalance(GetUsdtBalanceRequest request, ServerCallContext context)
    {
        var openSpotCommand = _serviceProvider.GetRequiredService<GetFuturesBalanceRequest>();
        try
        {
            var result = await openSpotCommand.GetFuturesBalanceAsync(request.Mailbox);
            return new GetUsdtBalanceResponse
            {
                Result = true,
                Balance = result.ConvertToTypesProtoDecimal(),
                ErrorCode = "",
                ErrorMessage = ""
            };
        }
        catch (AppException e)
        {
            _logger.LogError(e, e.Message);
            return new GetUsdtBalanceResponse
            {
                Result = false,
                ErrorCode = e.ErrorCode,
                ErrorMessage = e.Message
            };
        }
        catch (Exception e)
        {
            const string errorMessage =
                "Error when getting futures account balance from usdt wallet. See exception message below.";
            _logger.LogError(e, errorMessage);
            return new GetUsdtBalanceResponse
            {
                Result = false,
                ErrorCode = "UNKNOWN",
                ErrorMessage = errorMessage + e
            };
        }
    }
}