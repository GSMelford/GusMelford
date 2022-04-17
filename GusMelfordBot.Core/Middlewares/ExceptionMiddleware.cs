using System.Net;
using GusMelfordBot.Core.Exception;
using Newtonsoft.Json;

namespace GusMelfordBot.Core.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext context) {
        try {
            await _next(context);
        } catch (System.Exception ex) {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, System.Exception exception)
    {
        _logger.LogError(exception, "Exception: {Message}", exception.Message);

        Error error = BuildError(exception);
        
        if (!context.Response.HasStarted) {
            context.Response.StatusCode = error.StatusCode;
        }

        return context.Response.WriteAsync(JsonConvert.SerializeObject(error));
    }

    private Error BuildError(System.Exception exception)
    {
        Error error = new Error();

        if (exception is WrongArgumentsException wrongArgumentsException)
        {
            error.StatusCode = (int) HttpStatusCode.BadRequest;
            error.Message = wrongArgumentsException.Message;
        }

        return error;
    }
}