using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fym.Api.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Registrar el error detallado internamente para auditoría
        _logger.LogError(exception, "Ha ocurrido una excepción no controlada: {Message}", exception.Message);

        // 2. Mapear el tipo de excepción al código HTTP correcto (por ahora genérico 500)
        var statusCode = HttpStatusCode.InternalServerError;
        var title = "Error Interno del Servidor";
        var detail = exception.Message;

        // Aquí capturaremos excepciones personalizadas más adelante (ej. Validaciones = 400, NotFound = 404)
        if (exception is UnauthorizedAccessException)
        {
            statusCode = HttpStatusCode.Unauthorized;
            title = "No Autorizado";
        }

        // 3. Construir la estructura estandarizada RFC 7807
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{(int)statusCode}",
            Instance = httpContext.Request.Path
        };

        // 4. Configurar y enviar la respuesta en formato JSON
        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retornamos true para indicar a .NET que la excepción ya fue manejada con éxito
        return true;
    }
}