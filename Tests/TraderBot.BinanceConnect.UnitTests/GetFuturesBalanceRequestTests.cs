using System.Globalization;
using System.Net;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using TraderBot.BinanceConnect.Commands;
using TraderBot.BinanceConnect.Infrastructure;
using TraderBot.RavenDb.MailBoxDomain;

namespace TraderBot.BinanceConnect.UnitTests;

public class GetFuturesBalanceRequestTests
{
    [Fact]
    public async Task FetFuturesBalanceRequestTest()
    {
        var testData = new[]
        {
            new
            {
                accountAlias = "FzSgAuSgmYTisR",
                asset = "USDT",
                balance = "15000.00000000",
                crossWalletBalance = "15000.00000000",
                crossUnPnl = "0.00000000",
                availableBalance = "9000.00000000",
                maxWithdrawAmount = "9000.00000000",
                marginAvailable = true,
                updateTime = 1669223878607
            }
        };
        var mailBox = new MailBoxSettingRecord("Name", "username", "password", "binanceKey", "binanceSecret",
            Array.Empty<string>());
        var mailBoxDalStub = new StubMailBoxDal();
        await mailBoxDalStub.UpsertMailBoxAsync(mailBox);
        var mockHttp = new MockHttpMessageHandler();
        var method = mockHttp
            .When("https://testnet.binancefuture.com/fapi/v2/balance")
            .Respond(HttpStatusCode.OK, "application/json", JsonConvert.SerializeObject(testData));
        var client = mockHttp.ToHttpClient();
        client.BaseAddress = new Uri("https://testnet.binancefuture.com");
        var command = new GetFuturesBalanceRequest(mailBoxDalStub, client, new BinanceMetrics(), new OptionsWrapper<BinanceOptions>(new BinanceOptions
        {
            UseTestnet = true
        }));

        // Act
        var result = await command.GetFuturesBalanceAsync(mailBox.Name);

        // Assert
        Assert.Equal(1, mockHttp.GetMatchCount(method));
        Assert.Equal(decimal.Parse(testData[0].availableBalance, CultureInfo.InvariantCulture), result);
    }
}