using Grpc.Core;
using TraderBot.EmailListener.Commands;
using TraderBot.EmailListenerProto;

namespace TraderBot.EmailListener.Services;

public class EmailListenerService : EmailListenerGrpc.EmailListenerGrpcBase
{
    private readonly ILogger<EmailListenerService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EmailListenerService(
        IServiceProvider serviceProvider,
        ILogger<EmailListenerService> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public override async Task<TickEmailCollectJobResponse> TickEmailCollectJob(TickEmailCollectJobRequest request,
        ServerCallContext context)
    {
        var collectProcessCommand = _serviceProvider.GetRequiredService<CollectProcessCommand>();

        await collectProcessCommand.CollectProcessAsync(DateTime.Today.AddDays(-1), "");

        return new TickEmailCollectJobResponse
        {
            Result = true,
            ErrorCode = "",
            ErrorMessage = ""
        };
    }
}