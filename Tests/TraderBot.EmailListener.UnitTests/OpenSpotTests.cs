using TraderBot.EmailListener.Commands;

namespace TraderBot.EmailListener.UnitTests;

public class OpenSpotTests
{
    [Fact]
    public void ParseParameters()
    {
        // Arrange
        var command = new OpenPositionHandler();

        const string testData = "order type: ETHUSDT , opening side: BUY, open price: 1125.56000.";

        // Act
        var parameters = command.SplitParameters(testData);

        // Assert
        Assert.True(parameters.ContainsKey("order type"));
        Assert.Equal("ETHUSDT", parameters["order type"]);

        Assert.True(parameters.ContainsKey("opening side"));
        Assert.Equal("BUY", parameters["opening side"]);

        Assert.True(parameters.ContainsKey("open price"));
        Assert.Equal("1125.56000", parameters["open price"]);
    }

    [Fact]
    public void Parse()
    {
        // Arrange
        const string TextBody = @"
Copied Position Successfully Opened
Dear customer,
Your copied position from Mirabelle’s Elon Reeve Musk portfolio successfully opened at 2022-11-22 14:16:31(UTC), order type: ETHUSDT , opening side: BUY, open price: 1125.56000.
Risk warning: Buying, selling, holding, or in any other way participating in cryptocurrency Futures trading is subject to high market risk. The volatile and unpredictable nature of cryptocurrency markets may result in a significant loss. TraderWagon is not responsible for any losses or damage that may incur from price fluctuations when you buy, sell, hold, or in any other way participate in cryptocurrency trading.
TraderWagon Team
This is an automated message, please do not reply.
© 2021  TraderWagon.com All Rights Reserved
Reserved URL:  www.traderwagon.com";
        var message = new EmailMessage("", "Copied Position Successfully Opened", "", TextBody);
        var command = new OpenPositionHandler();

        // Act
        Assert.True(command.IsApplicable(message));
        var parameters = command.ParseOpenPosition(message);

        // Assert
        Assert.Equal("Mirabelle’s Elon Reeve Musk", parameters.From);
        Assert.Equal(new DateTime(2022, 11, 22, 14, 16, 31, DateTimeKind.Utc), parameters.At);
        Assert.Equal("ETHUSDT", parameters.TradingSymbol.Value);
        Assert.Equal("BUY", parameters.OpeningSide.Value);
        Assert.Equal(1125.56m, parameters.Price);
    }
}