using System.Net;

namespace ReportGenerator.Core.Tools;

public class GitHubException : Exception
{
    public GitHubException(HttpStatusCode statusCode)
        => StatusCode = statusCode;
    
    public HttpStatusCode StatusCode { get; }
}