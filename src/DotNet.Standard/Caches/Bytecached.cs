using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DotNet.Standard.Utilities;

namespace DotNet.Standard.Caches
{
    public class Bytecached : IDisposable
    {
        private readonly IDictionary<string, byte[]> _caches;
        private readonly IDictionary<string, DateTime> _times;
        private long _size;
        private readonly ReaderWriterLock _readerWriterLock;
        private readonly BackgroundWorker _checkWorker;
        //private const int _millisecondsTimeout = 1000;

        /// <summary>
        /// 最大容量/B
        /// </summary>
        public long MaxSize { get; set; }

        /// <summary>
        /// 是否启用缓存
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// 一次清理数量
        /// </summary>
        public int ClearCount { get; set; }

        /// <summary>
        /// 是否正在检查并清理超时缓存
        /// </summary>
        public bool Checking { get; private set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="timingcheck">定时检查时间</param>
        public Bytecached(TimeSpan timingcheck = default(TimeSpan))
        {
            _caches = new Dictionary<string, byte[]>();
            _times = new Dictionary<string, DateTime>();
            _readerWriterLock = new ReaderWriterLock();
            MaxSize = 64*1024*1024;
            Enabled = true;
            ClearCount = 1;

            #region 检查并清理缓存

            if (timingcheck == default(TimeSpan))
            {
                _checkWorker = new BackgroundWorker();
                _checkWorker.DoWork += CheckWorker_DoWork;
            }
            else
            {
                new Thread(obj =>
                {
                    while (true)
                    {
                        var swt = new Stopwatch();
                        swt.Start();
                        CheckWorker_DoWork(null, null);
                        swt.Stop();
                        //LogUtil.WriteLog("清理时间：" + swt.ElapsedMilliseconds + "ms\r\n---------------------------------------------------------------------");
                        Thread.Sleep(timingcheck);
                    }
                }) { IsBackground = true }.Start();
            }

            #endregion
        }

        public bool Set(string key, byte[] value)
        {
            var expiry = Timeout == TimeSpan.Zero ? DateTime.MaxValue : DateTime.Now.Add(Timeout);
            return Set(key, value, expiry);
        }

        public bool Set(string key, byte[] value, DateTime effectiveDate)
        {
            if (!Enabled)
                return false;

            _readerWriterLock.AcquireWriterLock(-1);
            try
            {
                if (_caches.ContainsKey(key))
                {
                    _size -= _caches[key].Length;
                    _caches[key] = value;
                    _times[key] = effectiveDate;
                }
                else
                {
                    _caches.Add(key, value);
                    _times.Add(key, effectiveDate);
                }
                _size += value.Length;
            }
            catch(Exception er)
            {
                LogUtil.WriteLog("Bytecached.Set", er);
                return false;
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }

            #region 检查并清理缓存

            try
            {
                if (_checkWorker != null && !_checkWorker.IsBusy)
                {
                    _checkWorker.RunWorkerAsync();
                }
            }
            catch (Exception er)
            {
                LogUtil.WriteLog("检查并清理缓存", er);
            }

            #endregion

            return true;
        }

        public byte[] Get(string key)
        {
            var expiry = Timeout == TimeSpan.Zero ? default(DateTime) : DateTime.Now.Add(Timeout);
            return Get(key, expiry);
        }

        public byte[] Get(string key, DateTime effectiveDate)
        {
            if (!Enabled)
                return null;

            _readerWriterLock.AcquireReaderLock(-1);
            var exist = false;
            try
            {
                exist = _caches.TryGetValue(key, out var v);
                return v;
            }
            catch(Exception er)
            {
                LogUtil.WriteLog("Bytecached.Get", er);
                return null;
            }
            finally
            {
                _readerWriterLock.ReleaseReaderLock();
                //if (_caches.ContainsKey(key))
                if (exist && effectiveDate != default(DateTime))
                {
                    #region 刷新缓存

                    var t = new Thread(obj =>
                    {
                        var k = (string)((object[])obj)[0];
                        var edate = (DateTime)((object[])obj)[1];
                        while (!Refresh(k, edate))
                        {
                            Thread.Sleep(100);
                        }
                        ((object[]) obj)[2] = null;
                    }) { IsBackground = true };
                    t.Start(new object[] {key, effectiveDate, t});

                    #endregion
                }
            }
        }

        public bool ContainsKey(string key)
        {
            if (!Enabled)
                return false;

            _readerWriterLock.AcquireReaderLock(-1);
            try
            {
                return _caches.ContainsKey(key);
            }
            catch(Exception er)
            {
                LogUtil.WriteLog("Bytecached.ContainsKey", er);
                return false;
            }
            finally
            {
                _readerWriterLock.ReleaseReaderLock();
            }
        }

        public bool Refresh(string key)
        {
            return Timeout != TimeSpan.Zero && Refresh(key, DateTime.Now.Add(Timeout));
        }

        public bool Refresh(string key, DateTime effectiveDate)
        {
            _readerWriterLock.AcquireWriterLock(-1);
            try
            {
                if (_caches.ContainsKey(key))
                {
                    _times[key] = effectiveDate;
                }
            }
            catch(Exception er)
            {
                LogUtil.WriteLog("Bytecached.Refresh", er);
                return false;
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
            return true;
        }

        public IList<string> Keys
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    return _caches.Keys.ToList();
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Bytecached.Keys", er);
                    return new List<string>();
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public ICollection<byte[]> Values
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
                    LogUtil.WriteLog("Bytecached.Keys", er);
                    return new List<byte[]>();
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
                    LogUtil.WriteLog("Bytecached.Times", er);
                    return new Collection<DateTime>();
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// 移除指定数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            _readerWriterLock.AcquireWriterLock(-1);
            try
            {
                if (_caches.ContainsKey(key))
                {
                    _size -= _caches[key].Length;
                    _caches.Remove(key);
                    _times.Remove(key);
                }
            }
            catch(Exception er)
            {
                LogUtil.WriteLog("Bytecached.Remove_key", er);
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
                _size = 0;
                _caches.Clear();
                _times.Clear();
            }
            catch(Exception er)
            {
                LogUtil.WriteLog("Bytecached.Clear", er);
            }
            finally
            {
                _readerWriterLock.ReleaseWriterLock();
            }
        }

        private void CheckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Checking = true;
            try
            {
                bool clearSize;
                bool collect;
                IEnumerable<string> clearKeys;
                var count = 0;
                //var stopCount = 0;
                var swt = new Stopwatch();
                var t = new Stopwatch();

                #region 清理超时记录

                _readerWriterLock.AcquireReaderLock(-1);
                //LogUtil.WriteLog("开始清理----------------------------------------------------");
                swt.Start();
                try
                {
                    clearKeys = from time in _times where time.Value <= DateTime.Now orderby time.Value select time.Key;
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                    swt.Stop();
                    //LogUtil.WriteLog("获取超时列表用时" + swt.ElapsedMilliseconds + "ms");
                }
                Thread.Sleep(10);
                _readerWriterLock.AcquireWriterLock(-1);
                t.Start();
                swt.Reset();
                swt.Start();
                try
                {
                    foreach (var clearKey in clearKeys)
                    {
                        if (t.ElapsedMilliseconds > 20)
                        {
                            //stopCount++;
                            _readerWriterLock.ReleaseWriterLock();
                            Thread.Sleep(10);
                            _readerWriterLock.AcquireWriterLock(-1);
                            t.Reset();
                        }
                        if (_caches.ContainsKey(clearKey))
                        {
                            _size -= _caches[clearKey].Length;
                            _caches.Remove(clearKey);
                            _times.Remove(clearKey);
                        }
                        count++;
                    }
                }
                finally
                {
                    clearSize = _size >= MaxSize;
                    collect = count > 0 || clearSize;
                    _readerWriterLock.ReleaseWriterLock();
                    t.Stop();
                    swt.Stop();
                    //LogUtil.WriteLog("超时清理了" + count + "条记录，停顿了" + stopCount + "次，用时" + swt.ElapsedMilliseconds + "ms");
                }

                #endregion

                if (clearSize)
                {
                    #region 清理超量记录

                    Thread.Sleep(10);
                    _readerWriterLock.AcquireReaderLock(-1);
                    swt.Reset();
                    swt.Start();
                    try
                    {
                        clearKeys = from time in _times orderby time.Value select time.Key;
                    }
                    finally
                    {
                        _readerWriterLock.ReleaseReaderLock();
                        swt.Stop();
                        //LogUtil.WriteLog("获取所有列表用时" + swt.ElapsedMilliseconds + "ms");
                    }
                    Thread.Sleep(10);
                    _readerWriterLock.AcquireWriterLock(-1);
                    t.Reset();
                    t.Start();
                    swt.Reset();
                    swt.Start();
                    count = 0;
                    //stopCount = 0;
                    try
                    {
                        var i = 0;
                        foreach (var clearKey in clearKeys)
                        {
                            if (i == ClearCount - 1)
                                i = 0;
                            if (i == 0 && _size < MaxSize)
                                break;
                            if (t.ElapsedMilliseconds > 20)
                            {
                                //stopCount++;
                                _readerWriterLock.ReleaseWriterLock();
                                Thread.Sleep(10);
                                _readerWriterLock.AcquireWriterLock(-1);
                                t.Reset();
                            }
                            if (_caches.ContainsKey(clearKey))
                            {
                                _size -= _caches[clearKey].Length;
                                _caches.Remove(clearKey);
                                _times.Remove(clearKey);
                            }
                            i++;
                            count++;
                        }
                    }
                    finally
                    {
                        _readerWriterLock.ReleaseWriterLock();
                        t.Stop();
                        swt.Stop();
                        //LogUtil.WriteLog("超量清理了" + count + "条记录，停顿了" + stopCount + "次，用时" + swt.ElapsedMilliseconds + "ms");
                    }

                    #endregion
                }

                if (collect)
                {
                    GC.Collect();
                }
            }
            finally
            {
                Checking = false;
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
                catch(Exception er)
                {
                    LogUtil.WriteLog("Bytecached.Count", er);
                    return 0;
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public long Size
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    return _size;
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Bytecached.Size", er);
                    return 0;
                }
                finally
                {
                    _readerWriterLock.ReleaseReaderLock();
                }
            }
        }

        public object[][] List
        {
            get
            {
                _readerWriterLock.AcquireReaderLock(-1);
                try
                {
                    return _caches.Select(obj => new object[] { obj.Key, obj.Value.Length, _times[obj.Key] }).ToArray();
                }
                catch (Exception er)
                {
                    LogUtil.WriteLog("Bytecached.List", er);
                    return new object[3][];
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
            _checkWorker.Dispose();
        }
    }
}
