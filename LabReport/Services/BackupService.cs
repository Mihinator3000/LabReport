using System.IO;

namespace LabReport.Services
{
    public class BackupService
    {
        public const string BackupFolderName = "LabBackup";

        private readonly string _labFolder;

        public BackupService(string labFolder) =>
            _labFolder = labFolder;

        public void Copy() => CopyAllFiles(
            new DirectoryInfo(_labFolder),
            new DirectoryInfo(BackupFolderName));

        private static void CopyAllFiles(DirectoryInfo source, DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName))
                Directory.Delete(target.FullName);

            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fileInfo in source.GetFiles())
                fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name));

            foreach (DirectoryInfo subDirectoryInfo in source.GetDirectories())
                CopyAllFiles(subDirectoryInfo,
                    target.CreateSubdirectory(subDirectoryInfo.Name));
        }
    }
}