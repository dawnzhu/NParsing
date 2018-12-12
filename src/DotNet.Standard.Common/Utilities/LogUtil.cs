using System;
using System.IO;
using System.Threading;

namespace DotNet.Standard.Common.Utilities
{
    public class LogUtil
    {
        private static Mutex _directMutex;
        private static string _directPath;
        private static string _mark;

        static LogUtil()
        {
            try
            {
                _mark = "";
                /*AppDomain.CurrentDomain.SetupInformation.ApplicationBase*//*AppContext.BaseDirectory*/
                _directPath = $@"{Directory.GetCurrentDirectory()}/log";
                if (!Directory.Exists(_directPath))
                {
                    Directory.CreateDirectory(_directPath);
                }
                _directMutex = new Mutex(false, SecretUtil.MD5Encrypt32(_directPath));
            }
            catch { }
        }

        public static void SetDirectory(string dir)
        {
            try
            {
                if (!string.IsNullOrEmpty(dir))
                {
                    _directPath += "\\" + dir;
                    if (!Directory.Exists(_directPath))
                    {
                        Directory.CreateDirectory(_directPath);
                    }
                    _directMutex = new Mutex(false, SecretUtil.MD5Encrypt32(_directPath));
                }
            }
            catch { }
        }

        public static void SetMark(string value)
        {
            _mark = value;
        }

        /// <summary>
        /// 在本地写入错误日志
        /// </summary>
        /// <param name="er"></param>
        /// 错误信息
        public static void WriteLog(Exception er)
        {
            WriteLog(null, er);
        }

        public static void WriteLog(string title, Exception er)
        {
            try
            {
                _directMutex.WaitOne();
                StreamWriter sw = null;
                try
                {
                    var dt = DateTime.Now;
                    if (!Directory.Exists(_directPath))
                    {
                        Directory.CreateDirectory(_directPath);
                    }
                    var filePath = _directPath + string.Format(@"\{0}.log", dt.ToString("yyyy-MM-dd"));
                    sw = !File.Exists(filePath) ? File.CreateText(filePath) : File.AppendText(filePath);
                    sw.WriteLine("{0}\t{1}", dt.ToString("HH:mm:ss"), _mark);
                    if (string.IsNullOrEmpty(title))
                        title = "异常信息：";
                    sw.WriteLine(title);
                    if (er != null)
                    {
                        sw.WriteLine(er.Message);
                        sw.WriteLine(er.StackTrace);
                    }
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                    _directMutex.ReleaseMutex();
                }
            }
            catch { }
        }

        public static void WriteLog(string message)
        {
            WriteLog(null, message);
        }

        public static void WriteLog(string title, string message)
        {
            try
            {
                _directMutex.WaitOne();
                StreamWriter sw = null;
                try
                {
                    var dt = DateTime.Now;
                    if (!Directory.Exists(_directPath))
                    {
                        Directory.CreateDirectory(_directPath);
                    }
                    var filePath = _directPath + string.Format(@"\{0}.log", dt.ToString("yyyy-MM-dd"));
                    sw = !File.Exists(filePath) ? File.CreateText(filePath) : File.AppendText(filePath);
                    sw.WriteLine("{0}\t{1}", dt.ToString("HH:mm:ss"), _mark);
                    if (!string.IsNullOrEmpty(title))
                        sw.WriteLine(title);
                    sw.WriteLine(message);
                    sw.WriteLine();
                    sw.Flush();
                    sw.Close();
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                    _directMutex.ReleaseMutex();
                }
            }
            catch { }
        }
    }
}