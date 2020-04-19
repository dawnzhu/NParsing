/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2018-12-22 14.27:00
* 版 本 号：1.0.0
* 功能说明：数据库函数显示字段成生实现类
* ----------------------------------
 */
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Reflection;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.SQLite
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

        private string CreateSql(object value, bool renaming, char separator, ref IList<DbParameter> dbParameter)
        {
            IList<object> brothers;
            var columnNames = string.Empty;
            bool isAll;
            if (value is IObProperty iObProperty)
            {
                string obSettledValue = null;
                foreach (var propertyInfo in iObProperty.ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (propertyInfo.ToColumnName() == ColumnName)
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
                        columnNames += "-1";
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
                columnNames += renaming ? $" AS {TableName}_{PropertyName}" : "";
                brothers = iObProperty.Brothers;
                isAll = iObProperty.AriSymbol == DbAriSymbol.Null;
            }
            else
            {
                var iObValue = (IObValue)value;
                var parameterName = "$NPaValue";

                #region 防止重复参数名

                int i = 0;
                foreach (var parameter in dbParameter)
                {
                    var pn = parameter.ParameterName;
                    if (pn.Length > parameterName.Length && pn.Substring(0, parameterName.Length).Equals(parameterName))
                        i++;
                    else if (pn.Length == parameterName.Length && pn.Equals(parameterName))
                        i++;
                }
                parameterName += i == 0 ? "" : i.ToString();

                #endregion

                dbParameter.Add(new SQLiteParameter(parameterName, iObValue.Value));
                columnNames += parameterName;
                brothers = iObValue.Brothers;
                isAll = iObValue.AriSymbol == DbAriSymbol.Null;
            }
            int iBrotherCount = brothers.Count;
            isAll = isAll && iBrotherCount > 0;
            for (int i = 0; i < iBrotherCount; i++)
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
                columnNames += string.Format(andorWhere, CreateSql(brother, renaming, separator, ref dbParameter));
            }
            if (isAll)
                return "(" + columnNames + ")";
            return columnNames;
        }
    }
}