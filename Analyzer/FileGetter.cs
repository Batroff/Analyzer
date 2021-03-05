using System;
using System.Collections.Generic;
using System.IO;

namespace Analyzer
{
    public class FileGetter
    {
        private DirectoryInfo _dir;
        private List<FileInfo> _logs;

        public FileGetter()
        {
            _logs = new List<FileInfo>();
        }

        private void ImportLogs()
        {
            try
            {
                _dir = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\logs");
                foreach (DirectoryInfo chunkDir in _dir.GetDirectories())
                {
                    if (chunkDir.Name.Contains("old") || !chunkDir.Name.Contains("chunk"))
                        continue;
                    
                    foreach (FileInfo file in chunkDir.GetFiles())
                    {
                        if (file.Name.Contains("log")) _logs.Add(file);
                    }
                }
                foreach (FileInfo file in _dir.GetFiles())
                {
                    if (file.Name.Contains("log")) _logs.Add(file);
                }

                Console.WriteLine("===============================");
                Console.WriteLine("Найденные логи:");
                foreach (var log in _logs)
                {
                    Console.WriteLine(log.Name);
                }
                Console.WriteLine("===============================");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public List<FileInfo> GetLogs()
        {
            ImportLogs();
            return _logs;
        }
    }
}