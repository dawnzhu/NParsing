/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-03-29 14:10:00
* 版 本 号：1.0.0
* 功能说明：创建排序接口实现(数据库ORDER BY)
* ----------------------------------
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Standard.NParsing.Factory;
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

    public class ObSort<TTerm> : ObSort, IObSort<TTerm>
        where TTerm : ObTermBase
    {
        private readonly TTerm _term;
        public ObSort(TTerm term)
        {
            _term = term;
        }

        public ObSort(TTerm term, ObProperty obProperty, bool isAsc) : base(obProperty, isAsc)
        {
            _term = term;
        }

        public IObSort<TTerm> AddOrderBy(Func<TTerm, ObProperty> keySelector)
        {
            var property = keySelector(_term);
            base.AddOrderBy(property);
            return this;
        }

        public IObSort<TTerm> AddOrderByDescending(Func<TTerm, ObProperty> keySelector)
        {
            var property = keySelector(_term);
            base.AddOrderByDescending(property);
            return this;
        }

        public IObSort<TTerm> AddOrderBy(Func<TTerm, ObProperty[]> keySelector)
        {
            var property = keySelector(_term);
            base.AddOrderBy(property);
            return this;
        }

        public IObSort<TTerm> AddOrderByDescending(Func<TTerm, ObProperty[]> keySelector)
        {
            var property = keySelector(_term);
            base.AddOrderByDescending(property);
            return this;
        }

        public IObSort<TTerm> AddOrderBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            var list = new List<ObProperty>();
            var key = keySelector(_term);
            if (key is ObProperty value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObProperty value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            base.AddOrderBy(list.ToArray());
            return this;
        }

        public IObSort<TTerm> AddOrderByDescending<TKey>(Func<TTerm, TKey> keySelector)
        {
            var list = new List<ObProperty>();
            var key = keySelector(_term);
            if (key is ObProperty value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObProperty value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            base.AddOrderByDescending(list.ToArray());
            return this;
        }
    }
}
