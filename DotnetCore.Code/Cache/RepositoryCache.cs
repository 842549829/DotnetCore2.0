//-----------------------------------------------------------------------------------
// <copyright file="RepositoryCache.cs" company="EastWestWalk Enterprises">
// * copyright: (C) 2017 东走西走科技有限公司 版权所有。
// * version  : 1.0.0.0
// * author   : rongbo
// * fileName : RepositoryCache.cs
// * history  : created by rongbo 2017/3/17 11:07:01
// </copyright>
// <summary>
//   EastWestWalk.NetFrameWork.Common.Cache.RepositoryCache
// </summary>
//-----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Timers;

namespace DotnetCore.Code.Cache
{
    /// <summary>
    /// 缓存
    /// </summary>
    /// <typeparam name="TKey">
    /// TKey
    /// </typeparam>
    /// <typeparam name="TValue">
    /// TValue
    /// </typeparam>
    public class RepositoryCache<TKey, TValue>
    {
        /// <summary>
        /// 刷新时间
        /// </summary>
        protected const double DefaultInterval = 2 * 60 * 1000;

        /// <summary>
        /// IRepository
        /// </summary>
        protected readonly IRepository repository;

        /// <summary>
        /// Cache
        /// </summary>
        protected readonly Cache<TKey, TValue> cache;

