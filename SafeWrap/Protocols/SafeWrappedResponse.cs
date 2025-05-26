
using SafeWrap.Interfaces;

namespace SafeWrap.Protocols;

public class SafeWrappedResponse(string message, string errorType) : ISafeWrappedResponse
{
    public string Message { get; init; } = message;
    public string ErrorType { get; init; } = errorType;
}
