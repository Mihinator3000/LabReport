using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace ReportGenerator.WebClient.Extensions;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddGithubAuth(
        this AuthenticationBuilder authBuilder,
        IConfiguration configuration)
    {
        return authBuilder.AddOAuth("GitHub", o =>
         {
             o.CorrelationCookie.HttpOnly = true;
             o.CorrelationCookie.SameSite = SameSiteMode.Lax;

             o.Scope.Add("repo");

             o.ClientId = configuration["GitHub:ClientId"];
             o.ClientSecret = configuration["GitHub:ClientSecret"];
             o.CallbackPath = new PathString("/github-oauth");

             o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
             o.TokenEndpoint = "https://github.com/login/oauth/access_token";
             o.UserInformationEndpoint = "https://api.github.com/user";

             o.SaveTokens = true;

             o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
             o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
             o.ClaimActions.MapJsonKey("urn:github:login", "login");
             o.ClaimActions.MapJsonKey("urn:github:url", "html_url");
             o.ClaimActions.MapJsonKey("urn:github:avatar", "avatar_url");

             o.Events = new OAuthEvents
             {
                 OnCreatingTicket = OnCreatingTicketHandler
             };
         });
    }

    private static async Task OnCreatingTicketHandler(OAuthCreatingTicketContext context)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
        var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        context.RunClaimActions(json.RootElement);
    }
}