/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 23:30:53
* 版 本 号：1.0.0
* 功能说明：数据库函数显示字段成生实现类
* ----------------------------------
 */

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using DotNet.Standard.Common.Utilities;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;
using MySql.Data.MySqlClient;

namespace DotNet.Standard.NParsing.MySQL
{
    public class ObProperty : ObPropertyBase
    {
        public ObProperty(IObProperty iObProperty)
        {
            Brothers = iObProperty.Brothers;
            AriSymbol = iObProperty.AriSymbol;
            ModelType = iObProperty.ModelType;
            TableName = iObProperty.TableName;
            ColumnName = iObProperty.ColumnName;
            PropertyName = iObProperty.PropertyName;
            DbFunc = iObProperty.DbFunc;
        }

        public override string ToString()
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            return CreateSql(this, true, '.', ref dbParameters);
        }

        public override string ToString(char separator)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            return CreateSql(this, true, separator, ref dbParameters);
        }

        public override string ToString(ref IList<DbParameter> dbParameters)
        {
            return CreateSql(this, false, '.', ref dbParameters);
        }

        public override string ToString(char separator, ref IList<DbParameter> dbParameters)
        {
            return CreateSql(this, false, separator, ref dbParameters);
        }

        private string CreateSql(object value, ref IList<DbParameter> dbParameter)
        {
            return CreateSql(value, false, '.', ref dbParameter);
        }

        private string CreateSql(object value, bool renaming, char separator, ref IList<DbParameter> dbParameter)
        {
            IList<object> brothers;
            var brotherIndex = 0;
            var columnNames = string.Empty;
            bool isAll;
            var asString = "";
            if (value is IObProperty iObProperty)
            {
                string obSettledValue = null;
                foreach (var propertyInfo in iObProperty.ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (propertyInfo.ToColumnName() == ColumnName)
                    {
                        obSettledValue = propertyInfo.GetSettledValue();
                        break;
                    }
                }
                var columnValue = obSettledValue ?? string.Format("{0}{2}{1}", iObProperty.TableName, iObProperty.ColumnName, separator);
                brothers = iObProperty.Brothers;
                brotherIndex = iObProperty.FuncBrotherCount;
                if (iObProperty.CustomParams == null)
                {
                    columnValue = CreateSql(columnValue, brothers, 0, brotherIndex, ref dbParameter);
                }
                else
                {
                    columnValue = "";
                    foreach (var customParam in iObProperty.CustomParams)
                    {
                        if (columnValue.Length > 0)
                            columnValue += ",";
                        columnValue += CreateSql(customParam, ref dbParameter);
                    }
                }
                switch (DbFunc)
                {
                    case DbFunc.Null:
                        columnNames += $"{columnValue}";
                        break;
                    case DbFunc.Avg:
                        columnNames += $"AVG({columnValue})";
                        break;
                    case DbFunc.Count:
                        columnNames += $"COUNT({columnValue})";
                        break;
                    case DbFunc.Max:
                        columnNames += $"MAX({columnValue})";
                        break;
                    case DbFunc.Min:
                        columnNames += $"MIN({columnValue})";
                        break;
                    case DbFunc.Sum:
                        columnNames += $"SUM({columnValue})";
                        break;
                    case DbFunc.Replace:
                        columnNames += $"REPLACE({columnValue})";
                        break;
                    case DbFunc.SubString:
                        columnNames += $"SUBSTRING({columnValue})";
                        break;
                    case DbFunc.IndexOf:
                        var indCvs = columnValue.Split(',');
                        columnNames += $"LOCATE({indCvs[1]},{indCvs[0]})-1";
                        break;
                    case DbFunc.ToInt16:
                        columnNames += $"CONVERT(SMALLINT, {columnValue})";
                        break;
                    case DbFunc.ToInt32:
                        columnNames += $"CONVERT(INT, {columnValue})";
                        break;
                    case DbFunc.ToInt64:
                        columnNames += $"CONVERT(BIGINT, {columnValue})";
                        break;
                    case DbFunc.ToSingle:
                        columnNames += $"CONVERT(FLOAT, {columnValue})";
                        break;
                    case DbFunc.ToDouble:
                        columnNames += $"CONVERT(DOUBLE, {columnValue})";
                        break;
                    case DbFunc.ToDecimal:
                        var cvs = columnValue.Split(',');
                        columnNames += $"CONVERT(DECIMAL({cvs[1]}, {cvs[2]}), {cvs[0]})";
                        break;
                    case DbFunc.ToDateTime:
                        columnNames += $"CONVERT(DATETIME, {columnValue})";
                        break;
                    case DbFunc.ToString:
                        columnNames += $"CONVERT(VARCHAR, {columnValue})";
                        break;
                    case DbFunc.Format:
                        columnNames += $"FORMAT({columnValue})";
                        break;
                }
                asString = renaming ? $" AS {iObProperty.AsProperty.TableName}_{iObProperty.AsProperty.PropertyName}" : "";
                isAll = iObProperty.AriSymbol == DbAriSymbol.Null;
            }
            else if (value is IObValue iObValue)
            {
                var parameterName = "@NPaValue";
                MySqlParameter sqlParameter = null;

                #region 防止重复参数名

                var i = 0;
                foreach (var parameter in dbParameter)
                {
                    if (parameter.ParameterName.StartsWith(parameterName))
                    {
                        if (parameter.Value.Equals(iObValue.Value))
                        {
                            sqlParameter = (MySqlParameter)parameter;
                            parameterName = parameter.ParameterName;
                            break;
                        }
                        i++;
                    }
                }

                #endregion

                if (sqlParameter == null)
                {
                    parameterName += i == 0 ? "" : i.ToString();
                    sqlParameter = new MySqlParameter { ParameterName = parameterName };
                    if (iObValue.Value.IsString())
                    {
                        var vv = iObValue.Value.ToString();
                        sqlParameter.MySqlDbType = MySqlDbType.VarChar;
                        sqlParameter.Size = vv.Length == 0 ? 1 : vv.Length;
                    }
                    sqlParameter.Value = iObValue.Value;
                    dbParameter.Add(sqlParameter);
                }
                columnNames += parameterName;
                brothers = iObValue.Brothers;
                isAll = iObValue.AriSymbol == DbAriSymbol.Null;
            }
            else if (value is Type type)
            {
                var parameterName = "";
                if (type == typeof(short))
                {
                    parameterName = "SMALLINT";
                }
                else if (type == typeof(int))
                {
                    parameterName = "INT";
                }
                else if (type == typeof(long))
                {
                    parameterName = "BIGINT";
                }
                else if (type == typeof(float))
                {
                    parameterName = "FLOAT";
                }
                else if (type == typeof(double))
                {
                    parameterName = "DOUBLE";
                }
                else if (type == typeof(decimal))
                {
                    parameterName = "DECIMAL(38,8)";
                }
                else if (type == typeof(DateTime))
                {
                    parameterName = "DATETIME";
                }
                else if (type == typeof(string))
                {
                    parameterName = "VARCHAR";
                }
                columnNames += parameterName;
                brothers = new List<object>();
                isAll = false;
            }
            else
            {
                var parameterName = "@NPaValue";
                MySqlParameter sqlParameter = null;

                #region 防止重复参数名

                var i = 0;
                foreach (var parameter in dbParameter)
                {
                    if (parameter.ParameterName.StartsWith(parameterName))
                    {
                        if (parameter.Value.Equals(value))
                        {
                            sqlParameter = (MySqlParameter)parameter;
                            parameterName = parameter.ParameterName;
                            break;
                        }
                        i++;
                    }
                }

                #endregion

                if (sqlParameter == null)
                {
                    parameterName += i == 0 ? "" : i.ToString();
                    sqlParameter = new MySqlParameter { ParameterName = parameterName };
                    if (value.IsString())
                    {
                        var vv = value.ToString();
                        sqlParameter.MySqlDbType = MySqlDbType.VarChar;
                        sqlParameter.Size = vv.Length == 0 ? 1 : vv.Length;
                    }
                    sqlParameter.Value = value;
                    dbParameter.Add(sqlParameter);
                }
                columnNames += parameterName;
                brothers = new List<object>();
                isAll = false;
            }
            int iBrotherCount = brothers.Count;
            isAll = isAll && iBrotherCount > 0;
            columnNames = CreateSql(columnNames, brothers, brotherIndex, iBrotherCount, ref dbParameter);
            if (isAll)
                return "(" + columnNames + ")" + asString;
            return columnNames + asString;
        }

        /// <summary>
        /// 算术运算
        /// </summary>
        /// <param name="columnValue"></param>
        /// <param name="brothers"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        private string CreateSql(string columnValue, IList<object> brothers, int start, int end, ref IList<DbParameter> dbParameter)
        {
            for (var i = start; i < end; i++)
            {
                var brother = brothers[i];
                DbAriSymbol ariSymbol;
                int brothersCount;
                if (brother is IObProperty property)
                {
                    ariSymbol = property.AriSymbol;
                    brothersCount = property.Brothers.Count;
                }
                else
                {
                    ariSymbol = ((IObValue)brother).AriSymbol;
                    brothersCount = ((IObValue)brother).Brothers.Count;
                }
                switch (ariSymbol)
                {
                    case DbAriSymbol.Plus:
                        columnValue += "+";
                        break;
                    case DbAriSymbol.Minus:
                        columnValue += "-";
                        break;
                    case DbAriSymbol.Multiply:
                        columnValue += "*";
                        break;
                    case DbAriSymbol.Except:
                        columnValue += "/";
                        break;
                    case DbAriSymbol.Mod:
                        columnValue += "%";
                        break;
                    case DbAriSymbol.And:
                        columnValue += "&";
                        break;
                    case DbAriSymbol.Or:
                        columnValue += "|";
                        break;
                }
                var andorWhere = "{0}";
                if (brothersCount > 0)
                {
                    andorWhere = "(" + andorWhere + ")";
                }
                columnValue += string.Format(andorWhere, CreateSql(brother, ref dbParameter));
            }
            return columnValue;
        }
    }
}