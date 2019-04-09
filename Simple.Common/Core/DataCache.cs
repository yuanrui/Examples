using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Common.Core
{
    public class DataCache<TEntity, TKey> : IDisposable
    {
        protected CancellationTokenSource cancelToken;
        protected ConcurrentDictionary<TKey, DataEntry<TEntity>> Cache = new ConcurrentDictionary<TKey, DataEntry<TEntity>>();

        protected virtual Task Task { get; set; }
        protected virtual TimeSpan Delay { get; set; }
        public volatile Int32 LifeMinutes;

        public static readonly DataCache<TEntity, TKey> Instance = new DataCache<TEntity, TKey>();

        public class DataEntry<TEntry>
        {
            public TEntry Item { get; set; }

            public DateTime CreatedAt { get; set; }

            public DataEntry()
            {
                this.CreatedAt = DateTime.Now;
            }
        }

        protected DataCache()
            : this(5)
        {

        }

        protected DataCache(Int32 lifeMinutes)
        {
            this.LifeMinutes = lifeMinutes;
            this.Delay = TimeSpan.FromSeconds(5);
            this.cancelToken = new CancellationTokenSource();
            this.Task = Task.Factory.StartNew(new Action(RunInLoop), cancelToken.Token);
        }

        protected virtual void RunInLoop()
        {
            while (!cancelToken.Token.IsCancellationRequested)
            {
                try
                {
                    DoWork();
                }
                catch (Exception ex)
                {
                    Trace.Write("Clear cache exception:" + ex);
                }
                finally
                {
                    Thread.Sleep(this.Delay);
                }
            }

        }

        protected virtual void DoWork()
        {
            //TODO time back
            var list = Cache.Where(m => (DateTime.Now - m.Value.CreatedAt).TotalMinutes >= this.LifeMinutes).ToList();
            if (list == null || list.Count == 0)
            {
                return;
            }

            DataEntry<TEntity> entry = null;
            foreach (var item in list)
            {
                Cache.TryRemove(item.Key, out entry);
            }
        }

        public void Set(TKey key, TEntity entity)
        {
            var entry = new DataEntry<TEntity>() { Item = entity };
            Cache.AddOrUpdate(key, entry, (k, ue) =>
            {
                ue.Item = entity;
                ue.CreatedAt = DateTime.Now;
                return ue;
            });
        }

        public TEntity Get(TKey key)
        {
            DataEntry<TEntity> entry = null;
            if (Cache.TryGetValue(key, out entry))
            {
                if (entry != null)
                {
                    return entry.Item;
                }
            }

            return default(TEntity);
        }

        public void Dispose()
        {
            DoDispose();
        }

        protected virtual void DoDispose()
        {
            Cache.Clear();
        }
    }
}
