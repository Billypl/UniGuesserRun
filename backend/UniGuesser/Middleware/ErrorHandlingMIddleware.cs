using Microsoft.EntityFrameworkCore;
using PartyGame.Extensions.Exceptions;

namespace PartyGame.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (DbUpdateException ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Database update error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (ForbidException ex)
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync($"The resources are forbidden: {ex.Message}");
            }
            catch (NotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"Resource was not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Unexpected error occurred: {ex.Message}");
            }
        }
    }
}