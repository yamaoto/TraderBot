using TraderBot.EmailListener.Commands;

namespace TraderBot.EmailListener.UnitTests;

public class CloseSpotTests
{
    [Fact]
    public void ParseParameters()
    {
        // Arrange
        var command = new OpenPositionHandler();

        const string testData = "order type: MATICUSDT , close price: 0.84960, realized profit: 1.66610000.";

        // Act
        var parameters = command.SplitParameters(testData);

        // Assert
        Assert.True(parameters.ContainsKey("order type"));
        Assert.Equal("MATICUSDT", parameters["order type"]);

        Assert.True(parameters.ContainsKey("close price"));
        Assert.Equal("0.84960", parameters["close price"]);

        Assert.True(parameters.ContainsKey("realized profit"));
        Assert.Equal("1.66610000", parameters["realized profit"]);
    }

    [Fact]
    public void Parse()
    {
        // Arrange
        const string TextBody = @"
Dear customer,

Your copied position from TIMENACCI’s SNIPER SCAL portfolio has been closed successfully at 2022-11-22 14:23:35(UTC), order type: MATICUSDT , close price: 0.84960, realized profit: 1.66610000.
Risk warning: Buying, selling, holding, or in any other way of participating in cryptocurrency Futures trading is subject to high market risk. The volatile and unpredictable nature of cryptocurrency markets may result in a significant loss. TraderWagon is not responsible for any losses or damages that may incur from price fluctuations when you buy, sell, hold, or in any other way of participating in cryptocurrency trading.
TraderWagon Team
This is an automated message, please do not reply.
© 2021  TraderWagon.com All Rights Reserved
Reserved URL:  www.traderwagon.com";
        var message = new EmailMessage("", "Copied Position Successfully Closed", "", TextBody);
        var command = new ClosePositionHandler();

        // Act
        Assert.True(command.IsApplicable(message));
        var parameters = command.ParseClosePosition(message);

        // Assert
        Assert.Equal("TIMENACCI’s SNIPER SCAL", parameters.From);
        Assert.Equal(new DateTime(2022, 11, 22, 14, 23, 35, DateTimeKind.Utc), parameters.At);
        Assert.Equal("MATICUSDT", parameters.TradingSymbol.Value);
        Assert.Equal(0.84960m, parameters.ClosePrice);
        Assert.Equal(1.66610000m, parameters.RealizedProfit);
    }
}