using Microsoft.AspNetCore.Mvc;
using SafeWrap;
using SafeWrap.Protocols;
using System.Net.Mime;

namespace Examples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        /// <summary>
        /// Gets weather forecasts with synchronous error handling using SafeWrap.Execute
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET /WeatherForecast/safe
        /// </remarks>
        /// <response code="200">Returns the list of weather forecasts</response>
        /// <response code="400">If a random error occurs during execution</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("safe", Name = "GetWeatherForecastSafe")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status500InternalServerError)]
        public ActionResult GetSafe() =>        
            new SafeWrap<IEnumerable<WeatherForecast>>().Execute(
                () =>
                {
                    if (Random.Shared.Next(2) == 0)
                        throw new InvalidOperationException("Random error occurred");

                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    })
                    .ToArray();
                },
                (typeof(InvalidOperationException), StatusCodes.Status400BadRequest)
            );
        
        /// <summary>
        /// Gets weather forecasts asynchronously with error handling using SafeWrap.ExecuteAsync
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET /WeatherForecast/safe/async
        /// 
        /// This endpoint includes a simulated delay of 1 second to demonstrate async behavior.
        /// </remarks>
        /// <response code="200">Returns the list of weather forecasts</response>
        /// <response code="400">If the request contains invalid parameters or operation is invalid</response>
        /// <response code="408">If the operation times out</response>
        /// <response code="429">If too many requests are made</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("safe/async", Name = "GetWeatherForecastSafeAsync")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status408RequestTimeout)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> GetSafeAsync() =>        
            await new SafeWrap<IEnumerable<WeatherForecast>>().ExecuteAsync(
                async () =>
                {
                    // Simulate potential timeout scenario
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                    try 
                    {
                        await Task.Delay(1000, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new TimeoutException("Operation timed out while retrieving weather forecast");
                    }

                    // Simulate rate limiting
                    if (Random.Shared.Next(10) == 0)
                        throw new InvalidOperationException("Rate limit exceeded") { Data = { ["RateLimit"] = true } };

                    // Simulate validation error
                    if (Random.Shared.Next(10) == 1)
                        throw new ArgumentException("Invalid temperature range specified");

                    // Simulate business logic error
                    if (Random.Shared.Next(10) == 2)
                        throw new InvalidOperationException("Weather service is currently unavailable");

                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    })
                    .ToArray();
                },
                (typeof(TimeoutException), StatusCodes.Status408RequestTimeout),
                (typeof(ArgumentException), StatusCodes.Status400BadRequest),
                (typeof(InvalidOperationException), StatusCodes.Status400BadRequest)
            );
        
        /// <summary>
        /// Gets weather forecasts using SafeWrap's implicit operator conversion
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     GET /WeatherForecast/safe/implicit
        /// 
        /// This endpoint demonstrates the implicit operator conversion from SafeWrap to ActionResult.
        /// </remarks>
        /// <response code="200">Returns the list of weather forecasts</response>
        /// <response code="400">If a random error occurs during execution</response>
        /// <response code="500">If an unexpected error occurs</response>
        [HttpGet("safe/implicit", Name = "GetWeatherForecastSafeImplicit")]
        [ProducesResponseType(typeof(IEnumerable<WeatherForecast>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status500InternalServerError)]
        public ActionResult GetSafeImplicit() =>
          new SafeWrap<IEnumerable<WeatherForecast>>(
                () =>
                {
                    if (Random.Shared.Next(2) == 0)
                        throw new InvalidOperationException("Random error in implicit operator example");

                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                    })
                    .ToArray();
                },
                (typeof(InvalidOperationException), StatusCodes.Status400BadRequest)
          );
    }
}
