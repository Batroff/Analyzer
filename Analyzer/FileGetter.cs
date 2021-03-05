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
                foreach (FileInfo file in _dir.GetFiles())
                {
                    if (file.Name.StartsWith("log")) _logs.Add(file);
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