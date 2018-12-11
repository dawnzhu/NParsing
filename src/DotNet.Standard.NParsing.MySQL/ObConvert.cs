using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using DotNet.Standard.Utilities;
using MySql.Data.MySqlClient;

namespace DotNet.Standard.NParsing.MySQL
{
    public static class ObConvert
    {
        public static MySqlDbType ToDbType(this Type t)
        {
            if(t == typeof(string))
                return MySqlDbType.VarChar;
            if (t == typeof(short) || t == typeof(short?) || t == typeof(ushort) || t == typeof(ushort?))
                return MySqlDbType.Int16;
            if (t == typeof(int) || t == typeof(int?) || t == typeof(uint) || t == typeof(uint?))
                return MySqlDbType.Int32;
            if (t == typeof(long) || t == typeof(long?) || t == typeof(ulong) || t == typeof(ulong?))
                return MySqlDbType.Int64;
            if (t == typeof(double) || t == typeof(double?))
                return MySqlDbType.Double;
            if (t == typeof(float) || t == typeof(float?))
                return MySqlDbType.Float;
            if (t == typeof(decimal) || t == typeof(decimal?))
                return MySqlDbType.Decimal;
            if (t == typeof(char) || t == typeof(char?) || t == typeof(char[]) || t == typeof(char?[]))
                return MySqlDbType.VarChar;
            if (t == typeof(byte) || t == typeof(byte?))
                return MySqlDbType.Byte;
            if (t == typeof(byte[]) || t == typeof(byte?[]))
                return MySqlDbType.VarBinary;
            if(t == typeof(DateTime) || t == typeof(DateTime?))
                return MySqlDbType.DateTime;
            if(t == typeof(bool) || t == typeof(bool?))
                return MySqlDbType.Bit;
            return MySqlDbType.Set;
        }

        public static string ToDbTypeString(this Type t)
        {
            if (t == typeof(string))
                return "VARCHAR({0})";
            if (t == typeof(short) || t == typeof(short?) || t == typeof(ushort) || t == typeof(ushort?))
                return "SMALLINT";
            if (t == typeof(int) || t == typeof(int?) || t == typeof(uint) || t == typeof(uint?))
                return "INT";
            if (t == typeof(long) || t == typeof(long?) || t == typeof(ulong) || t == typeof(ulong?))
                return "BIGINT";
            if (t == typeof(double) || t == typeof(double?))
                return "DOUBLE";
            if (t == typeof(float) || t == typeof(float?))
                return "FLOAT";
            if (t == typeof(decimal) || t == typeof(decimal?))
                return "DECIMAL({0},{1})";
            if (t == typeof(char) || t == typeof(char?) || t == typeof(char[]) || t == typeof(char?[]))
                return "CHAR({0})";
            if (t == typeof(byte) || t == typeof(byte?))
                return "VARBINARY(1)";
            if (t == typeof(byte[]) || t == typeof(byte?[]))
                return "VARBINARY({0})";
            if (t == typeof(DateTime) || t == typeof(DateTime?))
                return "DATETIME";
            if (t == typeof(bool) || t == typeof(bool?))
                return "BIT";
            return "SET";
        }

        /// <summary>
        /// 防止参数名称重复，并重用参数
        /// </summary>
        /// <param name="dbParameters"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IList<DbParameter> Add(this IList<DbParameter> dbParameters, ref string name, object value)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }
            if (value.IsEnum())
            {
                value = Convert.ToDecimal(value);
            }
            name = "@" + name;
            MySqlParameter sqlParameter = null;
            var i = 0;
            foreach (var parameter in dbParameters)
            {
                if (parameter.ParameterName.StartsWith(name))
                {
                    if (parameter.Value.Equals(value))
                    {
                        sqlParameter = (MySqlParameter)parameter;
                        name = parameter.ParameterName;
                        break;
                    }
                    i++;
                }
            }
            if (sqlParameter == null)
            {
                name += i == 0 ? "" : i.ToString(CultureInfo.InvariantCulture);
                sqlParameter = new MySqlParameter { ParameterName = name };
                if (value.IsString())
                {
                    var v = value.ToString();
                    sqlParameter.MySqlDbType = MySqlDbType.VarChar;
                    sqlParameter.Size = v.Length == 0 ? 1 : v.Length;
                }
                sqlParameter.Value = value;
                dbParameters.Add(sqlParameter);
            }
            return dbParameters;
        }
    }
}
