using TraderBot.Abstractions;

namespace TraderBot.EmailListener.Commands;

public class ClosePositionHandler : BaseParser
{
    public override bool IsApplicable(EmailMessage message)
    {
        return message.Subject.Contains("Copied Position Successfully Closed");
    }

    public TraderWagonCloseSpotParameters ParseClosePosition(EmailMessage message)
    {
        var beginning = message.TextBody.IndexOf("Your copied position from");
        var marker = message.TextBody.IndexOf("has been closed successfully");
        var from = message.TextBody.Substring(beginning + 26, marker - beginning - 26 - 11);
        var atText = message.TextBody.Substring(marker + 32, 19);
        var parametersLine =
            message.TextBody.Substring(marker + 58, message.TextBody.IndexOf('\n', marker) - marker - 58);
        var parameters = SplitParameters(parametersLine);
        if (!parameters.ContainsKey("close price") || !TryParseAmount(parameters["close price"], out var closePrice))
            throw new InvalidOperationException("There is no close price parameter found in message");

        if (!parameters.ContainsKey("realized profit") ||
            !TryParseAmount(parameters["realized profit"], out var realizedProfit))
            throw new InvalidOperationException("There is no realized profit parameter found in message");

        if (!TryParseDateTime(atText, out var at))
            throw new InvalidOperationException("DateTime format is not match yyyy-MM-dd HH:mm:ss");
        if (!TradingSymbol.TryParse(parameters["order type"], out var orderType))
            throw new InvalidOperationException("OrderType is no valid");

        return new TraderWagonCloseSpotParameters(from, at, orderType!,
            closePrice, realizedProfit);
    }
}