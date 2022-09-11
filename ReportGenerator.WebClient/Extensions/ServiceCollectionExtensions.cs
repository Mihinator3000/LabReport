using Microsoft.AspNetCore.Authentication.Cookies;

namespace ReportGenerator.WebClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGithubAuth(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddAuthentication(o => 
            { 
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; 
                o.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddGithubAuth(configuration);

        return serviceCollection;
    }
}