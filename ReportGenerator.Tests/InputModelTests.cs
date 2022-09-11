using Moq;
using NUnit.Framework;
using ReportGenerator.Core.Models;
using ReportGenerator.Core.Providers.CodeProviders;
using ReportGenerator.Core.Tools;

namespace ReportGenerator.Tests;

public class InputModelTests
{
    private readonly ICodeProvider _codeProvider;

    public InputModelTests()
    {
        _codeProvider = new Mock<ICodeProvider>().Object;
    }

    [TestCase(0)]
    [TestCase(6)]
    public void InvalidLabNumber_ThrowReportGenException(int labNumber)
    {
        Assert.Throws<ReportGenException>(() =>
        {
            _ = new InputModel(labNumber, string.Empty, string.Empty, _codeProvider);
        });
    }

    [TestCase(1, "������ ������", "M32051")]
    [TestCase(2, "������ ������", "M33051")]
    [TestCase(3, "������ �����", "M34051")]
    public void ConstructReportPath(int labNumber, string fullName, string groupName)
    {
        var inputModel = new InputModel(labNumber, fullName, groupName, _codeProvider);

        string formattedFullName = fullName
            .Replace(" ", string.Empty)
            .Replace(".", string.Empty);

        string expected = $"�����{labNumber}������������{formattedFullName}{groupName}.docx";

        string actual = inputModel.GetReportPath();

        Assert.That(expected, Is.EqualTo(actual));
    }
}