        /// <summary>
        /// Timer
        /// </summary>
        protected readonly Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public RepositoryCache(IRepository repository)
            : this(repository, new Cache<TKey, TValue>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public RepositoryCache(IRepository repository, Cache<TKey, TValue> cache)
            : this(repository, cache, DefaultInterval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="refreshInterval">
        /// The refresh interval.
        /// </param>
        public RepositoryCache(IRepository repository, double refreshInterval)
            : this(repository, new Cache<TKey, TValue>(), new Timer(refreshInterval))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="refreshInterval">
        /// The refresh interval.
        /// </param>
        public RepositoryCache(IRepository repository, Cache<TKey, TValue> cache, double refreshInterval)
            : this(repository, new Cache<TKey, TValue>(), new Timer(refreshInterval))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="timer">
        /// The timer.
        /// </param>
        private RepositoryCache(IRepository repository, Cache<TKey, TValue> cache, Timer timer)
        {
            this.repository = repository;
            this.cache = cache;
            this.timer = timer;
            this.timer.Elapsed += this.TimerElapsed;
            this.Refresh();
            this.timer.Start();
        }

        /// <summary>
        /// Values
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get { return this.cache.Values; }
        }

        /// <summary>
        /// TValue
        /// </summary>
        /// <param name="key">
        /// TKey
        /// </param>
        /// <returns>
        /// TValue
        /// </returns>
        public TValue this[TKey key]
        {
            get
            {
                return this.cache[key];
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            IEnumerable<KeyValuePair<TKey, TValue>> data = this.QueryFromRepository();
            this.cache.Refresh(data);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">
        /// key
        /// </param>
        /// <param name="value">
        /// value
        /// </param>
        public void Add(TKey key, TValue value)
        {
            this.AddModelToRepository(value);
            this.cache.Add(key, value);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="value">
        /// value
        /// </param>
        public virtual void Add(TValue value)
        {
            string key = this.AddModelToRepositoryResult(value).ToString();
            this.cache.Add((TKey)(object)key, value);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="key">
        /// key
        /// </param>
        /// <param name="value">
        /// value
        /// </param>
        public void Update(TKey key, TValue value)
        {
            this.UpdateModelFromRepository(value);
            this.cache.Save(key, value);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key">
        /// key
        /// </param>
        /// <returns>
        /// TValue
        /// </returns>
        public TValue Remove(TKey key)
        {
            TValue value = this[key];
            if (value != null)
            {
                this.DeleteModelFromRepository(value);
                this.cache.Remove(key);
            }

            return value;
        }

        /// <summary>
        /// QueryFromRepository
        /// </summary>
        /// <returns>IEnumerable</returns>
        protected virtual IEnumerable<KeyValuePair<TKey, TValue>> QueryFromRepository()
        {
            if (null != this.repository)
            {
                return this.repository.Query();
            }

            return null;
        }

        /// <summary>
        /// AddModelToRepository
        /// </summary>
        /// <param name="value">
        /// value
        /// </param>
        protected virtual void AddModelToRepository(TValue value)
        {
            if (null != this.repository)
            {
                this.repository.Insert(value);
            }
        }

        /// <summary>
        /// AddModelToRepository
        /// </summary>
        /// <param name="value">
        /// value
        /// </param>
        protected virtual int AddModelToRepositoryResult(TValue value)
        {
            if (null != this.repository)
            {
                return this.repository.Insert(value);
            }
            return 0;
        }

        /// <summary>
        /// UpdateModelFromRepository
        /// </summary>
        /// <param name="value">
        /// value
        /// </param>
        protected virtual void UpdateModelFromRepository(TValue value)
        {
            if (null != this.repository)
            {
                this.repository.Modify(value);
            }
        }

        /// <summary>
        /// DeleteModelFromRepository
        /// </summary>
        /// <param name="value">
        /// value
        /// </param>
        protected virtual void DeleteModelFromRepository(TValue value)
        {
            if (null != this.repository)
            {
                this.repository.Delete(value);
            }
        }

        /// <summary>
        /// 事件
        /// </summary>
        /// <param name="sender">
        /// sender
        /// </param>
        /// <param name="e">
        /// e
        /// </param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.Refresh();
        }

        /// <summary>
        /// IRepository
        /// </summary>
        public interface IRepository
        {
            /// <summary>
            /// Query
            /// </summary>
            /// <returns>IEnumerable</returns>
            IEnumerable<KeyValuePair<TKey, TValue>> Query();

            /// <summary>
            /// Insert
            /// </summary>
            /// <param name="value">
            /// value
            /// </param>
            /// <returns>
            /// int
            /// </returns>
            int Insert(TValue value);

            /// <summary>
            /// Modify
            /// </summary>
            /// <param name="value">
            /// value
            /// </param>
            /// <returns>
            /// int
            /// </returns>
            int Modify(TValue value);

            /// <summary>
            /// Delete
            /// </summary>
            /// <param name="value">
            /// value
            /// </param>
            /// <returns>
            /// int
            /// </returns>
            int Delete(TValue value);
        }
    }

    /// <summary>
    /// 自动增长Id
    /// </summary>
    [DataContract]
    public abstract class IdentityValue
    {
        /// <summary>
        /// Id
        /// </summary>
        [DataMember]
        public int Id { get; set; }
    }

    /// <summary>
    /// 兼容自动增长Id
    /// </summary>
    /// <typeparam name="TKey">key</typeparam>
    /// <typeparam name="TValue">value</typeparam>
    public class RepositoryIdentityCache<TKey, TValue> : RepositoryCache<TKey, TValue> where TValue : IdentityValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public RepositoryIdentityCache(IRepository repository) : base(repository)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        public RepositoryIdentityCache(IRepository repository, Cache<TKey, TValue> cache) : base(repository, cache)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="refreshInterval">
        /// The refresh interval.
        /// </param>
        public RepositoryIdentityCache(IRepository repository, double refreshInterval) : base(repository, refreshInterval)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryCache{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="refreshInterval">
        /// The refresh interval.
        /// </param>
        public RepositoryIdentityCache(IRepository repository, Cache<TKey, TValue> cache, double refreshInterval) : base(repository, cache, refreshInterval)
        {
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="value">
        /// value
        /// </param>
        public override void Add(TValue value)
        {
            int key = this.AddModelToRepositoryResult(value);
            value.Id = key;
            string keyVal = key.ToString();
            this.cache.Add((TKey)(object)keyVal, value);
        }
    }
}