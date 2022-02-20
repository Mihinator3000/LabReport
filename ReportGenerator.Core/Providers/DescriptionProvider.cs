using System.Text.RegularExpressions;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Core.Providers;

public class DescriptionProvider
{
    private const string DescriptionPath = @"..\..\..\..\docs\lab_description.txt";

    private readonly int _labNumber;

    public DescriptionProvider(int labNumber)
        => _labNumber = labNumber;

    public string GetLabDescription()
    {
        if (!File.Exists(DescriptionPath))
            throw new ReportGenException(
                "description file does not exist");
        
        string labsDescription = File.ReadAllText(DescriptionPath);

        var match = Regex.Match(labsDescription);

        if (!match.Success)
            throw new ReportGenException(
                "invalid description");

        return match.Groups[1].Value;
    }

    private Regex Regex => new ($"{TagBorder}(.*){TagBorder}", RegexOptions.Singleline);

    private string TagBorder => $"<lab_{_labNumber}>";
}