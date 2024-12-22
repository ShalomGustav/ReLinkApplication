using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ReLinkApplication.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception ex) 
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound, 
            ArgumentNullException => StatusCodes.Status400BadRequest, 
            InvalidOperationException => StatusCodes.Status400BadRequest, 
            FormatException => StatusCodes.Status400BadRequest, 
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized, 
            DbUpdateException => StatusCodes.Status500InternalServerError, 
            NotImplementedException => StatusCodes.Status501NotImplemented, 
            _ => StatusCodes.Status500InternalServerError 
        };

        context.Response.StatusCode = statusCode;

        var errorDetails = new
        {
            error = exception.Message,
            StatusCode = statusCode,
            details = exception.InnerException?.Message
        };

        var result = JsonConvert.SerializeObject(errorDetails);
        return context.Response.WriteAsync(result);
    }
}
