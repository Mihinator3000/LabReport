using Microsoft.Office.Interop.Word;
using ReportGenerator.Core.Models;
using ReportGenerator.Core.Tools;
using Range = Microsoft.Office.Interop.Word.Range;

namespace ReportGenerator.Core.Creators;

public class WordCreator
{
    private const string TemplateWordPath = @"..\..\..\..\docs\report.docx";

    private readonly InputModel _inputData;

    private readonly FileInfo _fileInfo;

    public WordCreator(InputModel inputData)
    {
        _inputData = inputData ?? throw new ArgumentNullException(nameof(inputData));

        _fileInfo = File.Exists(TemplateWordPath)
            ? new FileInfo(TemplateWordPath)
            : throw new ReportGenException(
                "template file does not exist");
    }

    public void CreateReport()
    {
        var tagDictionary = _inputData.GetTagDictionary();

        var application = new Application();
        try
        {
            Document document = application.Documents.Open(_fileInfo.FullName);

            foreach (var (tag, replacementText) in tagDictionary)
            {
                Range range = document.Content;
                range.Find.Execute(tag);
                range = document.Range(range.Start, range.End);
                range.Text = replacementText;
            }

            string newFileName = _inputData.GetReportPath;
            application.ActiveDocument.SaveAs2(newFileName);
        }
        finally
        {
            application.ActiveDocument.Close();
            application.Quit();
            
        }
    }
}