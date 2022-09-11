using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Core.Providers.CodeProviders;

public class GithubCodeProvider : ICodeProvider
{
    private readonly string _githubToken;

    private readonly string _githubUserName;
    private readonly int _pullNumber;
    
    public GithubCodeProvider(string gitHubToken, string githubUserName, int pullNumber)
    {
        _githubToken = gitHubToken;
        _githubUserName = githubUserName;
        _pullNumber = pullNumber;
    }

    public string GetSourceCode()
    {
        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", _githubToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3.diff"));
        client.DefaultRequestHeaders.UserAgent.TryParseAdd(_githubUserName);

        var response = client.GetAsync(GetPullUrl).Result;
        
        if (!response.IsSuccessStatusCode)
            throw new GitHubException(response.StatusCode);

        string responseContent = response.Content.ReadAsStringAsync().Result;
        return NormalizeGithubOutput(responseContent);
    }

    public static string NormalizeGithubOutput(string s)
    {
        s = s.Replace("\n\\ No newline at end of file", null);
        s = Regex.Replace(s, @"(?m)^diff --git (.+?\n){0,5}--- a/(.+?)\n\+\+\+ /dev/null\n@@ -.+?\+.+? @@\n(-.*?\n)+(-.*)?", "\nУдален файл $2\n");
        s = Regex.Replace(s, @"(?m)^diff --git(.+?\n){0,5}rename from (.+?)\nrename to (.+?)\n(?s-m).+?@@ -.+?\+.+?@@", "\n\nИзменен файл $2 -> $3:\n");
        s = Regex.Replace(s, @"(?m)^diff --git(.+?\n){0,5}--- /dev/null\n\+\+\+ b/(.+?)\n@@ -.+?\+.+? @@", "\n\nДобавлен файл $2:\n");
        s = RemovePlusesAfterAddedFiles(s);
        s = Regex.Replace(s, @"(?m)^diff --git(?s-m).+?\+\+\+ b/(.+?)\n@@ -.+?\+.+? @@", "\n\nИзменен файл $1:\n");
        s = Regex.Replace(s, @"(?m)^diff --git(.+?\n){0,5}Binary files /dev/null and b/(.+?) differ", "\nДобавлен файл $2\n");
        s = Regex.Replace(s, @"(?m)^diff --git(?s-m).+?Binary.+?b/(.+?) differ", "\nИзменен файл $1\n");
        s = Regex.Replace(s, @"(?m)^@@ -.+?\+.+? @@.+?", string.Empty);
        return s.Replace("�", string.Empty).Trim('\n');
    }

    private static string RemovePlusesAfterAddedFiles(string s)
    {
        var addRegex = new Regex(@"Добавлен файл (.+?):");
        
        var sb = new StringBuilder();

        bool isLineAfterAddition = false;

        foreach (string line in s.Split('\n'))
        {
            if (line is not "" && isLineAfterAddition)
            {
                if (line[0] is '+')
                {
                    sb.AppendLine(line[1..]);
                    continue;
                }

                isLineAfterAddition = false;
            }

            if (addRegex.Match(line).Success)
                isLineAfterAddition = true;

            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    private string GetPullUrl => $"https://api.github.com/repos/is-tech-y24-2/{_githubUserName}/pulls/{_pullNumber}";
}
