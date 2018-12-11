using DotNet.Standard.NParsing.Factory;

namespace DotNet.Standard.NParsing.Utilities
{
    public class DbSort
    {
        public DbSort()
        { }

        public DbSort(ObProperty obProperty, bool isAsc)
        {
            ObProperty = obProperty;
            TableName = obProperty.TableName;
            ColumnName = obProperty.ColumnName;
            IsAsc = isAsc;
        }

        public ObProperty ObProperty { get; private set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 顺序
        /// </summary>
        public bool IsAsc { get; set; }
    }
}