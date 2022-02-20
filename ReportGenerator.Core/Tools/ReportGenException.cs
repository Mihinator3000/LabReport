namespace ReportGenerator.Core.Tools;

public class ReportGenException : Exception
{
    public ReportGenException()
    {
    }

    public ReportGenException(string message)
        : base(message)
    {
    }

    public ReportGenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}