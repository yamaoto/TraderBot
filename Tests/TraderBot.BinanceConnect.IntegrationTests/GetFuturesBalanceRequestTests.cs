using Microsoft.AspNetCore.Mvc.Testing;
using TraderBot.BinanceConnect.Commands;
using TraderBot.RavenDb.MailBoxDomain;
using TraderBot.RavenDb.OrderDomain;

namespace TraderBot.BinanceConnect.IntegrationTests;

public class GetFuturesBalanceRequestTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetFuturesBalanceRequestTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetFuturesBalanceRequest()
    {
        // Arrange
        var client = _factory
            .WithWebHostBuilder(c => c
                .UseSetting("RavenDb:UseStub", "true")
                .UseSetting("Binance:UseTestnet", "true")
            )
            .CreateClient();
        var command = _factory.Services.GetRequiredService<GetFuturesBalanceRequest>();
        var mailBoxDal = _factory.Server.Services.GetRequiredService<IMailBoxDal>();
        var mailBox = new MailBoxSettingRecord("test", "username", "password",
            Environment.GetEnvironmentVariable("BINANCE_API_KEY")!,
            Environment.GetEnvironmentVariable("BINANCE_API_SECRET")!,
            Array.Empty<string>());
        await mailBoxDal.UpsertMailBoxAsync(mailBox);
        // Act
        var result = await command.GetFuturesBalanceAsync("test");

        // Assert
        Assert.True(result > 0, "result > 0");
    }
}