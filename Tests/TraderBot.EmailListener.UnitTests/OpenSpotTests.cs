using TraderBot.EmailListener.Commands;

namespace TraderBot.EmailListener.UnitTests;

public class OpenSpotTests
{
    [Theory]
    [InlineData("order type: BTCUSDT , opening side: SELL, open price: 16814.70000.", "BTCUSDT", "SELL", "16814.70000")]
    [InlineData("order type: ETHUSDT , opening side: BUY, open price: 1125.56000.", "ETHUSDT", "BUY", "1125.56000")]
    [InlineData("order type: BTCUSDT , opening side: SELL, open price: 16814.70000. ", "BTCUSDT", "SELL", "16814.70000")]
    [InlineData("order type: ETHUSDT , opening side: BUY, open price: 1125.56000. ", "ETHUSDT", "BUY", "1125.56000")]
    public void ParseParameters(string testData, string orderType, string openSide, string openPrice)
    {
        // Arrange
        var command = new OpenPositionHandler();

        // Act
        var parameters = command.SplitParameters(testData);

        // Assert
        Assert.True(parameters.ContainsKey("order type"));
        Assert.Equal(orderType, parameters["order type"]);

        Assert.True(parameters.ContainsKey("opening side"));
        Assert.Equal(openSide, parameters["opening side"]);

        Assert.True(parameters.ContainsKey("open price"));
        Assert.Equal(openPrice, parameters["open price"]);
    }

    [Fact]
    public void Parse()
    {
        // Arrange
        var message = new EmailMessage("", "", "Copied Position Successfully Opened", HtmlBody);
        var command = new OpenPositionHandler();

        // Act
        Assert.True(command.IsApplicable(message));
        var parameters = command.ParseOpenPosition(message);

        // Assert
        Assert.Equal("騏騏戰士77’s smart2h", parameters.From);
        Assert.Equal(new DateTime(2022, 12, 07, 17, 30, 11, DateTimeKind.Utc), parameters.At);
        Assert.Equal("BTCUSDT", parameters.TradingSymbol.Value);
        Assert.Equal("SELL", parameters.OpeningSide.Value);
        Assert.Equal(16814.70000m, parameters.Price);
    }

    const string HtmlBody =
"""
<html lang="en">
  <head>
    <meta name="viewport" content="width=device-width,minimum-scale=1,maximum-scale=1.0,initial-scale=1,user-scalable=no,viewport-fit=true" />
  </head> 
  <body style="margin: 0; padding: 0;">
<div style="background-color: #F2F6FA; display: flex; justify-content: center;">
  <div style="margin: 40px;">
  <div style="background-color: #E1EAF7; max-width: 600px; height: 140px; display: flex; justify-content: center; align-items: center; border-top-left-radius: 16px; border-top-right-radius: 16px;">
  <img src="https://static.traderwagon.com/static/images/socialtrading-ui/logo-horizontal.svg" style="width: 200px; height: 54px; font-size:30px;"/>
</div>
<div style="max-width: 600px; padding: 40px; box-sizing: border-box; border-bottom-left-radius: 16px; border-bottom-right-radius: 16px; margin-bottom: 28px; background-color: white;">
<h1 style="font-size: 24px; line-height: 32px; font-weight: 700; padding-top: 22px; padding-bottom: 24px;">Copied Position Successfully Opened</h1>
<div style="color: #6C6C6C; font-size: 14px; line-height: 24px; font-weight: 400; padding-bottom: 36px;">Dear customer,</div>
    <p style="color: #6C6C6C; font-size: 14px; line-height: 24px; font-weight: 400; padding-bottom: 36px;">
   Your copied position from 騏騏戰士77’s smart2h portfolio successfully opened at 2022-12-07 17:30:11(UTC), order type: BTCUSDT , opening side: SELL, open price: 16814.70000. 
<br>
Risk warning: Buying, selling, holding, or in any other way participating in cryptocurrency Futures trading is subject to high market risk. The volatile and unpredictable nature of cryptocurrency markets may result in a significant loss. TraderWagon is not responsible for any losses or damage that may incur from price fluctuations when you buy, sell, hold, or in any other way participate in cryptocurrency trading.
    </p>
    <div style="color: #72768F; font-size: 14px; line-height: 20px; font-weight: 400;">TraderWagon Team<br>This is an automated message, please do not reply.</div>
</div>
<div style="color: #87909C; font-size: 14px; line-height: 20px; font-weight: 400; text-align: center;">© 2021 <a href="https://www.traderwagon.com" target="_blank" style="color: #87909C; text-decoration: none;">TraderWagon.com</a> All Rights Reserved<br>Reserved URL: <a href="https://www.traderwagon.com" target="_blank" style="color: #87909C; text-decoration: none;">www.traderwagon.com</a></div>
  </div>
</div>
</body></html>
""";
}