using System.Text;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Core.Providers.CodeProviders;

public class LocalCodeProvider : ICodeProvider
{
    private readonly string _localFolderPath;

    private readonly DateTime? _previousReportTime;

    private readonly StringBuilder _stringBuilder;

    public LocalCodeProvider(string localFolderPath)
    {
        if (!Directory.Exists(localFolderPath))
            throw new ReportGenException("directory does not exist");

        _localFolderPath = localFolderPath;

        _stringBuilder = new StringBuilder();
    }

    public LocalCodeProvider(string localFolderPath, DateTime previousReportTime)
        : this(localFolderPath)
    {
        _previousReportTime = previousReportTime;
    }

    public string GetSourceCode()
    {
        RecordDifference(new DirectoryInfo(_localFolderPath));
        return _stringBuilder.ToString();
    }

    private void RecordDifference(DirectoryInfo source)
    {
        foreach (FileInfo fileInfo in source.GetFiles())
        {
            if (IsNotJavaFile(fileInfo))
                continue;

            if (IsNewFile(fileInfo))
                RecordFileInfo("+", fileInfo);
            else if (IsChangedFile(fileInfo))
                RecordFileInfo("+-", fileInfo);
        }

        foreach (DirectoryInfo subDirectoryInfo in source.GetDirectories())
            RecordDifference(subDirectoryInfo);
    }

    private void RecordFileInfo(string titleSymbol, FileInfo fileInfo)
        => _stringBuilder
            .AppendLine($"{titleSymbol} {fileInfo.Name}")
            .AppendLine()
            .AppendLine(File.ReadAllText(fileInfo.FullName))
            .AppendLine()
            .AppendLine();

    private static bool IsNotJavaFile(FileInfo fileInfo)
        => Path.GetExtension(fileInfo.Name) is not ".java";

    private bool IsChangedFile(FileInfo fileInfo)
        => _previousReportTime is null || fileInfo.LastWriteTime > _previousReportTime;

    private bool IsNewFile(FileInfo fileInfo)
        => _previousReportTime is null || fileInfo.CreationTime > _previousReportTime;
 }