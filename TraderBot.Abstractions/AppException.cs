namespace TraderBot.Abstractions;

public class AppException : Exception
{
    public AppException(string errorCode, string errorMessage) : base(errorMessage)
    {
    }

    public string ErrorCode { get; init; }
}