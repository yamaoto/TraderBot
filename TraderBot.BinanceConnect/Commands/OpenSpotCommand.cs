using Binance.Common;
using Microsoft.Extensions.Options;
using TraderBot.BinanceConnect.BinanceSupport;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.BinanceConnectProto;
using TraderBot.RavenDb.MailBoxDomain;

namespace TraderBot.BinanceConnect.Commands;

public class OpenSpotCommand
{
    private readonly IMailBoxDal _mailBoxDal;
    private readonly HttpClient _httpClient;
    private readonly IOptions<BinanceOptions> _options;

    public OpenSpotCommand(
        IMailBoxDal mailBoxDal,
        HttpClient httpClient,
        IOptions<BinanceOptions> options
    )
    {
        _mailBoxDal = mailBoxDal;
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<FuturesNewOrderResponse> OpenSpotAsync(OpenSpotRequest openSpotParameters)
    {
        var mailBox = await _mailBoxDal.GetMailBoxAsync(openSpotParameters.Mailbox);
        var baseUrl = _options.Value.UseTestnet ? "https://testnet.binancefuture.com" : "https://fapi.binance.com";
        var futureTrade = new FuturesTradeClient(_httpClient, baseUrl, mailBox.BinanceApiKey,
            new BinanceHmac(mailBox.BinanceApiSecret)
        );
        var newOrderResponse = await futureTrade.NewOrderAsync(openSpotParameters);
        return newOrderResponse;
    }
}