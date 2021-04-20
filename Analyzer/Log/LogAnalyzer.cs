using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Analyzer.Log
{
    public class LogAnalyzer
    {
        private readonly Regex _timeRegex = new Regex("^[0-9]{8}\\s([0-9]{2}:?){3}");
        private readonly Regex _wdRegex = new Regex("WD:.*,\\sshares:.*");
        private CliFormatter _cliFormatter;
        private Match _match;
        
        public void Start(List<FileInfo> logs)
        {
            DateTime startTime = InputTime("Дата начала поиска", DateTime.MinValue);
            DateTime endTime = InputTime("Дата конца поиска", DateTime.MaxValue);
            bool showAll = InputShowAllOption("Показать файлы не в рабочем интервале[Нажмите Y(Да)/Enter(нет)]");
            
            _cliFormatter = new CliFormatter(logs, showAll);
            _cliFormatter.PrintLogsTable(startTime, endTime);
        }

        private bool InputShowAllOption(string msg)
        {
            Console.Write($"{msg} >>> ");
            var key = Console.ReadKey().Key;
            Console.WriteLine();
            return key == ConsoleKey.Y;
        }

        private DateTime InputTime(string msg, DateTime defaultTime)
        {
            Console.WriteLine("Введите дату в формате YYYYMMDD hh:mm:ss. Например: 20201125 23:28:45");
            Console.Write($"{msg} >>> ");
            string input = Console.ReadLine();
            
            if (!_timeRegex.Match(input ?? "").Success)
            {
                Console.WriteLine("Неправильный формат даты");
                Console.WriteLine($"Установлено время по умолчанию {defaultTime}");
                return defaultTime;
            }
            
            Console.WriteLine();
            return ConvertToDate(input);
        }

        private DateTime ConvertToDate(string date)
        {
            int year = Convert.ToInt32(date.Substring(0, 4));
            int month = Convert.ToInt32(date.Substring(4, 2));
            int day = Convert.ToInt32(date.Substring(6, 2));
            int hour = Convert.ToInt32(date.Substring(9, 2));
            int min = Convert.ToInt32(date.Substring(12, 2));
            int sec = Convert.ToInt32(date.Substring(15, 2));

            return new DateTime(year, month, day, hour, min, sec);
        }

        public LogStats ScanFile(string path)
        {
            bool ContainsShare(string l) => l.Contains("[ OK ]");
            List<LogShare> shares = new List<LogShare>();
            
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;

            using StreamReader streamReader = new StreamReader(path);
            string line = streamReader.ReadLine();

            bool foundStart = false;
            string firstWdString = null;

            while (!string.Equals(line, null, StringComparison.CurrentCultureIgnoreCase))
            {
                _match = _timeRegex.Match(line);
                if (_match.Success)
                {
                    string unparsedDate = _match.Groups[0].ToString();
                    DateTime parsedDate = ConvertToDate(unparsedDate);

                    if (ContainsShare(line))
                    {
                        shares.Add(new LogShare(parsedDate));
                    }

                    if (!foundStart)
                    {
                        startTime = parsedDate;
                        foundStart = true;
                    }

                    endTime = parsedDate;
                }
                else if (firstWdString == null && _wdRegex.Match(line).Success)
                {
                    firstWdString = line;
                    int startShares = Int32.Parse(line.Split(',')[1].Substring(8).Split('/')[1]);
                    for (int i = 0; i < startShares; i++)
                        shares.Add(new LogShare(DateTime.MinValue));
                }
                
                line = streamReader.ReadLine();
            }
            
            return new LogStats(shares, startTime, endTime);
        }
    }
}