using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ReportGenerator.WebClient.Pages.Base;

namespace ReportGenerator.WebClient.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : BasePageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public override async Task OnGetAsync()
        {
            await base.OnGetAsync();
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}