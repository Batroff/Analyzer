using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Analyzer
{
    public class LogAnalyzer
    {
        public void Start(List<FileInfo> logs)
        {
            int allShares = 0;
            TimeSpan allTime = TimeSpan.Zero;

            foreach (FileInfo log in logs)
            {
                Console.WriteLine($"\n** {log.Name} **");
                string path = $"{log.Directory}\\{log.Name}";
                FileStats stats = ScanFile(path);
                Console.WriteLine($"SHARES: {stats.Counter}");
                // Console.WriteLine($"Время работы: {stats.WorkTime.Days}(дней) {stats.WorkTime.Hours}:{stats.WorkTime.Minutes}:{stats.WorkTime.Seconds}");
                Console.WriteLine($"Время работы (дни.часы:минуты:секунды): {stats.WorkTime}");
                allShares += stats.Counter;
                allTime += stats.WorkTime;
                Console.WriteLine("===============================");
            }

            Console.WriteLine();
            Console.WriteLine("** Всего **");
            Console.WriteLine($"SHARES: {allShares}");
            Console.WriteLine($"Время работы (дни.часы:минуты:секунды): {allTime}");
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
            int counter = 0;
            TimeSpan workTime = TimeSpan.Zero;
            
            try
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string line = streamReader.ReadLine();

                    bool foundStart = false;
                    DateTime startTime = DateTime.MinValue;
                    DateTime endTime = DateTime.MinValue;

                    while (!string.Equals(line, null, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (isShare(line))
                            counter++;

                        if (line.Contains("WD:"))
                        {
                            string stringDate = Regex.Replace(line, "\\sWD.*", "");

                            if (!foundStart)
                            {
                                startTime = ConvertToDate(stringDate);
                                foundStart = true;
                            }
                        
                            endTime = ConvertToDate(stringDate);
                        }

                        line = streamReader.ReadLine();
                    }
                    workTime = endTime - startTime;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            return new FileStats(counter, workTime);
        }
        
        
        private bool isShare(string line)
        {
            return line.Contains("[ OK ]");
        }
    }
}

class FileStats
{
    public FileStats(int counter, TimeSpan workTime)
    {
        Counter = counter;
        WorkTime = workTime;
    }

    public int Counter { get; }
    public TimeSpan WorkTime { get; }
}