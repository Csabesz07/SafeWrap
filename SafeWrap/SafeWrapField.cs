
using Microsoft.AspNetCore.Mvc;

namespace SafeWrap;

public sealed partial class SafeWrap<TReturnValue>
{
    private TReturnValue? _result;
    private bool _success = false;
    private ObjectResult? _handledException = null;
    private (Type exceptionType, int statusCode)[] _exceptions = [];
}
