using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Standard.Common.Caches
{
    public class Objcached : IDisposable
    {
        private readonly IDictionary<string, object> _caches;
        private readonly IDictionary<string, DateTime> _times; 
        private readonly ReaderWriterLock _readerWriterLock;
        private readonly Thread _tMain;
        private bool _threadexit;
        //private const int _millisecondsTimeout = 5000;

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="timingcheck">定时检查时间</param>
        public Objcached(TimeSpan timingcheck = default(TimeSpan))
        {
            _caches = new Dictionary<string, object>();
            _times = new Dictionary<string, DateTime>();
            _readerWriterLock = new ReaderWriterLock();
            _tMain = new Thread(() =>
            {
                while (true)
                {
                    IEnumerable<string> clearKeys;
                    _readerWriterLock.AcquireReaderLock(-1);
                    try
                    {
                        clearKeys = from time in _times where time.Value <= DateTime.Now orderby time.Value select time.Key;
                    }
                    finally
                    {
                        _readerWriterLock.ReleaseReaderLock();
                    }
                    _readerWriterLock.AcquireWriterLock(-1);
                    var t = new Stopwatch();
                    t.Start();
                    try
                    {
                        foreach (var clearKey in clearKeys)
                        {
                            if (t.ElapsedMilliseconds > 100)
                            {
                                _readerWriterLock.ReleaseWriterLock();
                                Thread.Sleep(2);
                                _readerWriterLock.AcquireWriterLock(-1);
                                t.Reset();
                            }
                            if (_caches.ContainsKey(clearKey))
                            {
                                _caches.Remove(clearKey);
                                _times.Remove(clearKey);
                            }
                        }
                    }
                    finally
                    {
                        _readerWriterLock.ReleaseWriterLock();
                        t.Stop();
                    }
                    if (_threadexit)
                        break;
                    Thread.Sleep(timingcheck);
                }
            }) { IsBackground = true };
            if (timingcheck != TimeSpan.Zero)
                _tMain.Start();
        }

        public bool Set(string key, object value)
        {
            var expiry = Timeout == TimeSpan.Zero ? DateTime.MaxValue : DateTime.Now.Add(Timeout);
            return Set(key, value, expiry);
        }

        public bool Set(string key, object value, DateTime effectiveDate)
        {
            _readerWriterLock.AcquireWriterLock(-1);
            try
            {
                if(_caches.ContainsKey(key))
                {
                    _caches[key] = value;
                    _times[key] = effectiveDate;
                }
                else
                {
                    _caches.Add(key, value);
                    _times.Add(key, effectiveDate);
                }
            }
            catch (Exception er)
            {
                LogUtil.WriteLog("Objcached.Set", er);
                return false;
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
            return true;
        }

        public object Get()
        {
            _readerWriterLock.AcquireReaderLock(-1);
            try
            {
                switch (_caches.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return _caches.Values.ToArray()[0];
                    default:
                        var r = new Random((int)DateTime.Now.Ticks);
                        return _caches.Values.ToArray()[r.Next(0, _caches.Count - 1)];
                }
            }
            catch(Exception er)
            {
                LogUtil.WriteLog("Objcached.Get", er);
                return null;
            }
            finally
            {
                _readerWriterLock.ReleaseReaderLock();
            }
        }

        public object Get(string key)
        {
            _readerWriterLock.AcquireReaderLock(-1);
            try
            {
                if (_caches.ContainsKey(key))
                {
                    return _caches[key];
                }
            }
            catch (Exception er)
            {
                LogUtil.WriteLog("Objcached.Get_key", er);
                return null;
            }
            finally
            {
                _readerWriterLock.ReleaseReaderLock();
            }
            return null;
        }

        public ICollection<string> Keys
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    return _caches.Keys;
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Objcached.Keys", er);
                    return new List<string>();
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public ICollection<object> Values
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    return _caches.Values;
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Objcached.Keys", er);
                    return new Collection<object>();
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public ICollection<DateTime> Times
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    return _times.Values;
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Objcached.Times", er);
                    return new Collection<DateTime>();
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public bool Remove(string key)
        {
            _readerWriterLock.AcquireWriterLock(-1);
            try
            {
                if (_caches.ContainsKey(key))
                {
                    _caches.Remove(key);
                    _times.Remove(key);
                }
            }
            catch (Exception er)
            {
                LogUtil.WriteLog("Objcached.Remove_key", er);
                return false;
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
            return true;
        }

        public void Clear()
        {
            _readerWriterLock.AcquireWriterLock(-1);
            try
            {
                _caches.Clear();
                _times.Clear();
            }
            catch (Exception er)
            {
                LogUtil.WriteLog("Objcached.Clear", er);
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
        }

        public object[][] List
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    //var keys = new string[_caches.Count];
                    //_caches.Keys.CopyTo(keys, 0);
                    return _caches.OrderBy(obj => obj.Key).Select(obj => new[] { obj.Key, obj.Value, _times[obj.Key] }).ToArray();
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Objcached.List", er);
                    return new object[3][];
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public int Count
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    return _caches.Count;
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Objcached.Count", er);
                    return 0;
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public void Dispose()
        {
            Clear();
            _threadexit = true;
            _tMain?.Abort();
        }
    }
}
