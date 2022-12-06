namespace TraderBot.Abstractions;

public class AppException : Exception
{
    public AppException(string errorCode, string errorMessage) : base(errorMessage)
    {
        if (errorCode == null)
        {
            throw new ArgumentException("errorCode must be not null", nameof(errorCode));
        }
        ErrorCode = errorCode;
    }

    public string ErrorCode { get; init; }
}