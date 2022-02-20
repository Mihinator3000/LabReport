using ReportGenerator.Core.Providers.CodeProviders;

namespace ReportGenerator.Core.Models;

public class InputModelBuilder
{
    private int _labNumber;
    private string _fullName;
    private string _groupName;

    private string _reportFolderPath;
    
    private ICodeProvider _codeProvider;
    
    public InputModelBuilder SetLabNumber(int labNumber)
    {
        _labNumber = labNumber;
        return this;
    }

    public InputModelBuilder SetFullName(string fullName)
    {
        _fullName = fullName;
        return this;
    }

    public InputModelBuilder SetGroupName(string groupName)
    {
        _groupName = groupName;
        return this;
    }

    public InputModelBuilder SetReportFolderPath(string reportFolderPath)
    {
        _reportFolderPath = reportFolderPath;
        return this;
    }

    public InputModelBuilder SetCodeProvider(ICodeProvider codeProvider)
    {
        _codeProvider = codeProvider;
        return this;
    }

    public InputModel Build()
        => new(
            _labNumber, 
            _fullName,
            _groupName,
            _reportFolderPath,
            _codeProvider);
}