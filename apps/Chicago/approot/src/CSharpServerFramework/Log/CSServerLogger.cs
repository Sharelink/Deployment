using CSharpServerFramework.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CSharpServerFramework.Log
{
    /// <summary>
    /// 记录器
    /// 启动一条线程进行Log记录
    /// </summary>
    public class CSServerLogger
    {
        private Thread _logThread;
        private volatile bool _running = false;
        private System.Collections.Concurrent.ConcurrentQueue<string> _logQueue;
        private ManualResetEventSlim _nextLogAdded;
        private IList<ILoggerLog> _loggers;
        public CSServerLogger()
        {
            _loggers = new List<ILoggerLog>();
            _nextLogAdded = new ManualResetEventSlim(false);
        }

        public void AddLogger(ILoggerLog Logger)
        {
            _loggers.Add(Logger);
        }

        public void Init()
        {
            _logThread = new Thread(DoLogProc);
            _logQueue = new System.Collections.Concurrent.ConcurrentQueue<string>();
            _running = true;
            _logThread.Start();
            Log("Logger Inited");
        }

        private void DoLogProc(object obj)
        {
            while (_running)
            {
                string log;
                while (_logQueue.Count > 0)
                {
                    _logQueue.TryDequeue(out log);
                    foreach (var item in _loggers)
                    {
                        item.Log(log);
                    }
                }
                _nextLogAdded.Reset();
                _nextLogAdded.Wait(5 * 1000);
            }
        }
        public void Log(string LogText)
        {
            string log = string.Format("<{0}>:{1}", DateTime.UtcNow.ToString(), LogText);
            //Console.WriteLine(log);
            _logQueue.Enqueue(log);
            _nextLogAdded.Set();
        }
        public void Stop()
        {
            _running = false;
            _nextLogAdded.Set();
        }
    }
}
