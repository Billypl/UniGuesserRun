using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UniGuesser.API.Middleware.Exceptions;

namespace UniGuesser.API.Middleware
{
    // Middleware
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (BaseException ex)
            {
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Title = ex.Message,
                    Status = ex.StatusCode,
                    Instance = context.Request.Path
                };

                var json = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(json);
            }
            catch (DbUpdateException ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/problem+json";
                var problemDetails = new ProblemDetails
                {
                    Title = "Database update error",
                    Status = 500,
                    Detail = ex.InnerException?.Message ?? ex.Message,
                    Instance = context.Request.Path
                };
                var json = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(json);
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/problem+json";
                var problemDetails = new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = 401,
                    Instance = context.Request.Path
                };
                var json = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($" Unexpected error: {ex}");
            }
        }
    }
}