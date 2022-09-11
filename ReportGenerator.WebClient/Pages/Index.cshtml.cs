using System.Net;
using Microsoft.AspNetCore.Mvc;
using ReportGenerator.Core.Creators;
using ReportGenerator.Core.Models;
using ReportGenerator.Core.Providers.CodeProviders;
using ReportGenerator.Core.Tools;
using ReportGenerator.WebClient.Extensions;
using ReportGenerator.WebClient.Pages.Base;

namespace ReportGenerator.WebClient.Pages
{
    public class IndexModel : BasePageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public string GithubUserName { get; private set; }

        public override async Task OnGetAsync()
        {
            await base.OnGetAsync();
            if (User.Identity is not null && User.Identity.IsAuthenticated)
                GithubUserName = GitHubUser.Login;
        }

        public async Task<IActionResult> OnPostDownloadWordAsync(int labNumber, string githubUserName, string fullName, string groupName, int pullNumber)
        {
            if (pullNumber is 0)
                pullNumber = labNumber;

            AccessToken = await HttpContext.GetAccessTokenAsync();

            try
            {
                string desc = await System.IO.File.ReadAllTextAsync("wwwroot/docs/lab_description.txt");
                byte[] byteArray = await System.IO.File.ReadAllBytesAsync("wwwroot/docs/report.docx");

                var githubProvider = new GithubCodeProvider(AccessToken, githubUserName, pullNumber);

                InputModel inputModel = new InputModelBuilder()
                    .SetLabNumber(labNumber)
                    .SetFullName(fullName)
                    .SetGroupName(groupName)
                    .SetCodeProvider(githubProvider)
                    .Build();

                byte[] reportByteArray = new WordCreator(inputModel, desc).CreateReport(byteArray).ToArray();

                return File(
                    reportByteArray,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    inputModel.GetReportPath);
            }
            catch (ReportGenException e)
            {
                return BadRequest($"Internal error: {e.Message}");
            }
            catch (GitHubException e)
            {
                return BadRequest(e.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Invalid github token",
                    HttpStatusCode.NotFound => "Could not access pull request",
                    _ => $"Github error: {e.StatusCode}"
                });
            }
            catch (Exception)
            {
                return BadRequest("Internal error");
            }
        }
    }
}