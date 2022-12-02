using System.Globalization;

namespace TraderBot.EmailListener.Commands;

public abstract class BaseParser
{
    public abstract bool IsApplicable(EmailMessage message);

    public IReadOnlyDictionary<string, string> SplitParameters(string parametersLine)
    {
        var parameters = new Dictionary<string, string>();
        var start = 0;
        var index = 0;
        var key = "";
        var value = "";
        var mode = 0;
        while (index < parametersLine.Length)
        {
            switch (mode)
            {
                case 0 when parametersLine[index] == ':':
                    // key
                    key = parametersLine.Substring(start, index - start).Trim();
                    start = index + 2;
                    mode = 1;
                    break;

                case 1 when parametersLine[index] == ',' ||
                            (parametersLine[index] == '.' && index + 1 == parametersLine.Length):
                    // value
                    value = parametersLine.Substring(start, index - start).Trim();
                    parameters.Add(key, value);
                    start = index + 2;
                    mode = 0;

                    break;
            }

            index++;
        }

        return parameters;
    }

    public bool TryParseAmount(string amountString, out decimal amount)
    {
        return decimal.TryParse(amountString, NumberFormatInfo.InvariantInfo, out amount);
    }

    public bool TryParseDateTime(string dateTimeString, out DateTime dateTime)
    {
        return DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo,
            DateTimeStyles.AdjustToUniversal, out dateTime);
    }
}