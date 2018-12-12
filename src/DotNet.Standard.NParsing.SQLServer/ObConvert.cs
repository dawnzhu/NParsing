using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Standard.NParsing.SQLServer
{
    public static class ObConvert
    {
        public static SqlDbType ToDbType(this Type t)
        {
            if(t == typeof(string))
                return SqlDbType.VarChar;
            if (t == typeof(short) || t == typeof(short?) || t == typeof(ushort) || t == typeof(ushort?))
                return SqlDbType.SmallInt;
            if (t == typeof(int) || t == typeof(int?) || t == typeof(uint) || t == typeof(uint?))
                return SqlDbType.Int;
            if (t == typeof(long) || t == typeof(long?) || t == typeof(ulong) || t == typeof(ulong?))
                return SqlDbType.BigInt;
            if (t == typeof(double) || t == typeof(double?))
                return SqlDbType.Float;
            if (t == typeof(float) || t == typeof(float?))
                return SqlDbType.Real;
            if (t == typeof(decimal) || t == typeof(decimal?))
                return SqlDbType.Decimal;
            if (t == typeof(char) || t == typeof(char?) || t == typeof(char[]) || t == typeof(char?[]))
                return SqlDbType.Char;
            if (t == typeof(byte) || t == typeof(byte?))
                return SqlDbType.TinyInt;
            if (t == typeof(byte[]) || t == typeof(byte?[]))
                return SqlDbType.VarBinary;
            if(t == typeof(DateTime) || t == typeof(DateTime?))
                return SqlDbType.DateTime;
            if(t == typeof(bool) || t == typeof(bool?))
                return SqlDbType.Bit;
            if (t.BaseType == typeof(Enum))
                return SqlDbType.Int;
            return SqlDbType.Variant;
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
                return "FLOAT";
            if (t == typeof(float) || t == typeof(float?))
                return "REAL";
            if (t == typeof(decimal) || t == typeof(decimal?))
                return "DECIMAL({0},{1})";
            if (t == typeof(char) || t == typeof(char?) || t == typeof(char[]) || t == typeof(char?[]))
                return "CHAR({0})";
            if (t == typeof(byte) || t == typeof(byte?))
                return "TINYINT";
            if (t == typeof(byte[]) || t == typeof(byte?[]))
                return "VARBINARY({0})";
            if (t == typeof(DateTime) || t == typeof(DateTime?))
                return "DATETIME";
            if (t == typeof(bool) || t == typeof(bool?))
                return "BIT";
            if (t.BaseType == typeof (Enum))
                return "INT";
            return "VARIANT";
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
            SqlParameter sqlParameter = null;
            var i = 0;
            foreach (var parameter in dbParameters)
            {
                if (parameter.ParameterName.StartsWith(name))
                {
                    if (parameter.Value.Equals(value))
                    {
                        sqlParameter = (SqlParameter)parameter;
                        name = parameter.ParameterName;
                        break;
                    }
                    i++;
                }
            }
            if (sqlParameter == null)
            {
                name += i == 0 ? "" : i.ToString(CultureInfo.InvariantCulture);
                sqlParameter = new SqlParameter { ParameterName = name };
                if (value.IsString())
                {
                    var v = value.ToString();
                    sqlParameter.SqlDbType = v.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                    sqlParameter.Size = v.Length == 0 ? 1 : v.Length;
                }
                sqlParameter.Value = value;
                dbParameters.Add(sqlParameter);
            }
            return dbParameters;
        }
    }
}
