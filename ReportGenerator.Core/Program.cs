using ReportGenerator.Core.Creators;
using ReportGenerator.Core.Models;
using ReportGenerator.Core.Providers.CodeProviders;
using ReportGenerator.Core.Tools;

try
{
    for (int i = 1; i <= 5; i++)
    {
        var inputData = new InputModelBuilder()
            .SetLabNumber(i)
            .SetFullName("Кошкин Михаил")
            .SetGroupName("M32051")
            .SetReportFolderPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
            .SetCodeProvider(new LocalCodeProvider(@"C:\Users\Koshkin\Desktop\Banks"))
            .Build();

        new WordCreator(inputData).CreateReport();
    }
}
catch (ReportGenException e)
{
    Console.WriteLine(e.Message);
}
