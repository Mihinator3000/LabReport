using Microsoft.AspNetCore.Mvc.RazorPages;
using Octokit;
using Octokit.Internal;
using ReportGenerator.WebClient.Extensions;

namespace ReportGenerator.WebClient.Pages.Base;

public class BasePageModel : PageModel
{
    private static readonly ProductHeaderValue AuthInformation = new("AspNetCoreGitHubAuth");

    public User GitHubUser { get; protected set; }

    public string AccessToken { get; protected set; }

    public virtual async Task OnGetAsync()
    {
        if (User.Identity is null || !User.Identity.IsAuthenticated)
            return;

        AccessToken = await HttpContext.GetAccessTokenAsync();

        var credentialStore = new InMemoryCredentialStore(new Credentials(AccessToken));

        var gitHubClient = new GitHubClient(AuthInformation, credentialStore);

        GitHubUser = await gitHubClient.User.Current();
    }
}