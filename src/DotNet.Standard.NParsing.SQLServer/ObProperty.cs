/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 23:30:53
* 版 本 号：1.0.0
* 功能说明：数据库函数显示字段成生实现类
* ----------------------------------
 */
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.Utilities;

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
                        /*var psAttribute = (ObSettledAttribute)propertyInfo.GetCustomAttributes(typeof(ObSettledAttribute), true).FirstOrDefault();
                        if (psAttribute != null)
                        {
                            /*obSettledValue = psAttribute.Value;#1#
                            if (propertyInfo.PropertyType.IsEnum())
                            {
                                obSettledValue = Convert.ToDecimal(psAttribute.Value).ToString(CultureInfo.InvariantCulture);
                            }
                            else if (psAttribute.Value is string ||
                                     psAttribute.Value is char ||
                                     psAttribute.Value is bool ||
                                     psAttribute.Value is DateTime)
                            {
                                obSettledValue = string.Format("'{0}'", psAttribute.Value);
                            }
                            else
                            {
                                obSettledValue = psAttribute.Value.ToString();
                            }
                        }*/
                        break;
                    }
                }
                //var tableName = ModelType.ToTableName();
                var columnValue = obSettledValue ?? string.Format("{0}{2}{1}", iObProperty.TableName, iObProperty.ColumnName, separator);
                brothers = iObProperty.Brothers;
                brotherIndex = iObProperty.FuncBrotherCount;
                columnValue = CreateSql(columnValue, brothers, 0, brotherIndex, ref dbParameter);
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
                        var custom = "";
                        foreach (var customParam in iObProperty.CustomParams)
                        {
                            if(custom.Length > 0)
                                custom += ",";
                            custom += CreateSql(customParam, ref dbParameter);
                        }
                        columnNames += $"{iObProperty.FuncName}({custom})";
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
                //return parameterName;
                columnNames += parameterName;
                brothers = new List<object>();
                isAll = false;
            }
            var iBrotherCount = brothers.Count;
            isAll = isAll && iBrotherCount > brotherIndex;
            columnNames = CreateSql(columnNames, brothers, brotherIndex, iBrotherCount, ref dbParameter);
            /*for (int i = brotherIndex; i < iBrotherCount; i++)
            {
                var brother = brothers[i];
                DbAriSymbol ariSymbol;
                int brothersCount;
                if (brother is IObProperty)
                {
                    ariSymbol = ((IObProperty) brother).AriSymbol;
                    brothersCount = ((IObProperty)brother).Brothers.Count;
                }
                else
                {
                    ariSymbol = ((IObValue)brother).AriSymbol;
                    brothersCount = ((IObValue)brother).Brothers.Count;
                }
                switch (ariSymbol)
                {
                    case DbAriSymbol.Plus:
                        columnNames += "+";
                        break;
                    case DbAriSymbol.Minus:
                        columnNames += "-";
                        break;
                    case DbAriSymbol.Multiply:
                        columnNames += "*";
                        break;
                    case DbAriSymbol.Except:
                        columnNames += "/";
                        break;
                    case DbAriSymbol.Mod:
                        columnNames += "%";
                        break;
                    case DbAriSymbol.And:
                        columnNames += "&";
                        break;
                    case DbAriSymbol.Or:
                        columnNames += "|";
                        break;
                }
                string andorWhere = "{0}";
                if (brothersCount > 0)
                {
                    andorWhere = "(" + andorWhere + ")";
                }
                columnNames += string.Format(andorWhere, CreateSql(brother, ref dbParameter));
            }*/
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