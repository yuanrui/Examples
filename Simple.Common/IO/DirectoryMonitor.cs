using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simple.Common.IO
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// copy from https://spin.atomicobject.com/2010/07/08/consolidate-multiple-filesystemwatcher-events/
    /// </remarks>
    public interface IDirectoryMonitor
    {
        event FileSystemEventHandler Change;
        void Start();
    }

    public class DirectoryMonitor : IDirectoryMonitor, IDisposable
    {
        private readonly FileSystemWatcher m_fileSystemWatcher = new FileSystemWatcher();
        private readonly Dictionary<string, FileSystemEventArgsWrapper> m_pendingEvents = new Dictionary<string, FileSystemEventArgsWrapper>();
        private readonly Timer m_timer;
        private bool m_timerStarted = false;

        public DirectoryMonitor(string dirPath, string filter)
        {
            m_fileSystemWatcher.Path = dirPath;
            m_fileSystemWatcher.Filter = filter;
            m_fileSystemWatcher.IncludeSubdirectories = true;
            m_fileSystemWatcher.Created += new FileSystemEventHandler(OnChange);
            m_fileSystemWatcher.Changed += new FileSystemEventHandler(OnChange);
            m_fileSystemWatcher.Deleted += new FileSystemEventHandler(OnChange);
            m_fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess
                                   | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;

            m_timer = new Timer(OnTimeout, null, Timeout.Infinite, Timeout.Infinite);
        }

        public event FileSystemEventHandler Change;

        public void Start()
        {
            m_fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void OnChange(object sender, FileSystemEventArgs e)
        {
            // Don't want other threads messing with the pending events right now  
            lock (m_pendingEvents)
            {
                // Save a timestamp for the most recent event for this path  
                m_pendingEvents[e.FullPath] = new FileSystemEventArgsWrapper(sender, e);

                // Start a timer if not already started  
                if (!m_timerStarted)
                {
                    m_timer.Change(100, 100);
                    m_timerStarted = true;
                }
            }
        }

        private void OnTimeout(object state)
        {
            List<FileSystemEventArgsWrapper> paths;

            // Don't want other threads messing with the pending events right now  
            lock (m_pendingEvents)
            {
                // Get a list of all paths that should have events thrown  
                paths = FindReadyPaths(m_pendingEvents);

                // Remove paths that are going to be used now  
                paths.ForEach(delegate(FileSystemEventArgsWrapper wrapper)
                {
                    m_pendingEvents.Remove(wrapper.Path);
                });

                // Stop the timer if there are no more events pending  
                if (m_pendingEvents.Count == 0)
                {
                    m_timer.Change(Timeout.Infinite, Timeout.Infinite);
                    m_timerStarted = false;
                }
            }

            // Fire an event for each path that has changed  
            paths.ForEach(delegate(FileSystemEventArgsWrapper path)
            {
                FireEvent(path);
            });
        }

        private List<FileSystemEventArgsWrapper> FindReadyPaths(Dictionary<string, FileSystemEventArgsWrapper> events)
        {
            List<FileSystemEventArgsWrapper> results = new List<FileSystemEventArgsWrapper>();
            DateTime now = DateTime.Now;

            foreach (KeyValuePair<string, FileSystemEventArgsWrapper> entry in events)
            {
                // If the path has not received a new event in the last 75ms  
                // an event for the path should be fired  
                double diff = now.Subtract(entry.Value.Time).TotalMilliseconds;
                if (diff >= 75)
                {
                    results.Add(entry.Value);
                }
            }

            return results;
        }

        private void FireEvent(FileSystemEventArgsWrapper wrapper)
        {
            FileSystemEventHandler evt = Change;
            if (evt != null)
            {
                evt(wrapper.Sender, wrapper.Args);
            }
        }

        public void Dispose()
        {
            m_fileSystemWatcher.EnableRaisingEvents = false;
            if (m_fileSystemWatcher != null)
            {
                m_fileSystemWatcher.Dispose();
            }

            lock (m_pendingEvents)
            {
                if (m_pendingEvents != null)
                {
                    m_pendingEvents.Clear();
                }
            }
        }

        private class FileSystemEventArgsWrapper
        {
            public FileSystemEventArgs Args { get; set; }

            public object Sender { get; set; }

            public DateTime Time { get; set; }

            public string Path { get; set; }

            public FileSystemEventArgsWrapper(object sender, FileSystemEventArgs e)
            {
                Args = e;
                Sender = sender;
                Path = e.FullPath;
                Time = DateTime.Now;
            }
        }
    }
}
