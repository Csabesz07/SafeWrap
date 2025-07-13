<img src="./Assets/Images/safe_wrap_logo_transparent.png" alt="Logo" width="150" style="display:block;margin:auto;" />

# SafeWrap

SafeWrap is a lightweight exception handling library for ASP.NET Core applications that provides a clean and consistent way to handle exceptions and convert them to appropriate HTTP responses.

## Features

- 🛡️ Type-safe exception handling wrapper
- 🎯 Automatic HTTP status code mapping
- 🔄 Support for both synchronous and asynchronous operations
- 📦 Consistent error response format
- 🔌 Implicit conversion to ActionResult
- 🚀 Zero-configuration defaults with customization options

## Installation

### Package Manager Console

`` Install-Package SafeWrap ``

### .NET CLI

`` dotnet add package SafeWrap ``

### Package Reference

`` <PackageReference Include="SafeWrap" Version="8.1.0" /> ``

> **Note**: This package is not yet available on NuGet. Currently, you can reference it directly in your solution:
`` <ItemGroup> <ProjectReference Include="..\SafeWrap\SafeWrap.csproj" /> </ItemGroup> ``

## Usage

### Basic Exception Handling

```csharp
public ActionResult GetData() => 
	new SafeWrap<IEnumerable<DataItem>>().Execute( () => 
		{ 
			// Your code that might throw exceptions 
			return _service.GetItems(); 
		}, 
		(typeof(InvalidOperationException), StatusCodes.Status400BadRequest) 
	);
```

### Async Operations

```csharp
public async Task<ActionResult> GetDataAsync() => 
	await new SafeWrap<IEnumerable<DataItem>>().ExecuteAsync( async () => 
		{ 
			// Your async code 
			return await _service.GetItemsAsync(); 
		}, 
		(typeof(InvalidOperationException), StatusCodes.Status400BadRequest) 
	);
```

### Using Implicit Operator

```csharp
public ActionResult GetData() => 
	new SafeWrap<IEnumerable<DataItem>>( () => 
		_service.GetItems(), 
		(typeof(InvalidOperationException), StatusCodes.Status400BadRequest) 
	);
```


## Error Response Format

When an exception occurs, SafeWrap returns a consistent error response:
```json
{ "message": "The error message", "errorType": "ExceptionTypeName" }
```

## Advanced Example

```csharp
[HttpGet("data")] 
[ProducesResponseType(typeof(IEnumerable<DataItem>), StatusCodes.Status200OK)] 
[ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status400BadRequest)] 
[ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status408RequestTimeout)] 
[ProducesResponseType(typeof(SafeWrappedResponse), StatusCodes.Status500InternalServerError)] 
public async Task<ActionResult> GetDataAsync() => 
	await new SafeWrap<IEnumerable<DataItem>>().ExecuteAsync( async () => 
		{ 
			using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); 
			return await _service.GetItemsAsync(cts.Token); 
		}, 
		(typeof(TimeoutException), StatusCodes.Status408RequestTimeout), 
		(typeof(ArgumentException), StatusCodes.Status400BadRequest), 
		(typeof(InvalidOperationException), StatusCodes.Status400BadRequest) 
	);
```


## Exception Mapping

SafeWrap maps exceptions to HTTP status codes based on the configuration provided:

- Configured exceptions are mapped to their specified status codes
- Uncaught exceptions default to 500 Internal Server Error
- Multiple exception types can be mapped to the same status code

## Best Practices

1. Always specify expected exceptions and their status codes
2. Use appropriate HTTP status codes for different error scenarios
3. Keep error messages user-friendly but informative
4. Use async methods for async operations
5. Consider adding timeout handling for long-running operations

## License

MIT License. See LICENSE file for details.