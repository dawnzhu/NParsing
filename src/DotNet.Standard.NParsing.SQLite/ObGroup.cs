/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2018-12-22 14.27:00
* 版 本 号：1.0.0
* 功能说明：GROUP BY和数据库函数显示字段成生实现类
* ----------------------------------
 */
using System.Collections.Generic;
using System.Data.Common;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.SQLite
{
    public class ObGroup : ObGroupBase
    {
        public ObGroup(IList<DbGroup> dbGroups, IList<IObProperty> obProperties)
        {
            DbGroups = dbGroups;
            ObProperties = obProperties;
        }

        public override string ToString(ref IList<DbParameter> dbParameter, out string columns, out IList<string> columnNames)
        {
            string sqlGroup = string.Empty;
            columns = string.Empty;
            columnNames = new List<string>();
            foreach (var dbGroup in DbGroups)
            {
                if (sqlGroup.Length != 0)
                    sqlGroup += ",";
                if (columns.Length != 0)
                    columns += ",";
                sqlGroup += $"{dbGroup.TableName}.{dbGroup.ColumnName}";
                columns += $"{dbGroup.TableName}.{dbGroup.ColumnName} AS {dbGroup.TableName}_{dbGroup.PropertyName}";
                columnNames.Add($"{dbGroup.TableName}_{dbGroup.PropertyName}");
            }
            foreach (var obProperty in ObProperties)
            {
                if (columns.Length != 0)
                    columns += ",";
                var p = new ObProperty(obProperty);
                sqlGroup += $"{p.TableName}.{obProperty.ColumnName}";
                columns += p.ToString();
                columnNames.Add($"{p.TableName}_{obProperty.PropertyName}");
            }
            return sqlGroup;
        }
    }
}
