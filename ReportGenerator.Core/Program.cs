using ReportGenerator.Core.Creators;
using ReportGenerator.Core.Models;
using ReportGenerator.Core.Providers.CodeProviders;
using ReportGenerator.Core.Tools;

try
{
    var inputData = new InputModelBuilder()
         .SetLabNumber(1)
         .SetFullName("Кошкин Михаил")
         .SetGroupName("M32051")
         .SetReportFolderPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
         .SetCodeProvider(new GithubCodeProvider("ghp_yourGithubToken:)", "Mihinator3000", 1))
         //.SetCodeProvider(new LocalCodeProvider(@"C:\Users\Koshkin\Desktop\Banks"))
         .Build();

    new WordCreator(inputData).CreateReport();
}
catch (ReportGenException e)
{
    Console.WriteLine(e.Message);
}
