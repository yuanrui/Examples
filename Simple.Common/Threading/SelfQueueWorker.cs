using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simple.Common.Threading
{
    public class SelfQueueWorker<T> : ThreadWorker, IWorker
    {
        protected ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();

        public virtual void Enqueue(T item)
        {
            Queue.Enqueue(item);
        }

        protected virtual IList<T> TryDequeue(Int32 capacity)
        {
            List<T> result = new List<T>(capacity);
            Int32 index = 0;

            while (!Queue.IsEmpty)
            {
                if (index >= capacity)
                {
                    break;
                }

                T item;
                if (!Queue.TryDequeue(out item))
                {
                    break;
                }

                result.Add(item);
                index++;
            }

            return result;
        }
    }
}
