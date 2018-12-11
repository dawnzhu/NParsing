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
using DotNet.Standard.Utilities;
using DotNet.Standard.Xml;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public static class ObCache
    {
        private static readonly Mutex DirectMutex;
        private static readonly Hashtable HCacheMemory = new Hashtable();
        private static readonly string ObCachePath = $@"{Directory.GetCurrentDirectory()}/NParsing";//存放路径
        private const string FileName = @"\{0}";//文件名GUID
        private static string _exteName = ".xml";//缓存不加密默认为xml，加密为dat
        private static readonly long FileSize = 512 * 1024;//缓存分文件存放，单个文件不大于fileSize，单位为B
        private static string _obCache = "OFF";//是不是进行缓存，ON开启，OFF关闭，SECRET开启并加密
        //private static readonly bool obDebug;

        static ObCache()
        {
            DirectMutex = new Mutex(false, SecretUtil.MD5Encrypt32(ObCachePath));
            /*if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ObCache"])) obCache = ConfigurationManager.AppSettings["ObCache"];
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ObCachePath"])) obCachePath = ConfigurationManager.AppSettings["ObCachePath"];
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ObCacheSize"]))
            {
                var fs = ConfigurationManager.AppSettings["ObCacheSize"];
                switch (fs.Substring(fs.Length - 1, 1).ToUpper())
                {
                    case "B":
                        fileSize = Convert.ToInt64(fs.Substring(0, fs.Length - 1));
                        break;
                    case "K":
                        fileSize = Convert.ToInt64(fs.Substring(0, fs.Length - 1)) * 1024 * 1024;
                        break;
                    case "M":
                        fileSize = Convert.ToInt64(fs.Substring(0, fs.Length - 1)) * 1024 * 1024 * 1024;
                        break;
                    case "G":
                        fileSize = Convert.ToInt64(fs.Substring(0, fs.Length - 1)) * 1024 * 1024 * 1024 * 1024;
                        break;
                    case "T":
                        fileSize = Convert.ToInt64(fs.Substring(0, fs.Length - 1)) * 1024 * 1024 * 1024 * 1024 * 1024;
                        break;
                }
                //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ObDebug"])) obDebug = Convert.ToBoolean(ConfigurationManager.AppSettings["ObDebug"]);
            }*/
        }

        public static void Initialize(string mode)
        {
            _obCache = mode;
            var doc = new XmlDocument();
            switch (_obCache)
            {
                case "ON":
                    break;
                case "OFF":
                    return;
                case "SECRET":
                    _exteName = ".dat";
                    doc = new XmlEDocument();
                    break;
            }
            if (!Directory.Exists(ObCachePath)) Directory.CreateDirectory(ObCachePath);
            foreach (var filePath in Directory.GetFiles(ObCachePath, "*" + _exteName))
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
        }

        public static void Add(string key, ObSqlcache model)
        {
            var doc = new XmlDocument();
            switch (_obCache)
            {
                case "ON":
                    break;
                case "OFF":
                    return;
                case "SECRET":
                    doc = new XmlEDocument();
                    break;
            }
            lock (HCacheMemory)
            {
                if (HCacheMemory.Contains(key))
                    HCacheMemory[key] = model;
                else
                    HCacheMemory.Add(key, model);

                var di = new DirectoryInfo(ObCachePath);
                var files = di.GetFiles("*" + _exteName);
                var fc = new FileComparer();
                Array.Sort(files, fc);
                string filePath;
                if (files.Length != 0 && files[files.Length - 1].Length < FileSize)
                    filePath = files[files.Length - 1].FullName;
                else
                    filePath = ObCachePath + string.Format(FileName, Guid.NewGuid()) + _exteName;

                DirectMutex.WaitOne();
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
                    DirectMutex.ReleaseMutex();   
                }
            }
        }

        public static ObSqlcache Value(string key)
        {
            if(_obCache == "OFF"/* || obDebug*/) return null;
            lock (HCacheMemory)
            {
                if(HCacheMemory.Contains(key))
                    return (ObSqlcache) HCacheMemory[key];
                return null;
            }
        }
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
