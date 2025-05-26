
namespace SafeWrap;

public sealed partial class SafeWrap<TReturnValue>
{
    public SafeWrap() { }
    public SafeWrap(Func<TReturnValue> initialMethod, params (Type exceptionType, int statusCode)[] exceptions)
    {
        _exceptions = exceptions;

        try
        {
            _result = initialMethod();
            _success = true;
        }
        catch (Exception ex)
        {
            _handledException = HandleException(ex);
        }
    }
}
