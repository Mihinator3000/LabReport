using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportGenerator.Core.Models;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Core.Creators;

public class WordCreator
{
    private const string TemplateWordPath = @"..\..\..\..\docs\report.docx";

    private readonly InputModel _inputData;

    public WordCreator(InputModel inputData)
        => _inputData = inputData ?? throw new ArgumentNullException(nameof(inputData));


    public void CreateReport()
    {
        if (!File.Exists(TemplateWordPath))
            throw new ReportGenException("template file does not exist");

        var tagDictionary = _inputData.GetTagDictionary();

        if (File.Exists(_inputData.GetReportPath))
            File.Delete(_inputData.GetReportPath);

        string reportPath = _inputData.GetReportPath;

        File.Copy(TemplateWordPath, reportPath);

        using var wordDocument = WordprocessingDocument.Open(reportPath, true);

        foreach (var (tag, replacementText) in tagDictionary)
            ReplaceTag(wordDocument, tag, replacementText);
    }

    private static void ReplaceTag(WordprocessingDocument wordDocument, string tag, string replacementText)
    {
        var body = wordDocument.MainDocumentPart.Document.Body;

        foreach (var paragraph in body.Elements<Paragraph>())
        foreach (var run in paragraph.Elements<Run>())
        foreach (var text in run.Elements<Text>())
        {
            Console.WriteLine(text.Text);

            if (!text.Text.Contains(tag))
                continue;
            
            string[] replacementLines = replacementText.Split('\n');

            text.Text = text.Text.Replace(tag, replacementLines[0]);

            if (replacementLines.Length is 1)
                return;

            foreach (string replacementLine in replacementLines[1..])
            {
                var newRun = new Run(new Text(replacementLine))
                {
                    RunProperties = (RunProperties) run
                        .RunProperties.CloneNode(true)
                };

                var newParagraph = new Paragraph(newRun)
                {
                    ParagraphProperties = (ParagraphProperties) paragraph
                        .ParagraphProperties.CloneNode(true)
                };

                run.AppendChild(newParagraph);
            }

            return;
        }
    }
}