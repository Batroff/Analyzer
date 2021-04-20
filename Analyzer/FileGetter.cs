using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Analyzer
{
    // TODO: Add custom exceptions
    public class ImportLogsException : IOException
    {
        public ImportLogsException(string message) : base(message) { }
    }

    public class FileGetter
    {
        private DirectoryInfo _dir;
        public List<FileInfo> Logs { get; }
        private CliFormatter _cliFormatter;

        public FileGetter()
        {
            Logs = new List<FileInfo>();
        }

        public void ImportLogs()
        {
            IEnumerable<FileInfo> FindLogs(FileInfo[] files) =>
                files.Select(file => file).Where(file => file.Name.Contains("log"));
            
            try
            {
                _dir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\logs");
                foreach (DirectoryInfo chunkDir in _dir.GetDirectories())
                {
                    if (chunkDir.Name.Contains("old") || !chunkDir.Name.Contains("chunk"))
                        continue;
                 
                    Logs.AddRange(FindLogs(chunkDir.GetFiles()));
                }
                
                Logs.AddRange(FindLogs(_dir.GetFiles()));

                _cliFormatter = new CliFormatter(Logs, false);
                _cliFormatter.PrintFoundLogs();
            }
            catch
            {
                throw new ImportLogsException("An error ImportLogsException occured in method ImportLogs");
            }
        }
    }
}