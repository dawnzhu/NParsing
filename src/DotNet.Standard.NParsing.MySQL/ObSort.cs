/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-03-29 14:10:00
* 版 本 号：1.0.0
* 功能说明：创建排序接口实现(数据库ORDER BY)
* ----------------------------------
 */
using System.Collections.Generic;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.MySQL
{
    public class ObSort : ObSortBase
    {
        public ObSort(IList<DbSort> list)
        {
            List = list;
        }

        public override string ToString()
        {
            return ToString('.', new List<string>());
        }

        public override string ToString(char separator)
        {
            return ToString(separator, new List<string>());
        }

        public override string ToString(IList<string> columnNames)
        {
            return ToString('.', columnNames);
        }

        public override string ToString(char separator, IList<string> columnNames)
        {
            string strSqlOrder = string.Empty;
            foreach (DbSort dbSort in List)
            {
                if (strSqlOrder.Length > 0)
                    strSqlOrder += ",";
                string columnName;
                if (string.IsNullOrEmpty(dbSort.TableName))
                {
                    columnName = dbSort.ColumnName;
                }
                else
                {
                    columnName = string.Format("{0}{2}{1}", dbSort.TableName, dbSort.ColumnName, separator);
                    foreach (var name in columnNames)
                    {
                        if (name == columnName)
                        {
                            columnName = dbSort.ColumnName;
                            break;
                        }
                    }
                }
                switch (dbSort.ObProperty.DbFunc)
                {
                    case DbFunc.Null:
                        break;
                    case DbFunc.Avg:
                        columnName = $"AVG({columnName})";
                        break;
                    case DbFunc.Count:
                        columnName = $"COUNT({columnName})";
                        break;
                    case DbFunc.Max:
                        columnName = $"MAX({columnName})";
                        break;
                    case DbFunc.Min:
                        columnName = $"MIN({columnName})";
                        break;
                    case DbFunc.Sum:
                        columnName = $"SUM({columnName})";
                        break;
                    case DbFunc.Replace:
                        columnName += $"REPLACE({columnName})";
                        break;
                    case DbFunc.SubString:
                        columnName += $"SUBSTRING({columnName})";
                        break;
                    case DbFunc.IndexOf:
                        var indCvs = columnName.Split(',');
                        columnName += $"CHARINDEX({indCvs[1]},{indCvs[0]})-1";
                        break;
                    case DbFunc.ToInt16:
                        columnName += $"CONVERT(SMALLINT, {columnName})";
                        break;
                    case DbFunc.ToInt32:
                        columnName += $"CONVERT(INT, {columnName})";
                        break;
                    case DbFunc.ToInt64:
                        columnName += $"CONVERT(BIGINT, {columnName})";
                        break;
                    case DbFunc.ToSingle:
                        columnName += $"CONVERT(FLOAT, {columnName})";
                        break;
                    case DbFunc.ToDouble:
                        columnName += $"CONVERT(DOUBLE, {columnName})";
                        break;
                    case DbFunc.ToDecimal:
                        var cvs = columnName.Split(',');
                        columnName += $"CONVERT(DECIMAL({cvs[1]}, {cvs[2]}), {cvs[0]})";
                        break;
                    case DbFunc.ToDateTime:
                        columnName += $"CONVERT(DATETIME, {columnName})";
                        break;
                    case DbFunc.ToString:
                        columnName += $"CONVERT(VARCHAR, {columnName})";
                        break;
                    case DbFunc.Format:
                        columnName += $"FORMAT({columnName})";
                        break;
                    case DbFunc.IfNull:
                        var nullCvs = columnName.Split(',');
                        columnName += $"IFNULL({nullCvs[0]},{nullCvs[1]})";
                        break;
                }
                strSqlOrder += dbSort.IsAsc ? columnName : columnName + " DESC";
            }
            return strSqlOrder;
            /*string strSqlOrderAsc = string.Empty;
            string strSqlOrderDesc = string.Empty;
            foreach (DbSort dbSort in List)
            {
                if (dbSort.IsAsc)
                {
                    if (strSqlOrderAsc.Length > 0)
                        strSqlOrderAsc += ",";
                    //strSqlOrderAsc += string.Format("{0}.{1}", dbSort.TableName, dbSort.ColumnName);
                    string columnName;
                    if (string.IsNullOrEmpty(dbSort.TableName))
                    {
                        columnName = dbSort.ColumnName;
                    }
                    else
                    {
                        columnName = string.Format("{0}.{1}", dbSort.TableName, dbSort.ColumnName);
                        foreach (var name in columnNames)
                        {
                            if (name == columnName)
                            {
                                columnName = dbSort.ColumnName;
                                break;
                            }
                        }
                    }
                    strSqlOrderAsc += columnName;
                }
                else
                {
                    if (strSqlOrderDesc.Length > 0)
                        strSqlOrderDesc += ",";
                    //strSqlOrderDesc += string.Format("{0}.{1}", dbSort.TableName, dbSort.ColumnName);
                    string columnName;
                    if (string.IsNullOrEmpty(dbSort.TableName))
                    {
                        columnName = dbSort.ColumnName;
                    }
                    else
                    {
                        columnName = string.Format("{0}.{1}", dbSort.TableName, dbSort.ColumnName);
                        foreach (var name in columnNames)
                        {
                            if (name == columnName)
                            {
                                columnName = dbSort.ColumnName;
                                break;
                            }
                        }
                    }
                    strSqlOrderDesc += columnName;
                }
            }
            if (strSqlOrderAsc.Length > 0 && strSqlOrderDesc.Length > 0)
            {
                return strSqlOrderAsc + " ASC," + strSqlOrderDesc + " DESC";
            }
            if (strSqlOrderAsc.Length > 0)
            {
                return strSqlOrderAsc + " ASC";
            }
            if (strSqlOrderDesc.Length > 0)
            {
                return strSqlOrderDesc + " DESC";
            }
            return "";*/
        }
    }
}