/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 23:27:00
* 版 本 号：1.0.0
* 功能说明：创建分组接口实现(数据库GROUP BY)
* ----------------------------------
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObGroup : ObGroupBase
    {
        public ObGroup()
        {
            _Key = "GroupBy";
        }

        public ObGroup(/*string tableName, string columnName, string propertyName*/Factory.ObProperty obProperty)
        {
            //_Key = "GroupBy_" + obProperty.TableName + "_" + obProperty.ColumnName;
            _Key = "GroupBy_" + obProperty.Key;
            DbGroups.Add(new DbGroup(obProperty.TableName, obProperty.ColumnName, obProperty.PropertyName));
            //ObProperties.Add(new ObProperty(modelType, columnName));
            ObGroupProperties.Add(obProperty);
        }

        public override IObGroup AddGroupBy(Factory.ObProperty obProperty)
        {
            //_Key += "_" + obProperty.TableName + "_" + obProperty.ColumnName;
            _Key += "_" + obProperty.Key;
            DbGroups.Add(new DbGroup(obProperty.TableName, obProperty.ColumnName, obProperty.PropertyName));
            ObGroupProperties.Add(obProperty);
            return this;
        }

        public override IObGroup AddGroupBy(params Factory.ObProperty[] obPropertys)
        {
            foreach (var obProperty in obPropertys)
            {
                if (_Key.Length > 0)
                    _Key += "_";
                //_Key += obProperty.TableName + "_" + obProperty.ColumnName;
                _Key += obProperty.Key;
                DbGroups.Add(new DbGroup(obProperty.TableName, obProperty.ColumnName, obProperty.PropertyName));
                ObGroupProperties.Add(obProperty);
            }
            return this;
        }
    }

    public class ObGroup<TTerm> : ObGroup, IObGroup<TTerm>
        where TTerm : ObTermBase
    {
        private readonly TTerm _term;
        public ObGroup(TTerm term)
        {
            _term = term;
        }

        public ObGroup(TTerm term, ObProperty obProperty) : base(obProperty)
        {
            _term = term;
        }

        public IObGroup<TTerm> AddGroupBy(Func<TTerm, ObProperty> keySelector)
        {
            var property = keySelector(_term);
            base.AddGroupBy(property);
            return this;
        }

        public IObGroup<TTerm> AddGroupBy(Func<TTerm, ObProperty[]> keySelector)
        {
            var property = keySelector(_term);
            base.AddGroupBy(property);
            return this;
        }

        public IObGroup<TTerm> AddGroupBy<TKey>(Func<TTerm, TKey> keySelector)
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
            base.AddGroupBy(list.ToArray());
            return this;
        }
    }
}