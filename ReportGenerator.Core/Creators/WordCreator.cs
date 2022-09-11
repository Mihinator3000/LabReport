using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportGenerator.Core.Models;

namespace ReportGenerator.Core.Creators;

public class WordCreator
{
    private readonly Dictionary<string, string> _tagDictionary;

    public WordCreator(InputModel inputData, string desc)
    {
        ArgumentNullException.ThrowIfNull(inputData);

        _tagDictionary = inputData.GetTagDictionary(desc);
    }

    public MemoryStream CreateReport(byte[] byteArray)
    {
        using var memoryStream = new MemoryStream();
        memoryStream.Write(byteArray, 0, byteArray.Length);

        using (var wordDocument = WordprocessingDocument.Open(memoryStream, true))
            foreach (var (tag, replacementText) in _tagDictionary)
                ReplaceTag(wordDocument, tag, replacementText);

        return memoryStream;
    }

    private static void ReplaceTag(WordprocessingDocument wordDocument, string tag, string replacementText)
    {
        var body = wordDocument.MainDocumentPart.Document.Body;

        foreach (var paragraph in body.Elements<Paragraph>())
        foreach (var run in paragraph.Elements<Run>())
        foreach (var text in run.Elements<Text>())
        {
            if (!text.Text.Contains(tag))
                continue;
            
            string[] replacementLines = replacementText.Split('\n');

            text.Text = text.Text.Replace(tag, replacementLines[0]);

            if (replacementLines.Length is not 1)
                AppendParagraph(paragraph, run, replacementLines[1..]);

            return;
        }
    }

    private static void AppendParagraph(Paragraph paragraph, Run run, IEnumerable<string> replacementLines)
    {
        run.AppendChild(new Paragraph());

        foreach (string replacementLine in replacementLines)
        {
            var newText = new Text
            {
                Text = replacementLine,
                Space = SpaceProcessingModeValues.Preserve
            };

            var newRun = new Run(newText)
            {
                RunProperties = (RunProperties)run
                    .RunProperties.CloneNode(true)
            };

            var newParagraph = new Paragraph(newRun)
            {
                ParagraphProperties = (ParagraphProperties)paragraph
                    .ParagraphProperties.CloneNode(true)
            };

            run.AppendChild(newParagraph);
        }
    }
}