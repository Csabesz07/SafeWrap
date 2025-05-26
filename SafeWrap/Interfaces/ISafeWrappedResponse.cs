namespace SafeWrap.Interfaces;

internal interface ISafeWrappedResponse
{
    public string Message { get; init; }
    public string ErrorType { get; init; }
}
