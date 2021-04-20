using System;

namespace Analyzer.Log
{
    public class LogShare
    {
        public LogShare(DateTime time)
        {
            CreationTime = time;
        }
    
        public DateTime CreationTime { get; }
    }
}