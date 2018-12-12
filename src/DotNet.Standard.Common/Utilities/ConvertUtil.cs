using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNet.Standard.Common.Utilities
{
    public static class ConvertUtil
    {
        /// <summary>
        /// 默认编码
        /// </summary>
        private static readonly Encoding _defaultEncoding;

        static ConvertUtil()
        {
            _defaultEncoding = Encoding.UTF8;
        }

        public static EndPoint ToEndPoint(this string host)
        {
            IPAddress address;
            int port;
            var hs = host.Split(':');
            if (hs.Length != 2)
                throw new Exception("host格式不正确 host=" + host);
            if (!IPAddress.TryParse(hs[0], out address) || !int.TryParse(hs[1], out port))
                throw new Exception("host格式不正确 host=" + host);
            return new IPEndPoint(address, port);
        }

        public static string FromEndPoint(this IPEndPoint host)
        {
            return host.Address + ":" + host.Port;
        }

        /// <summary>
        /// 将字节数组以UTF-8格式转成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUTF8String(this byte[] value)
        {
            return ToString(value, _defaultEncoding);
        }

        /// <summary>
        /// 将字符串转成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string ToString(this byte[] value, Encoding encoding)
        {
            if (value == null || (value.Length == 1 && value[0] == '\0'))
                return null;
            return encoding.GetString(value);
        }

        /// <summary>
        /// 将字符串以UTF-8格式转成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value)
        {
            return ToBytes(value, _defaultEncoding);
        }

        /// <summary>
        /// 将字节数组转成字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string value, Encoding encoding)
        {
            if (value == null)
            {
                return new[] { (byte)'\0' };
            }
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 将对象转成字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this object value)
        {
            byte[] buffer;
            if (value != null)
            {
                var bf = new BinaryFormatter();
                using (var rms = new MemoryStream())
                {
                    bf.Serialize(rms, value);
                    buffer = rms.ToArray();
                    rms.Close();
                }
            }
            else
            {
                buffer = new[] { (byte)'\0' };
            }
            return buffer;
        }

        /// <summary>
        /// 将字节数组转成对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ToObject(this byte[] value)
        {
            if (value == null || (value.Length == 1 && value[0] == '\0'))
                return null;
            object obj;
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream(value))
            {
                ms.Position = 0;
                obj = bf.Deserialize(ms);
                ms.Close();
            }
            return obj;
        }

        private static readonly string[] _dws = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "BB" };
        private static string ToDataUnit(long value, string dw, bool viewByte)
        {
            var size = (double)value;
            var index = 0;
            while (size / 1024 >= 1 && index < _dws.Length - 1 && _dws[index] != dw)
            {
                size = size / 1024.0;
                index++;
            }
            var strValue = Math.Round(size, 2, MidpointRounding.AwayFromZero) + " " + _dws[index];
            if (viewByte)
                strValue += " (" + value.ToString("N0") + " 字节)";
            return strValue;
        }

        public static string ToDataUnit(this long value, bool viewByte = false)
        {
            return ToDataUnit(value, null, viewByte);
        }

        public static string ToDataUnit(this ulong value, bool viewByte = false)
        {
            return ToDataUnit((long)value, null, viewByte);
        }

        public static string ToDataUnit(this int value, bool viewByte = false)
        {
            return ToDataUnit(value, null, viewByte);
        }

        public static string ToDataUnit(this uint value, bool viewByte = false)
        {
            return ToDataUnit(value, null, viewByte);
        }

        public static string ToKiloByte(this long value, bool viewByte = false)
        {
            return ToDataUnit(value, "KB", viewByte);
        }

        public static string ToKiloByte(this ulong value, bool viewByte = false)
        {
            return ToDataUnit((long)value, "KB", viewByte);
        }

        public static string ToKiloByte(this int value, bool viewByte = false)
        {
            return ToDataUnit(value, "KB", viewByte);
        }

        public static string ToKiloByte(this uint value, bool viewByte = false)
        {
            return ToDataUnit(value, "KB", viewByte);
        }

        public static object ToChangeType(this object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            var s = value as string;
            if (type.IsEnum)
            {
                return s != null ? Enum.Parse(type, s) : Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                var innerType = type.GetGenericArguments()[0];
                var innerValue = ToChangeType(value, innerType);
                return Activator.CreateInstance(type, innerValue);
            }
            if (s != null && type == typeof(Guid)) return new Guid(s);
            if (s != null && type == typeof(Version)) return new Version((string)value);
            if (!(value is IConvertible)) return value;
            return Convert.ChangeType(value, type);
        }

        #region 当前类型为系统类型 public static bool IsSystem(this object value)

        /// <summary>
        /// 当前类型为系统类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsSystem(this object value)
        {
            return value != null && IsSystem(value.GetType());
            /*            return (value is string) ||
                               (value is short) ||
                               (value is short?) ||
                               (value is int) ||
                               (value is int?) ||
                               (value is long) ||
                               (value is long?) ||
                               (value is double) ||
                               (value is double?) ||
                               (value is decimal) ||
                               (value is decimal?) ||
                               (value is char) ||
                               (value is char?) ||
                               (value is byte) ||
                               (value is byte?) ||
                               (value is byte[]) ||
                               (value is byte?[]) ||
                               (value is DateTime) ||
                               (value is DateTime?) ||
                               (value is bool);*/
        }

        public static bool IsSystem(this Type t)
        {
            return (t == typeof(string)) ||
                   /*(t == typeof (short)) ||
                   (t == typeof (short?)) ||
                   (t == typeof (int)) ||
                   (t == typeof (int?)) ||
                   (t == typeof (long)) ||
                   (t == typeof (long?)) ||
                   (t == typeof (double)) ||
                   (t == typeof (double?)) ||
                   (t == typeof (decimal)) ||
                   (t == typeof (decimal?)) ||
                   (t == typeof (char)) ||
                   (t == typeof (char?)) ||
                   (t == typeof (byte)) ||
                   (t == typeof (byte?)) ||*/
                   (t == typeof(byte[])) ||
                   (t == typeof(byte?[])) ||
                   /*(t == typeof(ushort)) ||
                   (t == typeof(ushort?)) ||
                   (t == typeof(uint)) ||
                   (t == typeof(uint?)) ||
                   (t == typeof(ulong)) ||
                   (t == typeof(ulong?)) ||
                   (t == typeof (DateTime)) ||
                   (t == typeof (DateTime?)) ||
                   (t == typeof (bool)) ||
                   (t == typeof (bool?)) ||*/
                   (t.BaseType == typeof(ValueType)) ||
                   (t.BaseType == typeof(Enum));
        }

        #endregion

        #region 扩展类型转基本类型 public static Type ToBasic(this Type t)

        /// <summary>
        /// 扩展类型转基本类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Type ToBasic(this Type t)
        {
            if (t == typeof(byte?[]))
                t = typeof(byte[]);
            else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                t = t.GetGenericArguments()[0];
            return t;
            /*if (t == typeof (short?))
                t = typeof (short);
            else if (t == typeof (int?))
                t = typeof (int);
            else if (t == typeof(long?))
                t = typeof (long);
            else if (t == typeof(ushort?))
                t = typeof(ushort);
            else if (t == typeof(uint?))
                t = typeof(uint);
            else if (t == typeof(ulong?))
                t = typeof(ulong);
            else if (t == typeof(double?))
                t = typeof(double);
            else if (t == typeof(float?))
                t = typeof(float);
            else if (t == typeof(decimal?))
                t = typeof(decimal);
            else if (t == typeof(char?))
                t = typeof(char);
            else if (t == typeof(byte?))
                t = typeof(byte);
            else if (t == typeof(byte?[]))
                t = typeof(byte[]);
            else if (t == typeof(DateTime?))
                t = typeof(DateTime);
            else if (t == typeof(bool?))
                t = typeof(bool);
            return t;*/
        }

        #endregion

        #region 当前类型是否为字符串类型 public static bool IsString(this object value)

        /// <summary>
        /// 当前类型是否为字符串类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsString(this object value)
        {
            return value != null && IsString(value.GetType());
        }

        /// <summary>
        /// 当前类型是否为字符串类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsString(this Type t)
        {
            return t.ToBasic() == typeof(string);
        }

        /// <summary>
        /// 当前类型是否为枚举类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEnum(this object value)
        {
            return value != null && IsEnum(value.GetType());
        }

        /// <summary>
        /// 当前类型是否为枚举类型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsEnum(this Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments()[0].IsEnum;
            }
            return t.IsEnum;
        }

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsChinese(this string value)
        {
            return value != null && Regex.IsMatch(value, @"[\u4e00-\u9fa5]");
        }

        #endregion
    }
}
