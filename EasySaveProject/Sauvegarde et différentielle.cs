using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.IO;

namespace EasySaveProject
{
    public interface IBackupType
    {
        void Transfer(int id, string name, string source, string target);
        void Transfer(int id, string name, object source, object target);
    }

    public class FullBackup : IBackupType
    {
        private StateService stateService = new StateService();
        private LanguageService _languageService;

        public FullBackup(LanguageService languageService)
        {
            _languageService = languageService;
        }

        public void Transfer(int id, string name, string source, string target)
        {
            Console.WriteLine(string.Format(_languageService.GetTranslation("FullBackupExecution"), source, target));

            string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            int totalFiles = files.Length;
            int nbFilesLeft = totalFiles;
            long totalSize = 0;

            foreach (string file in files)
            {
                totalSize += new FileInfo(file).Length;
            }

            stateService.StartTracking(id, name, source, target, totalFiles, totalSize);

            foreach (string file in files)
            {
                string relativePath = Path.GetRelativePath(source, file);
                string destFile = Path.Combine(target, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
                File.Copy(file, destFile, true);
                nbFilesLeft--;

                int progression = totalFiles > 0 ? (100 * (totalFiles - nbFilesLeft) / totalFiles) : 100;
                stateService.Update(id, nbFilesLeft);
            }

            stateService.Finish(id);
            Console.WriteLine(_languageService.GetTranslation("FullBackupSuccess"));
        }

        void IBackupType.Transfer(int id, string name, string source, string target)
        {
            throw new NotImplementedException();
        }

        void IBackupType.Transfer(int id, string name, object source, object target)
        {
            throw new NotImplementedException();
        }
    }

    public class DifferentialBackup : IBackupType
    {
        private StateService stateService = new StateService();
        private LanguageService _languageService;

        public DifferentialBackup(LanguageService languageService)
        {
            _languageService = languageService;
        }

        public void Transfer(int id, string name, string source, string target)
        {
            Console.WriteLine(string.Format(_languageService.GetTranslation("DifferentialBackupExecution"), source, target));

            string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            int totalFiles = files.Length;
            int nbFilesLeft = totalFiles;
            long totalSize = 0;

            foreach (string file in files)
            {
                totalSize += new FileInfo(file).Length;
            }

            stateService.StartTracking(id, name, source, target, totalFiles, totalSize);

            foreach (string file in files)
            {
                string relativePath = Path.GetRelativePath(source, file);
                string destFile = Path.Combine(target, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);

                if (!File.Exists(destFile) || File.GetLastWriteTime(file) > File.GetLastWriteTime(destFile))
                {
                    File.Copy(file, destFile, true);
                }

                nbFilesLeft--;

                int progression = totalFiles > 0 ? (100 * (totalFiles - nbFilesLeft) / totalFiles) : 100;
                stateService.Update(id, nbFilesLeft);
            }

            stateService.Finish(id);
            Console.WriteLine(_languageService.GetTranslation("DifferentialBackupSuccess"));
        }

        void IBackupType.Transfer(int id, string name, string source, string target)
        {
            throw new NotImplementedException();
        }

        void IBackupType.Transfer(int id, string name, object source, object target)
        {
            throw new NotImplementedException();
        }
    }

    public class StateService
    {
        internal void StartTracking(int id, string name, string source, string target, int totalFiles, long totalSize)
        {
            Console.WriteLine($"[Tracking Start] Task: {name}, Files: {totalFiles}, Size: {totalSize} bytes");
        }

        internal void Update(int id, int nbFilesLeft)
        {
            Console.WriteLine($"[Progress] {nbFilesLeft} file(s) left.");
        }

        internal void Finish(int id)
        {
            Console.WriteLine("[Tracking Finish] Backup completed.");
        }
    }

    public class LanguageService
    {
        internal string GetTranslation(string key)
        {
            return key switch
            {
                "FullBackupExecution" => "Starting full backup from {0} to {1}...",
                "FullBackupSuccess" => "Full backup completed successfully.",
                "DifferentialBackupExecution" => "Starting differential backup from {0} to {1}...",
                "DifferentialBackupSuccess" => "Differential backup completed successfully.",
                _ => $"[{key}]"
            };
        }
    }
}

