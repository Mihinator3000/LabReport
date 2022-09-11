using System.Text.RegularExpressions;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Core.Providers;

public class DescriptionProvider
{
    private readonly int _labNumber;

    public DescriptionProvider(int labNumber)
        => _labNumber = labNumber;

    public string GetLabDescription(string desc)
    {
        var match = Regex.Match(desc);

        if (!match.Success)
            throw new ReportGenException("Invalid description");

        return match.Groups[1].Value;
    }

    private Regex Regex => new ($"{TagBorder}(.*){TagBorder}", RegexOptions.Singleline);

    private string TagBorder => $"<lab_{_labNumber}>";
}