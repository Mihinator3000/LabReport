using ReportGenerator.Core.Providers;
using ReportGenerator.Core.Providers.CodeProviders;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Core.Models;

public class InputModel
{
    private readonly int _labNumber;
    private readonly string _fullName;
    private readonly string _groupName;

    private readonly string _reportFolderPath;

    private readonly ICodeProvider _codeProvider;
    
    private readonly DescriptionProvider _descriptionProvider;

    internal InputModel(
        int labNumber,
        string fullName,
        string groupName,
        string reportFolderPath,
        ICodeProvider codeProvider)
    {
        _labNumber = labNumber is >= 1 and <= 5
            ? labNumber
            : throw new ReportGenException(
                "Incorrect lab number input");

        _fullName = fullName ?? throw new ArgumentNullException(nameof(fullName));

        _groupName = groupName ?? throw new ArgumentNullException(nameof(groupName));

        _reportFolderPath = Directory.Exists(reportFolderPath)
            ? reportFolderPath
            : throw new ReportGenException(
                "Incorrect report folder path");

        _codeProvider = codeProvider ?? throw new ArgumentNullException(nameof(codeProvider));

        _descriptionProvider = new DescriptionProvider(labNumber);
    }

    public Dictionary<string, string> GetTagDictionary()
        => new()
        {
            { "tagLABNUMBERtag", _labNumber.ToString() },
            { "tagFULLNAMEtag", _fullName },
            { "tagGROUPNAMEtag", _groupName },
            { "tagDESCRIPTIONtag", _descriptionProvider.GetLabDescription() },
            { "tagSOURCECODEtag", _codeProvider.GetSourceCode() }
        };

    public string GetReportPath
        => Path.Combine(_reportFolderPath, $"{GetReportName}.docx");

    private string GetReportName
        => @$"Отчет{_labNumber}Лабораторная{_fullName
            .Replace(" ", null)
            .Replace(".", null)}{_groupName}";
}