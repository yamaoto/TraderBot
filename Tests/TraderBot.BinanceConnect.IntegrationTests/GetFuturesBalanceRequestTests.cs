using Microsoft.AspNetCore.Mvc.Testing;
using TraderBot.BinanceConnect.Commands;

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
        var client = _factory.CreateClient();
        var command = _factory.Services.GetRequiredService<GetFuturesBalanceRequest>();

        // Act
        var result = await command.GetFuturesBalanceAsync();

        // Assert
        Assert.True(result > 0, "result > 0");
    }
}