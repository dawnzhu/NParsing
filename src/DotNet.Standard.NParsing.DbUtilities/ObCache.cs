/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2011-11-07 14:00:00
* 版 本 号：2.2.0
* 功能说明：缓存类(增加了SQL语句功能)
* ----------------------------------
* 修改标识：
* 修 改 人：
* 日    期：
* 版 本 号：
* 修改内容：
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using DotNet.Standard.Common.Utilities;
using DotNet.Standard.Common.Xml;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public static class ObCache
    {
        private static Mutex _directMutex;
        private static readonly Hashtable HCacheMemory = new Hashtable();
        public static string CachePath { get; private set; }//存放路径
        private const string FileName = @"\{0}";//文件名GUID
        private static string _exteName = ".xml";//缓存不加密默认为xml，加密为dat
        public static int FileSize { get; private set; }//缓存分文件存放，单个文件不大于fileSize，单位为MB
        public static ObCacheMode CacheMode { get; private set; }//是不是进行缓存，ON开启，OFF关闭，SECRET开启并加密
        public static bool Initialized { get; private set; }
        static ObCache()
        {
            CachePath = $@"{Directory.GetCurrentDirectory()}/NParsing";
            FileSize = 1;
            CacheMode = ObCacheMode.Off;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mode">存储模式</param>
        public static void Initialize(ObCacheMode mode)
        {
            CacheMode = mode;
            Initialize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mode">存储模式</param>
        /// <param name="fileSize">文件大小 MB</param>
        public static void Initialize(ObCacheMode mode, int fileSize)
        {
            CacheMode = mode;
            FileSize = fileSize;
            Initialize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mode">存储模式</param>
        /// <param name="path">存储路径</param>
        public static void Initialize(ObCacheMode mode, string path)
        {
            CacheMode = mode;
            CachePath = path;
            Initialize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mode">存储模式</param>
        /// <param name="fileSize">文件大小 MB</param>
        /// <param name="path">存储路径</param>
        public static void Initialize(ObCacheMode mode, int fileSize, string path)
        {
            CacheMode = mode;
            CachePath = path;
            FileSize = fileSize;
            Initialize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            _directMutex = new Mutex(false, SecretUtil.MD5Encrypt32(CachePath));
            var doc = new XmlDocument();
            switch (CacheMode)
            {
                case ObCacheMode.On:
                    break;
                case ObCacheMode.Off:
                    return;
                case ObCacheMode.Secret:
                    _exteName = ".dat";
                    doc = new XmlEDocument();
                    break;
            }
            if (!Directory.Exists(CachePath)) Directory.CreateDirectory(CachePath);
            foreach (var filePath in Directory.GetFiles(CachePath, "*" + _exteName))
            {
                try
                {
                    doc.Load(filePath);
                }
                catch
                {
                    continue;
                }
                var obcache = doc.SelectSingleNode("obcache");
                if (obcache == null) return;
                foreach (XmlElement childNode in obcache.ChildNodes)
                {
                    var model = new ObSqlcache
                    {
                        Version = childNode.GetAttribute("version"),
                        SqlText = childNode.ChildNodes[0].InnerText
                    };
                    var key = childNode.GetAttribute("key");
                    if (childNode.ChildNodes[1].InnerText.Length > 0)
                    {
                        var columns = childNode.ChildNodes[1].InnerText.Split(',');
                        model.ColumnNames = new List<string>();
                        foreach (var column in columns)
                        {
                            model.ColumnNames.Add(column);
                        }
                    }
                    lock (HCacheMemory)
                    {
                        try
                        {
                            if (!HCacheMemory.ContainsKey(key))
                            {
                                HCacheMemory.Add(key, model);
                            }
                        }
                        catch
                        {
                            //continue;
                        }
                    }
                }
            }
            Initialized = true;
        }

        public static void Add(string key, ObSqlcache model)
        {
            var doc = new XmlDocument();
            switch (CacheMode)
            {
                case ObCacheMode.On:
                    break;
                case ObCacheMode.Off:
                    return;
                case ObCacheMode.Secret:
                    doc = new XmlEDocument();
                    break;
            }
            lock (HCacheMemory)
            {
                if (HCacheMemory.Contains(key))
                    HCacheMemory[key] = model;
                else
                    HCacheMemory.Add(key, model);

                var di = new DirectoryInfo(CachePath);
                var files = di.GetFiles("*" + _exteName);
                var fc = new FileComparer();
                Array.Sort(files, fc);
                string filePath;
                if (files.Length != 0 && files[files.Length - 1].Length < FileSize * 1024 * 1024)
                    filePath = files[files.Length - 1].FullName;
                else
                    filePath = CachePath + string.Format(FileName, Guid.NewGuid()) + _exteName;

                _directMutex.WaitOne();
                try
                {
                    XmlNode obcache;
                    if (!File.Exists(filePath))
                    {
                        var dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                        doc.AppendChild(dec);
                        obcache = doc.CreateElement("obcache");
                        doc.AppendChild(obcache);
                    }
                    else
                    {
                        doc.Load(filePath);
                        obcache = doc.SelectSingleNode("obcache");
                    }
                    var obsqlcacheNode = (XmlElement)doc.SelectSingleNode($"/obcache/obsqlcache[@key='{key}']");
                    if (obsqlcacheNode == null)
                    {
                        var obsqlcache = doc.CreateElement("obsqlcache");
                        obsqlcache.SetAttribute("key", key);
                        if (!string.IsNullOrEmpty(model.Version))
                        {
                            obsqlcache.SetAttribute("version", model.Version);
                        }
                        var sqltext = doc.CreateElement("sqltext");
                        sqltext.InnerText = model.SqlText;
                        var columns = doc.CreateElement("columns");
                        columns.InnerText = model.ColumnNamesToString();
                        obsqlcache.AppendChild(sqltext);
                        obsqlcache.AppendChild(columns);
                        obcache?.AppendChild(obsqlcache);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(model.Version))
                        {
                            obsqlcacheNode.SetAttribute("version", model.Version);
                        }
                        var sqltext = obsqlcacheNode.ChildNodes[0];
                        var columns = obsqlcacheNode.ChildNodes[1];
                        sqltext.InnerText = model.SqlText;
                        columns.InnerText = model.ColumnNamesToString();
                    }
                    doc.Save(filePath);
                }
                finally
                {
                    _directMutex.ReleaseMutex();   
                }
            }
        }

        public static ObSqlcache Value(string key)
        {
            if(CacheMode == ObCacheMode.Off/* || obDebug*/) return null;
            lock (HCacheMemory)
            {
                if(HCacheMemory.Contains(key))
                    return (ObSqlcache) HCacheMemory[key];
                return null;
            }
        }
    }

    public enum ObCacheMode
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Off = 1,

        /// <summary>
        /// 开启
        /// </summary>
        On = 2,

        /// <summary>
        /// 加密开启
        /// </summary>
        Secret = 4
    }

    public class ObSqlcache
    {
        public string Version { get; set; }
        public string SqlText { get; set; }
        public IList<string> ColumnNames { get; set; }
        public string ColumnNamesToString()
        {
            var columnnameString = string.Empty;
            if (ColumnNames != null)
            {
                foreach (var columnName in ColumnNames)
                {
                    if (columnnameString.Length != 0)
                        columnnameString += ",";
                    columnnameString += columnName;
                }
            }
            return columnnameString;
        }
    }

    /// <summary>
    /// 文件排序（创建时间）
    /// </summary>
    public class FileComparer : IComparer
    {
        int IComparer.Compare(Object o1, Object o2)
        {
            var fi1 = (FileInfo)o1;
            var fi2 = (FileInfo)o2;
            return fi1.LastWriteTime.CompareTo(fi2.LastWriteTime);
        }
    }
}
