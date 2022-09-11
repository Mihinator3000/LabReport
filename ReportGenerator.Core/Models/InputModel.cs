using ReportGenerator.Core.Providers;
using ReportGenerator.Core.Providers.CodeProviders;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Core.Models;

public class InputModel
{
    private readonly int _labNumber;
    private readonly string _fullName;
    private readonly string _groupName;

    private readonly ICodeProvider _codeProvider;
    
    private readonly DescriptionProvider _descriptionProvider;

    public InputModel(
        int labNumber,
        string fullName,
        string groupName,
        ICodeProvider codeProvider)
    {
        if (labNumber is < 1 or > 5)
            throw new ReportGenException("Incorrect lab number input");

        ArgumentNullException.ThrowIfNull(fullName);
        ArgumentNullException.ThrowIfNull(groupName);
        ArgumentNullException.ThrowIfNull(codeProvider);

        _labNumber = labNumber;
        _fullName = fullName;
        _groupName = groupName;
        _codeProvider = codeProvider;

        _descriptionProvider = new DescriptionProvider(labNumber);
    }

    public Dictionary<string, string> GetTagDictionary(string desc)
        => new() 
        {
            { "tagLABNUMBERtag", _labNumber.ToString() },
            { "tagFULLNAMEtag", _fullName },
            { "tagGROUPNAMEtag", _groupName },
            { "tagDESCRIPTIONtag", _descriptionProvider.GetLabDescription(desc) },
            { "tagSOURCECODEtag", _codeProvider.GetSourceCode() }
        };

    public string GetReportPath()
        => $"{GetReportName()}.docx";

    private string GetReportName()
    {
        string formattedFullName = _fullName
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty);

        return $"Отчет{_labNumber}Лабораторная{formattedFullName}{_groupName}";
    }
}