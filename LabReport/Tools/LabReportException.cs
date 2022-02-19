namespace LabReport.Tools;

public class LabReportException : Exception
{
    public LabReportException()
    {
    }

    public LabReportException(string message)
        : base(message)
    {
    }

    public LabReportException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}