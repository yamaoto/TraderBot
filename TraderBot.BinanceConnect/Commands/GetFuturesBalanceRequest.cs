using System.Globalization;
using Binance.Common;
using Microsoft.Extensions.Options;
using TraderBot.BinanceConnect.BinanceSupport;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.RavenDb.MailBoxDomain;

namespace TraderBot.BinanceConnect.Commands;

public class GetFuturesBalanceRequest
{
    private readonly IMailBoxDal _mailBoxDal;
    private readonly HttpClient _httpClient;
    private readonly IOptions<BinanceOptions> _options;

    public GetFuturesBalanceRequest(
        IMailBoxDal mailBoxDal,
        HttpClient httpClient,
        IOptions<BinanceOptions> options
    )
    {
        _mailBoxDal = mailBoxDal;
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<decimal> GetFuturesBalanceAsync(string mailbox)
    {
        var mailBox = await _mailBoxDal.GetMailBoxAsync(mailbox);
        var baseUrl = _options.Value.UseTestnet ? "https://testnet.binancefuture.com" : "https://fapi.binance.com";
        var futureTrade = new FuturesTradeClient(_httpClient, baseUrl, mailBox.BinanceApiKey,
            new BinanceHmac(mailBox.BinanceApiSecret)
        );
        var balance = await futureTrade.GetFuturesAccountBalanceV2Async();
        var usdtBalance = balance.FirstOrDefault(f => f.Asset == "USDT");
        if (usdtBalance == null) throw new Exception("There is not found USDT wallet in futures account");

        if (!decimal.TryParse(usdtBalance.AvailableBalance, CultureInfo.InvariantCulture, out var usdtBalanceAmount))
            throw new InvalidOperationException("Failed AvailableBalance parsing");

        return usdtBalanceAmount;
    }
}