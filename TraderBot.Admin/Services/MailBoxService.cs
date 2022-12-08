using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using TraderBot.MailBoxProto;
using TraderBot.RavenDb.MailBoxDomain;

namespace TraderBot.Admin.Services;

public class MailBoxService : MailBoxGrpc.MailBoxGrpcBase
{
    private readonly IMailBoxDal _mailBoxDal;

    public MailBoxService(
        IMailBoxDal mailBoxDal
    )
    {
        _mailBoxDal = mailBoxDal;
    }

    public override async Task<GetMailBoxesResponse> GetMailBoxes(Empty request, ServerCallContext context)
    {
        var mailBoxes = (await _mailBoxDal.GetMailBoxesAsync(context.CancellationToken))
            .Select(s => new MailBoxSettings()
            {
                Name = s.Name,
                Username = s.Username
            });
        return new GetMailBoxesResponse()
        {
            Result = true,
            Items = { mailBoxes }
        };
    }

    public override async Task<UpsertMailBoxResponse> UpsertMailBox(MailBoxSettings request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new UpsertMailBoxResponse()
            {
                ErrorCode = "NAME_REQUIRED",
                ErrorMessage = "Fill name"
            };
        }
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return new UpsertMailBoxResponse()
            {
                ErrorCode = "USERNAME_REQUIRED",
                ErrorMessage = "Fill username"
            };
        }
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return new UpsertMailBoxResponse()
            {
                ErrorCode = "PASSWORD_REQUIRED",
                ErrorMessage = "password name"
            };
        }
        await _mailBoxDal.UpsertMailBoxAsync(new MailBoxSettingRecord(request.Name, request.Username, request.Password,
            request.BinanceApiKey, request.BinanceApiSecret));
        return new UpsertMailBoxResponse()
        {
            Result = true
        };
    }

    public override async Task<DeleteMailBoxResponse> DeleteMailBox(DeleteMailBoxRequest request, ServerCallContext context)
    {
        await _mailBoxDal.DeleteMailBoxAsync(request.Name);
        return new DeleteMailBoxResponse()
        {
            Result = true
        };
    }
}