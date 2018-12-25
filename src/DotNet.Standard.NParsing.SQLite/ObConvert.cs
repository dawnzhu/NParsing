using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Standard.NParsing.SQLite
{
    public static class ObConvert
    {
        public static DbType ToDbType(this Type t)
        {
            if(t == typeof(string))
                return DbType.String;
            if (t == typeof(short) || t == typeof(short?) || t == typeof(ushort) || t == typeof(ushort?))
                return DbType.Int16;
            if (t == typeof(int) || t == typeof(int?) || t == typeof(uint) || t == typeof(uint?))
                return DbType.Int32;
            if (t == typeof(long) || t == typeof(long?) || t == typeof(ulong) || t == typeof(ulong?))
                return DbType.Int64;
            if (t == typeof(double) || t == typeof(double?))
                return DbType.Double;
            if (t == typeof(float) || t == typeof(float?))
                return DbType.Single;
            if (t == typeof(decimal) || t == typeof(decimal?))
                return DbType.Decimal;
            if (t == typeof(char) || t == typeof(char?) || t == typeof(char[]) || t == typeof(char?[]))
                return DbType.String;
            if (t == typeof(byte) || t == typeof(byte?))
                return DbType.Byte;
            if (t == typeof(byte[]) || t == typeof(byte?[]))
                return DbType.Binary;
            if(t == typeof(DateTime) || t == typeof(DateTime?))
                return DbType.DateTime;
            if(t == typeof(bool) || t == typeof(bool?))
                return DbType.Boolean;
            return DbType.Object;
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
            name = "$" + name;
            SQLiteParameter sqlParameter = null;
            var i = 0;
            foreach (var parameter in dbParameters)
            {
                if (parameter.ParameterName.StartsWith(name))
                {
                    if (parameter.Value.Equals(value))
                    {
                        sqlParameter = (SQLiteParameter)parameter;
                        name = parameter.ParameterName;
                        break;
                    }
                    i++;
                }
            }
            if (sqlParameter == null)
            {
                name += i == 0 ? "" : i.ToString(CultureInfo.InvariantCulture);
                sqlParameter = new SQLiteParameter { ParameterName = name };
                if (value.IsString())
                {
                    var v = value.ToString();
                    sqlParameter.DbType = DbType.String;
                    sqlParameter.Size = v.Length == 0 ? 1 : v.Length;
                }
                sqlParameter.Value = value;
                dbParameters.Add(sqlParameter);
            }
            return dbParameters;
        }
    }
}
