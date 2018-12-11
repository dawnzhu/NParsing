using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Utilities
{
    public class DbNTerm
    {
        public DbNTerm()
        {
            
        }

        public DbNTerm(string tableName, string columnName, DbValue dbValue)
        {
            TableName = tableName;
            ColumnName = columnName;
            DbValue = dbValue;
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
        /// 条件值 IS NULL, IS NOT NULL
        /// </summary>
        public DbValue DbValue { get; set; }
    }

    public class DbNTerm2
    {
        public DbNTerm2()
        {
        }

        public DbNTerm2(IObProperty value, DbValue dbValue)
        {
            Value = value;
            DbValue = dbValue;
        }

        /// <summary>
        /// 值 与条件符号配套
        /// </summary>
        public IObProperty Value { get; set; }

        /// <summary>
        /// 条件值 IS NULL, IS NOT NULL
        /// </summary>
        public DbValue DbValue { get; set; }
    }
}