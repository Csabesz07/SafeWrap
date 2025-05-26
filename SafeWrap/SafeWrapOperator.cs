
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SafeWrap;

public sealed partial class SafeWrap<TReturnValue>
{
    public static implicit operator ActionResult(SafeWrap<TReturnValue> safeWrap)
    {
        if (safeWrap._success)
        {
            return new ObjectResult(safeWrap._result) { StatusCode = StatusCodes.Status200OK };
        }
        else
        {
            return safeWrap._handledException ?? new ObjectResult(StatusCodes.Status500InternalServerError);
        }
    }
}
