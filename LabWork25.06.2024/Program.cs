var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int Fibonacci(int n)
{
    if (n < 0 || n > 40) throw new ArgumentOutOfRangeException(nameof(n), "Index must be between 0 and 40.");
    if (n == 0) return 0;
    if (n == 1) return 1;

    int a = 0;
    int b = 1;
    for (int i = 2; i <= n; i++)
    {
        int temp = a + b;
        a = b;
        b = temp;
    }
    return b;
}

app.UseTokenValidation();


app.MapGet("/fibonacci", (HttpRequest request) =>
{
    if (request.Query.TryGetValue("index", out var indexValue) && int.TryParse(indexValue, out int index))
    {
        try
        {
            int result = Fibonacci(index);
            return Results.Ok(result);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }
    else
    {
        return Results.BadRequest("Invalid or missing 'index' parameter. It must be an integer between 0 and 40.");
    }
});

app.Run();

public class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Query.TryGetValue("token", out var token) || token != "vadym")
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context);
    }
}

public static class TokenValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenValidationMiddleware>();
    }
}

