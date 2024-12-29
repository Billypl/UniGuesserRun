using MongoDB.Driver;

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
            catch (MongoWriteException ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Database write error: {ex.Message}");
            }
            catch (MongoConnectionException ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Database connection error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsync($"Resource not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Unexpected error occurred: {ex.Message}");
            }

        }
    }
}