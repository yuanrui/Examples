using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Common.Threading
{
    public class TaskWorker : IWorker
    {
        private readonly Action _methodToRunInLoop;
        private Task _task;
        private bool _stopRequested;
        private object _syncObject = new object();
        protected string _workerName;

        public string Name
        {
            get { return this.TaskName; }
        }

        protected virtual string TaskName
        {
            get
            {
                return _workerName + "." + GetTaskId().ToString();
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

        public TaskWorker()
            : this(null)
        {
            _workerName = this.GetType().Name;
            _methodToRunInLoop = DoWork;
        }

        public TaskWorker(Action methodToRunInLoop)
        {
            _workerName = this.GetType().Name;
            _methodToRunInLoop = methodToRunInLoop;
            _task = new Task(this.Loop, TaskCreationOptions.LongRunning);
        }

        public void Start()
        {
            if (!(_task.Status == TaskStatus.Running))
            {
                _task.Start(TaskScheduler.Current);
            }
        }

        public void Stop()
        {
            lock (_syncObject)
            {
                _stopRequested = true;                
            }
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
                    Trace.WriteLine(this.TaskName + " run exception:" + ex.Message);
                }
            }
        }

        protected virtual void DoWork()
        {
            Trace.Write("//TODO Override");
        }

        protected Int32 GetTaskId()
        {
            return _task.Id;
        }
    }
}
