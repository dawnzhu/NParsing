/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2018-12-22 14.27:00
* 版 本 号：1.0.0
* 功能说明：创建排序接口实现(数据库ORDER BY)
* ----------------------------------
 */
using System.Collections.Generic;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.SQLite
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