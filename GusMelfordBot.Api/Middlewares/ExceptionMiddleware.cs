using System.Net;
using GusMelfordBot.Extensions.Exceptions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace GusMelfordBot.Api.Middlewares;

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
        } catch (Exception ex) {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Exception: {ExceptionMessage}", exception.Message);

        ExceptionDto error = GetException(exception);
        if (!context.Response.HasStarted) {
            context.Response.StatusCode = error.StatusCode;
        }
        
        context.Response.Headers.ContentType = new StringValues("application/json");
        return context.Response.WriteAsync(JsonConvert.SerializeObject(error));
    }

    private ExceptionDto GetException(Exception exception)
    {
        return exception switch
        {
            ConflictException conflictException => BuildError(conflictException, HttpStatusCode.Conflict),
            UnauthorizedException unauthorizedException => BuildError(unauthorizedException, HttpStatusCode.Unauthorized),
            _ => BuildError(exception, HttpStatusCode.BadRequest)
        };
    }

    private static ExceptionDto BuildError(Exception exception, HttpStatusCode httpStatusCode)
    {
        return new ExceptionDto
        {
            StatusCode = (int) httpStatusCode,
            Message = string.IsNullOrEmpty(exception.Message) ? "Something went wrong" : exception.Message
        };
    }
}