using Microsoft.AspNetCore.Authentication;

namespace ReportGenerator.WebClient.Extensions;

public static class HttpContextExtensions
{
    public static Task<string> GetAccessTokenAsync(this HttpContext context)
        => context.GetTokenAsync("access_token");
}