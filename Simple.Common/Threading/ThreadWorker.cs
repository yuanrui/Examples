using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Simple.Common.Threading
{
    public class ThreadWorker : IWorker
    {
        private readonly Action _methodToRunInLoop;
        private Thread _thread;
        private bool _stopRequested;
        private object _syncObject = new object();
        protected string _workerName;

        public string Name
        {
            get { return this.ThreadName; }
        }

        protected virtual string ThreadName
        {
            get
            {
                return _workerName + "." + GetThreadId().ToString();
            }
        }

        protected virtual bool IsBackground
        {
            get
            {
                return false;
            }
        }

        protected bool StopRequested
        {
            get
            {
                bool result;
                lock (_syncObject)
                    result = _stopRequested;

                return result;
            }
        }

        public ThreadWorker()
            : this(null)
        {
            _workerName = this.GetType().Name;
            _methodToRunInLoop = DoWork;
        }

        public ThreadWorker(Action methodToRunInLoop)
        {
            _workerName = this.GetType().Name;
            _methodToRunInLoop = methodToRunInLoop;
            this._thread = new Thread(new ThreadStart(this.Loop));
            this._thread.Name = this.ThreadName;
            this._thread.IsBackground = this.IsBackground;
        }

        public void Start()
        {
            if (!this._thread.IsAlive)
            {
                this._thread.Start();
            }

            Trace.WriteLine(this.ThreadName + " started.");
        }

        public void Stop()
        {
            lock (_syncObject)
            {
                _stopRequested = true;
            }

            Trace.WriteLine(this.ThreadName + " stopped.");
        }

        protected void Loop()
        {
            while (!StopRequested)
            {
                try
                {
                    _methodToRunInLoop();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(this.ThreadName + " run exception:" + ex.Message);
                    Thread.Sleep(1);
                }
            }
        }

        protected virtual void DoWork()
        {
            Trace.Write("//TODO Override");
        }

        protected Int32 GetThreadId()
        {
            return _thread.ManagedThreadId;
        }
    }
}
