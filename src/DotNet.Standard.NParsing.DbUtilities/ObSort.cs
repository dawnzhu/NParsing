/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-03-29 14:10:00
* 版 本 号：1.0.0
* 功能说明：创建排序接口实现(数据库ORDER BY)
* ----------------------------------
 */

using System.Linq;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObSort : ObSortBase
    {
        public ObSort()
        {
            Key = "OrderBy";
        }

        public ObSort(Factory.ObProperty obProperty, bool isAsc)
        {
            Key = "OrderBy_" + obProperty.TableName + "_" + obProperty.ColumnName + "_" + isAsc;
            List.Add(new DbSort(obProperty, isAsc));
        }

        public override IObSort AddOrderBy(Factory.ObProperty obProperty)
        {
            if (List.Any(obj => obj.TableName == obProperty.TableName && obj.ColumnName == obProperty.ColumnName))
                return this;
            Key += "_" + obProperty.TableName + "_" + obProperty.ColumnName + "_True";
            List.Add(new DbSort(obProperty, true));
            return this;
        }

        public override IObSort AddOrderByDescending(Factory.ObProperty obProperty)
        {
            if (List.Any(obj => obj.TableName == obProperty.TableName && obj.ColumnName == obProperty.ColumnName))
                return this;
            Key += "_" + obProperty.TableName + "_" + obProperty.ColumnName + "_False";
            List.Add(new DbSort(obProperty, false));
            return this;
        }

        public override IObSort AddOrderBy(Factory.ObProperty[] obPropertys)
        {
            foreach (var obProperty in obPropertys)
            {
                if (List.Any(obj => obj.TableName == obProperty.TableName && obj.ColumnName == obProperty.ColumnName))
                    continue;
                if (Key.Length > 0)
                    Key += "_";
                Key += obProperty.TableName + "_" + obProperty.ColumnName + "_True";
                List.Add(new DbSort(obProperty, true));
            }
            return this;
        }

        public override IObSort AddOrderByDescending(Factory.ObProperty[] obPropertys)
        {
            foreach (var obProperty in obPropertys)
            {
                if (List.Any(obj => obj.TableName == obProperty.TableName && obj.ColumnName == obProperty.ColumnName))
                    continue;
                if (Key.Length > 0)
                    Key += "_";
                Key += obProperty.TableName + "_" + obProperty.ColumnName + "_False";
                List.Add(new DbSort(obProperty, false));
            }
            return this;
        }

        public override IObSort Add(IObSort obSort)
        {
            if (obSort == null) return this;
            foreach (var dbSort in obSort.List)
            {
                if (List.Any(obj => obj.TableName == dbSort.TableName && obj.ColumnName == dbSort.ColumnName))
                    continue;
                if (Key.Length > 0)
                    Key += "_";
                Key += dbSort.TableName + "_" + dbSort.ColumnName + "_" + dbSort.IsAsc;
                List.Add(dbSort);
            }
            return this;
        }

        public override IObSort Add(IObSort[] obSorts)
        {
            foreach (var dbSort in obSorts.Where(obSort => obSort != null).SelectMany(obSort => obSort.List))
            {
                if (List.Any(obj => obj.TableName == dbSort.TableName && obj.ColumnName == dbSort.ColumnName))
                    continue;
                if (Key.Length > 0)
                    Key += "_";
                Key += dbSort.TableName + "_" + dbSort.ColumnName + "_" + dbSort.IsAsc;
                List.Add(dbSort);
            }
            return this;
        }
    }
}
