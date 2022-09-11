namespace ReportGenerator.Core.Tools;

public class ReportGenException : Exception
{
    public ReportGenException(string message)
        : base(message)
    {
    }
}