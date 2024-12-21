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
            KeyNotFoundException => StatusCodes.Status404NotFound, // Ссылка не найдена
            ArgumentNullException => StatusCodes.Status400BadRequest, // Некорректные данные
            InvalidOperationException => StatusCodes.Status400BadRequest, // Ошибка бизнес-логики
            FormatException => StatusCodes.Status400BadRequest, // Некорректный формат
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized, // Не авторизован
            DbUpdateException => StatusCodes.Status500InternalServerError, // Ошибка базы данных
            NotImplementedException => StatusCodes.Status501NotImplemented, // Функциональность отсутствует
            _ => StatusCodes.Status500InternalServerError // Неизвестная ошибка
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
