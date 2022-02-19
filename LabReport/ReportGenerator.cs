using LabReport.Services;

namespace LabReport;

public class ReportGenerator
{
    private readonly string _reportPath;
    private readonly IoService _ioService = new ();

    private StreamWriter _writer;

    public ReportGenerator() => _reportPath = Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.Desktop), "report.txt");

    public void Create()
    {
        if (File.Exists(_reportPath))
        {
            _ioService.PrintError($"file {_reportPath} already exist");
            return;
        }
        
        try
        {
            _writer = new StreamWriter(_reportPath);
            
            string labFolder = _ioService.GetPathFolder();

            if (!Directory.Exists(BackupService.BackupFolderName))
                CheckDifference(new DirectoryInfo(labFolder));
        }
        catch (Exception e)
        {
            _ioService.PrintError(e.Message);
        }
        finally
        {
            _writer.Close();
        }
    }

    private void CheckDifference(DirectoryInfo source)
    {
        foreach (FileInfo fileInfo in source.GetFiles())
            if (IfJavaFile(fileInfo))
                AddedFile(fileInfo);

        foreach (DirectoryInfo subDirectoryInfo in source.GetDirectories())
            CheckDifference(subDirectoryInfo);
    }

    private static bool IfJavaFile(FileInfo fileInfo) =>
        Path.GetExtension(fileInfo.Name) is ".java";

    private void AddedFile(FileInfo fileInfo) => 
        WriteFile($"+ {fileInfo.Name}", fileInfo);

    private void WriteFile(string title, FileInfo fileInfo)
    {
        _writer.WriteLine(title);
        _writer.WriteLine();
        _writer.WriteLine(File.ReadAllText(fileInfo.FullName));
        _writer.WriteLine();
        _writer.WriteLine();
    }
}