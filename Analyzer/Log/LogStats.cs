using System;
using System.Collections.Generic;
using System.Linq;

namespace Analyzer.Log
{
    public class LogStats
    {
        public LogStats(List<LogShare> share, DateTime startTime, DateTime endTime)
        {
            _shares = new List<LogShare>(share);
            _startTime = startTime;
            _endTime = endTime;
        }

        private readonly List<LogShare> _shares;
        private readonly DateTime _startTime;
        private readonly DateTime _endTime;

        public TimeSpan WorkTime => _endTime - _startTime;
        public int Shares => _shares.Count;

        public bool IsInInterval(DateTime start, DateTime end)
        {
            return start <= _endTime && end >= _startTime;
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
}