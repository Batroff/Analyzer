using System;
using System.Collections.Generic;
using System.IO;
using Analyzer.Log;

namespace Analyzer
{
    class Program
    {
        private static readonly FileGetter LogGetter = new FileGetter();

        static void Main(string[] args)
        {
            try
            {
                LogGetter.ImportLogs();
            }
            catch (ImportLogsException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            List<FileInfo> logs = LogGetter.Logs;
            
            LogAnalyzer logAnalyzer = new LogAnalyzer();
            logAnalyzer.Start(logs);
            
            CliFormatter.PrintExitMessage();
        }
    }
}