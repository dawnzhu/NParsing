/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 23:30:53
* 版 本 号：1.0.0
* 功能说明：GROUP BY和数据库函数显示字段成生实现类
* ----------------------------------
 */
using System.Collections.Generic;
using System.Data.Common;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.SQLServer
{
    public class ObGroup : ObGroupBase
    {
        public ObGroup(IList<DbGroup> dbGroups, IList<IObProperty> obGroupProperties, IList<IObProperty> obProperties)
        {
            DbGroups = dbGroups;
            ObGroupProperties = obGroupProperties;
            ObProperties = obProperties;
        }

        public override string ToString(ref IList<DbParameter> dbParameter, out string columns, out IList<string> columnNames)
        {
            string sqlGroup = string.Empty;
            columns = string.Empty;
            columnNames = new List<string>();
            /*foreach (var dbGroup in DbGroups)
            {
                if (sqlGroup.Length != 0)
                    sqlGroup += ",";
                if (columns.Length != 0)
                    columns += ",";
                sqlGroup += string.Format("{0}.{1}", dbGroup.TableName, dbGroup.ColumnName);
                columns += string.Format("{0}.{1} AS {0}_{2}", dbGroup.TableName, dbGroup.ColumnName, dbGroup.PropertyName);
                columnNames.Add(string.Format("{0}_{1}", dbGroup.TableName, dbGroup.PropertyName));
            }*/
            foreach (var obProperty in ObGroupProperties)
            {
                if (sqlGroup.Length != 0)
                    sqlGroup += ",";
                if (columns.Length != 0)
                    columns += ",";
                var p = new ObProperty(obProperty);
                var gname = p.ToString(ref dbParameter);
                var asname = $"{p.AsProperty.TableName}_{p.AsProperty.PropertyName}";
                sqlGroup += gname;
                columns += $"{gname} AS {asname}";
                columnNames.Add(asname);
            }
            foreach (var obProperty in ObProperties)
            {
                //if (sqlGroup.Length != 0)
                //    sqlGroup += ",";
                if (columns.Length != 0)
                    columns += ",";
                //sqlGroup += string.Format("{0}.{1}", obProperty.ModelType.ToTableName(), obProperty.ColumnName);
                var p = new ObProperty(obProperty);
                columns += p.ToString(true, ref dbParameter);
                columnNames.Add($"{p.AsProperty.TableName}_{p.AsProperty.PropertyName}");
            }
            return sqlGroup;
        }
    }
}
