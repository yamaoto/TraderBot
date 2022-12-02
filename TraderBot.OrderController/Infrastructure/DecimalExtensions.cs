using Decimal = TraderBot.TypesProto.Decimal;

namespace TraderBot.OrderController.Infrastructure;

public static class DecimalExtensions
{
    public static Decimal ConvertToTypesProtoDecimal(this decimal value)
    {
        var bytes = decimal.GetBits(value);
        return new Decimal
        {
            B1 = bytes[0],
            B2 = bytes[1],
            B3 = bytes[2],
            B4 = bytes[3]
        };
    }

    public static decimal ConvertToRegularDecimal(this Decimal value)
    {
        return new decimal(new[] { value.B1, value.B2, value.B3, value.B4 });
    }
}