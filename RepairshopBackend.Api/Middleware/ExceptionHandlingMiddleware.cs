using System.Net;
using System.Text.Json;
using Serilog;

namespace RepairshopBackend.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.ForContext("CorrelationId", context.TraceIdentifier)
                .ForContext("Path", context.Request.Path.Value)
                .ForContext("Method", context.Request.Method)
                .Error(ex, "Excepción no controlada procesando {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                message = "Ocurrió un error inesperado en el servidor.",
                correlationId = context.TraceIdentifier,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}