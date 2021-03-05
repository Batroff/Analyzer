using System;
using System.Collections.Generic;
using System.IO;

namespace Analyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            FileGetter getter = new FileGetter();
            List<FileInfo> logs = getter.GetLogs();
            
            LogAnalyzer logAnalyzer = new LogAnalyzer();
            logAnalyzer.Start(logs);

            Console.ReadKey(true);
        }
    }
}