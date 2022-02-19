using System;
using System.IO;
using LabReport.Tools;

namespace LabReport.Services
{
    public class IoService
    {
        public string GetPathFolder()
        {
            Console.Write("Input path folder: ");

            string path = Console.ReadLine();
            if (!Directory.Exists(path))
                throw new LabReportException("folder with that path does not exist");

            return path;
        }

        public void PrintError(string message) =>
            Console.WriteLine($"Error: {message}");
    }
}