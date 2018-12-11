namespace DotNet.Standard.NParsing.Utilities
{
    public class DbGroup
    {
        public DbGroup()
        {
        }

        public DbGroup(string tableName, string columnName, string propertyName)
        {
            TableName = tableName;
            ColumnName = columnName;
            PropertyName = propertyName;
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
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }
    }
}