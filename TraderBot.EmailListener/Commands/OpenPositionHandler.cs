using TraderBot.Abstractions;

namespace TraderBot.EmailListener.Commands;

public class OpenPositionHandler : BaseParser
{
    public override bool IsApplicable(EmailMessage message)
    {
        return message.Subject.Contains("Copied Position Successfully Opened");
    }

    public TraderWagonOpenSpotParameters ParseOpenPosition(EmailMessage message)
    {
        var beginning = message.HtmlBody.IndexOf("Your copied position from", StringComparison.InvariantCulture);
        var marker = message.HtmlBody.IndexOf("successfully opened", StringComparison.InvariantCulture);
        var from = message.HtmlBody.Substring(beginning + 26, marker - beginning - 26 - 11);
        var atText = message.HtmlBody.Substring(marker + 23, 19);
        var parametersLine =
            message.HtmlBody.Substring(marker + 49, message.HtmlBody.IndexOf('\n', marker) - marker - 49);
        var parameters = SplitParameters(parametersLine);
        if (!parameters.ContainsKey("open price") || !TryParseAmount(parameters["open price"], out var openPrice))
            throw new InvalidOperationException("There is no open price parameter found in message");

        if (!TryParseDateTime(atText, out var at))
            throw new InvalidOperationException("DateTime format is not match yyyy-MM-dd HH:mm:ss");
        if (!OrderSide.TryParse(parameters["opening side"], out var orderSide))
            throw new InvalidOperationException("OrderSide is no valid");
        if (!TradingSymbol.TryParse(parameters["order type"], out var orderType))
            throw new InvalidOperationException("OrderType is no valid");

        return new TraderWagonOpenSpotParameters(orderType!, orderSide!,
            openPrice)
        {
            From = from,
            At = at
        };
    }
}