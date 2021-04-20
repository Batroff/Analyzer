using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Analyzer.Log;

namespace Analyzer
{
    public class CliFormatter
    {
        private readonly List<FileInfo> _foundLogs;
        private readonly LogAnalyzer _analyzer = new LogAnalyzer();
        private readonly bool _showAllFiles;
        public CliFormatter(List<FileInfo> logs, bool showAllFiles)
        {
            _foundLogs = logs;
            _showAllFiles = showAllFiles;
        }

        /// <summary>
        /// Printing table line in format: <code>=================== | ============ | ====== | ===</code>
        /// </summary>
        private void PrintLine(string name, TimeSpan workTime, int shares, bool inInterval)
        {
            Console.WriteLine("| {0, -22} | {1, -12} | {2, -6} | {3, -11} |", name, workTime, shares, inInterval ? "Да" : "Нет");
        }

        /// <summary>
        /// Printing table header in format: <code>Имя файла | Время работы | Shares | В интервале</code>
        /// </summary>
        private void PrintHeader()
        {
            Console.WriteLine("| {0, -22} | {1, -12} | {2, -6} | {3, -11} |", "Имя файла", "Время работы", "Shares", "В интервале");
        }
        
        /// <summary>
        /// Printing table separator: <code>=========================================================</code>
        /// </summary>
        private void PrintSeparator()
        {
            Console.WriteLine("================================================================");
        }

        public void PrintLogsTable(DateTime startTime, DateTime endTime)
        {
            PrintSeparator();
            PrintHeader();
            
            int allShares = 0;
            TimeSpan allTime = TimeSpan.Zero;
            
            foreach (FileInfo log in _foundLogs)
            {
                string path = $"{log.Directory}\\{log.Name}";
                LogStats stats;
                try
                {
                    stats = _analyzer.ScanFile(path);
                }
                catch (IOException e)
                {
                    Console.WriteLine($"Ошибка {e.GetType()} при чтении лога, закройте приложения, которые могут использовать этот файл {log.Name}!");
                    continue;
                }

                bool isInterval = stats.IsInInterval(startTime, endTime);
                if (isInterval)
                {
                    var shares = stats.GetSharesInInterval(startTime, endTime);
                    var workTime = stats.GetWorkTimeInInterval(startTime, endTime);
                    allShares += shares;
                    allTime += workTime;
                    PrintLine(log.Name, workTime, shares, true);
                } else if (_showAllFiles)
                {
                    PrintLine(log.Name, TimeSpan.Zero, 0, false);
                }
                
            }
            PrintSeparator();

            Console.WriteLine("** Всего **");
            Console.WriteLine($"SHARES: {allShares}");
            Console.WriteLine($"Время работы (дни.часы:минуты:секунды): {allTime}");
        }

        public void PrintFoundLogs()
        {
            Console.WriteLine("===============================");
            Console.WriteLine($"Найденные логи: {String.Join(", ", _foundLogs.Select(log => log.Name))}");
            Console.WriteLine("===============================");
        }

        public static void PrintExitMessage()
        {
            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine("Для завершения работы нажмите любую клавишу...");
            Console.ReadKey(true);
        }
    }
}