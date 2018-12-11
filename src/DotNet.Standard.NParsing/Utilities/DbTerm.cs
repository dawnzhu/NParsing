using System;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Utilities
{
    public class DbTerm
    {
        public DbTerm()
        {
        }

        public DbTerm(string tableName, string columnName, DbSymbol dbSymbol, object value)
        {
            TableName = tableName;
            ColumnName = columnName;
            DbSymbol = dbSymbol;
            Value = value;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 条件符号
        /// </summary>
        public DbSymbol DbSymbol { get; set; }

        /// <summary>
        /// 值 与条件符号配套
        /// </summary>
        public object Value { get; set; }
    }

    public class DbTerm2
    {
        public DbTerm2()
        {
        }

        public DbTerm2(string tableName, string columnName, DbSymbol dbSymbol, string tableName2, string columnName2)
        {
            TableName = tableName;
            TableName2 = tableName2;
            ColumnName = columnName;
            DbSymbol = dbSymbol;
            ColumnName2 = columnName2;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 条件符号
        /// </summary>
        public DbSymbol DbSymbol { get; set; }

        /// <summary>
        /// 表名2
        /// </summary>
        public string TableName2 { get; set; }

        /// <summary>
        /// 字段名2
        /// </summary>
        public string ColumnName2 { get; set; }
    }

    public class DbTerm3
    {
        public DbTerm3()
        {
        }

        public DbTerm3(IObProperty srcValue, DbSymbol dbSymbol, object dstValue)
        {
            SrcValue = srcValue;
            DbSymbol = dbSymbol;
            DstValue = dstValue;
        }

        /// <summary>
        /// 值 与条件符号配套
        /// </summary>
        public IObProperty SrcValue { get; set; }

        /// <summary>
        /// 条件符号
        /// </summary>
        public DbSymbol DbSymbol { get; set; }

        /// <summary>
        /// 值 与条件符号配套
        /// </summary>
        public object DstValue { get; set; }
    }
}