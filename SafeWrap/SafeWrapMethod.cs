
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SafeWrap.Protocols;

namespace SafeWrap;

public sealed partial class SafeWrap<TReturnValue>
{
    public async Task<ActionResult> ExecuteAsync(Func<Task<TReturnValue>> initialMethodAsync, params (Type exceptionType, int statusCode)[] exceptions)
    {
        _exceptions = [..exceptions];

        try
        {
            _result = await initialMethodAsync();
            _success = true;

            return new ObjectResult(_result);
        }
        catch (Exception ex)
        {
            _handledException = HandleException(ex);
            return _handledException ?? new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    public ActionResult Execute(Func<TReturnValue> initialMethod, params (Type exceptionType, int statusCode)[] exceptions)
    {
        _exceptions = [..exceptions];
        try
        {
            _result = initialMethod();
            _success = true;

            return new ObjectResult(_result) { StatusCode = StatusCodes.Status200OK};
        }
        catch (Exception ex)
        {
            _handledException = HandleException(ex);
            return _handledException ?? new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }

    private ObjectResult HandleException(Exception ex)
    {
        var originalExceptionType = ex.GetType();
        var (exceptionType, statusCode) = 
            _exceptions.FirstOrDefault(e =>
                originalExceptionType.IsAssignableFrom(e.exceptionType)
            );

        SafeWrappedResponse errorPayload = new (ex.Message, originalExceptionType.Name);

        return new ObjectResult(errorPayload) { StatusCode = exceptionType is not null ? statusCode : StatusCodes.Status500InternalServerError };
    }
}
