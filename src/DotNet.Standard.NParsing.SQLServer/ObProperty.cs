/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 23:30:53
* 版 本 号：1.0.0
* 功能说明：数据库函数显示字段成生实现类
* ----------------------------------
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Standard.NParsing.SQLServer
{
    public class ObProperty : ObPropertyBase
    {
        /*public ObProperty(Type modelType, string tableName, string columnName, IList<object> brothers, DbAriSymbol ariSymbol, DbFunc dbFunc)
        {
            Brothers = brothers;
            AriSymbol = ariSymbol;
            ModelType = modelType;
            TableName = tableName;
            ColumnName = columnName;
            DbFunc = dbFunc;
        }*/

        public ObProperty(IObProperty iObProperty)
        {
            Brothers = iObProperty.Brothers;
            AriSymbol = iObProperty.AriSymbol;
            ModelType = iObProperty.ModelType;
            TableName = iObProperty.TableName;
            ColumnName = iObProperty.ColumnName;
            PropertyName = iObProperty.PropertyName;
            CustomParams = iObProperty.CustomParams;
            FuncName = iObProperty.FuncName;
            DbFunc = iObProperty.DbFunc;
            FuncBrotherCount = iObProperty.FuncBrotherCount;
            if(iObProperty.Sort != null)
                Sort = new ObSort(iObProperty.Sort.List);
            if (iObProperty.Group != null)
                Group = new ObGroup(iObProperty.Group.DbGroups, Group.ObGroupProperties, Group.ObProperties);
            AsProperty = iObProperty.AsProperty;
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public override string ToString(bool renaming)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            return CreateSql(this, renaming, '.', ref dbParameters);
        }

        public override string ToString(char separator)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            return CreateSql(this, true, separator, ref dbParameters);
        }

        public override string ToString(ref IList<DbParameter> dbParameters)
        {
            return ToString(false, ref dbParameters);
        }

        public override string ToString(bool renaming, ref IList<DbParameter> dbParameters)
        {
            return CreateSql(this, renaming, '.', ref dbParameters);
        }

        public override string ToString(char separator, ref IList<DbParameter> dbParameters)
        {
            return CreateSql(this, false, separator, ref dbParameters);
        }

        private string CreateSql(object value)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            return CreateSql(value, false, '.', ref dbParameters);
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
                    if (propertyInfo.ToColumnName() == iObProperty.ColumnName)
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
                switch (iObProperty.DbFunc)
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
                    case DbFunc.RowNumber:
                        var g = "";
                        if (iObProperty.Group != null)
                        {
                            g = $"PARTITION BY {iObProperty.Group.ToString(ref dbParameter, out _, out _)} ";
                        }
                        columnNames += $"ROW_NUMBER() OVER({g}ORDER BY {iObProperty.Sort.ToString()})";
                        break;
                    case DbFunc.Custom:
                        columnNames += $"{iObProperty.FuncName}({columnValue})";
                        break;
                    case DbFunc.Replace:
                        columnNames += $"REPLACE({columnValue})";
                        break;
                    case DbFunc.SubString:
                        columnNames += $"SUBSTRING({columnValue})";
                        break;
                    case DbFunc.IndexOf:
                        var indCvs = columnValue.Split(',');
                        columnNames += $"CHARINDEX({indCvs[1]},{indCvs[0]})-1";
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
                        columnNames += $"CONVERT(FLOAT(24), {columnValue})";
                        break;
                    case DbFunc.ToDouble:
                        columnNames += $"CONVERT(FLOAT(53), {columnValue})";
                        break;
                    case DbFunc.ToDecimal:
                        var decCvs = columnValue.Split(',');
                        columnNames += $"CONVERT(DECIMAL({decCvs[1]}, {decCvs[2]}), {decCvs[0]})";
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
                SqlParameter sqlParameter = null;

                #region 防止重复参数名

                var i = 0;
                foreach (var parameter in dbParameter)
                {
                    if (parameter.ParameterName.StartsWith(parameterName))
                    {
                        if (parameter.Value.Equals(iObValue.Value))
                        {
                            sqlParameter = (SqlParameter)parameter;
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
                    sqlParameter = new SqlParameter {ParameterName = parameterName};
                    if (iObValue.Value.IsString())
                    {
                        var vv = iObValue.Value.ToString();
                        sqlParameter.SqlDbType = vv.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
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
                    parameterName = "FLOAT(24)";
                }
                else if (type == typeof(double))
                {
                    parameterName = "FLOAT(53)";
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
                SqlParameter sqlParameter = null;

                #region 防止重复参数名

                var i = 0;
                foreach (var parameter in dbParameter)
                {
                    if (parameter.ParameterName.StartsWith(parameterName))
                    {
                        if (parameter.Value.Equals(value))
                        {
                            sqlParameter = (SqlParameter)parameter;
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
                    sqlParameter = new SqlParameter { ParameterName = parameterName };
                    if (value.IsString())
                    {
                        var vv = value.ToString();
                        sqlParameter.SqlDbType = vv.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                        sqlParameter.Size = vv.Length == 0 ? 1 : vv.Length;
                    }
                    sqlParameter.Value = value;
                    dbParameter.Add(sqlParameter);
                }
                columnNames += parameterName;
                brothers = new List<object>();
                isAll = false;
            }
            var iBrotherCount = brothers.Count;
            isAll = isAll && iBrotherCount > brotherIndex;
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
                    ariSymbol = ((IObValue) brother).AriSymbol;
                    brothersCount = ((IObValue) brother).Brothers.Count;
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