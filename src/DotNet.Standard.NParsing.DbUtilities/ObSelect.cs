using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObSelect<TModel, TTerm> : ObQuery<TModel>, IObSelect<TModel, TTerm>
        where TModel : ObModelBase, new()
        where TTerm : ObTermBase
    {
        public TTerm Term { get; }
        /*public IObParameter ObParameter { get; private set; }
        public IObSort ObSort { get; private set; }
        public IObGroup ObGroup { get; private set; }
        public IObJoin ObJoin { get; private set; }*/

        public ObSelect(IDbHelper iDbHelper, ISqlBuilder<TModel> iSqlBuilder, string providerName, TTerm term, IObTransaction iObTransaction) : base(iDbHelper, iSqlBuilder, providerName, iObTransaction)
        {
            Term = term;
        }

        /*public ObSelect(TTerm term)
        {
            Term = term;
        }*/

        public IObSelect<TModel, TTerm> Where(Func<TTerm, IObParameter> keySelector)
        {
            if (ObGroup == null)
            {
                ObParameter = keySelector(Term);
            }
            else
            {
                ObGroupParameter = keySelector(Term);
            }
            return this;
        }

        public IObSelect<TModel, TTerm> GroupBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            var list = new List<ObProperty>();
            var key = keySelector(Term);
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
            ObGroup.AddGroupBy(list.ToArray());
            return this;
        }

        public IObSelect<TModel, TTerm> GroupBy(Func<TTerm, ObProperty> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.AddGroupBy(keySelector(Term));
            return this;
        }

        public IObSelect<TModel, TTerm> DistinctBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            return GroupBy(keySelector);
        }

        public IObSelect<TModel, TTerm> DistinctBy(Func<TTerm, ObProperty> keySelector)
        {
            return GroupBy(keySelector);
        }

        public IObSelect<TModel, TTerm> Select<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            var key = keySelector(Term);
            if (key is IObProperty value)
            {
                ObGroup.ObProperties.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is IObProperty value2)
                    {
                        if (value2.AsProperty.PropertyName != propertyInfo.Name)
                        {
                            var asProperty = typeof(TTerm).GetProperty(propertyInfo.Name);
                            if (asProperty != null)
                            {
                                value2.AsProperty = (IObProperty)asProperty.GetValue(Term);
                            }
                        }
                        ObGroup.ObProperties.Add(value2);
                    }
                }
            }
            return this;
        }

        public IObSelect<TModel, TTerm> Select(Func<TTerm, IObProperty> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.ObProperties.Add(keySelector(Term));
            return this;
        }

        public IObSelect<TModel, TTerm> OrderBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            var list = new List<ObProperty>();
            var key = keySelector(Term);
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
            ObSort.AddOrderBy(list.ToArray());
            return this;
        }

        public IObSelect<TModel, TTerm> OrderBy(Func<TTerm, ObProperty> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.AddOrderBy(keySelector(Term));
            return this;
        }

        public IObSelect<TModel, TTerm> OrderByDescending<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            var list = new List<ObProperty>();
            var key = keySelector(Term);
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
            ObSort.AddOrderByDescending(list.ToArray());
            return this;
        }

        public IObSelect<TModel, TTerm> OrderByDescending(Func<TTerm, ObProperty> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.AddOrderByDescending(keySelector(Term));
            return this;
        }

        public IObSelect<TModel, TTerm> Join<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            var list = new List<ObTermBase>();
            var key = keySelector(Term);
            if (key is ObTermBase value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObTermBase value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            ObJoin.AddJoin(list.ToArray());
            return this;
        }

        public IObSelect<TModel, TTerm> Join(Func<TTerm, ObTermBase> keySelector)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            ObJoin.AddJoin(keySelector(Term));
            return this;
        }
    }
}
