using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Analyzer
{
    public class LogAnalyzer
    {
        private readonly Regex _timeRegex = new Regex("^[0-9]{8}\\s([0-9]{2}:?){3}");
        private Match _match;
        
        public void Start(List<FileInfo> logs)
        {
            int allShares = 0;
            TimeSpan allTime = TimeSpan.Zero;

            DateTime startTime = InputTime("Дата начала поиска", DateTime.MinValue);
            DateTime endTime = InputTime("Дата конца поиска", DateTime.MaxValue);
            List<string> unmatchedLogs = new List<string>();
            
            foreach (FileInfo log in logs)
            {
                string path = $"{log.Directory}\\{log.Name}";
                FileStats stats = ScanFile(path);

                int shares = stats.GetSharesInInterval(startTime, endTime);
                TimeSpan workTime = stats.GetWorkTimeInInterval(startTime, endTime);
                if (stats.IsInInterval(startTime))
                {
                    Console.WriteLine($"\n** {log.Name} **");
                    Console.WriteLine($"SHARES: {shares}");
                    Console.WriteLine($"Время работы (дни.часы:минуты:секунды): {workTime}");
                    allShares += shares;
                    allTime += workTime;
                    Console.WriteLine("===============================");
                }
                else
                {
                    unmatchedLogs.Add(log.Name);
                }
            }

            Console.WriteLine("Не совпадают с данным интервалом: " + String.Join(", ", unmatchedLogs));

            Console.WriteLine();
            Console.WriteLine("** Всего **");
            Console.WriteLine($"SHARES: {allShares}");
            Console.WriteLine($"Время работы (дни.часы:минуты:секунды): {allTime}");
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

        private FileStats ScanFile(string path)
        {
            bool ContainsShare(string line) => line.Contains("[ OK ]");
            List<Share> shares = new List<Share>();
            
            DateTime startTime = DateTime.MinValue;
            DateTime endTime = DateTime.MinValue;
            
            try
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string line = streamReader.ReadLine();

                    bool foundStart = false;

                    while (!string.Equals(line, null, StringComparison.CurrentCultureIgnoreCase))
                    {
                        _match = _timeRegex.Match(line);
                        if (!_match.Success)
                        {
                            line = streamReader.ReadLine();
                            continue;
                        }
                        
                        string unparsedDate = _match.Groups[0].ToString();
                        DateTime parsedDate = ConvertToDate(unparsedDate);
                        
                        if (ContainsShare(line))
                        {
                            shares.Add(new Share(parsedDate));
                        }
                        if (!foundStart)
                        {
                            startTime = parsedDate;
                            foundStart = true;
                        }
                        
                        endTime = parsedDate;
                        line = streamReader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return new FileStats(shares, startTime, endTime);
        }
    }
}

class FileStats
{
    public FileStats(List<Share> share, DateTime startTime, DateTime endTime)
    {
        _shares = new List<Share>(share);
        _startTime = startTime;
        _endTime = endTime;
    }

    private readonly List<Share> _shares;
    private readonly DateTime _startTime;
    private readonly DateTime _endTime;

    public TimeSpan WorkTime => _endTime - _startTime;
    public int Shares => _shares.Count;

    public bool IsInInterval(DateTime start)
    {
        return start < _endTime;
    }
    
    public TimeSpan GetWorkTimeInInterval(DateTime start, DateTime end)
    {
        DateTime resStart = _startTime > start ? _startTime : start;
        DateTime resEnd = _endTime > end ? end : _endTime;
        return resEnd - resStart;
    } 
        
    public int GetSharesInInterval(DateTime start, DateTime end)
    {
        return _shares.Count(share => share.CreationTime >= start && share.CreationTime <= end);
    }
}

class Share
{
    public Share(DateTime time)
    {
        CreationTime = time;
    }
    
    public DateTime CreationTime { get; }
}