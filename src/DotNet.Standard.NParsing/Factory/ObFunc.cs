/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 16:35:12
* 版 本 号：1.0.0
* 功能说明：实现创建显示属性+函数的工厂类
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2011-12-02 17:51:00
* 版 本 号：2.3.0
* 修改内容：代码整理
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObFunc
    {
/*        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObProperty";*/

        #region 基础方法

        public static ObProperty Avg(ObProperty obProperty)
        {
            obProperty.DbFunc = DbFunc.Avg;
            obProperty.FuncBrotherCount = obProperty.Brothers.Count;
            return obProperty;
        }

        public static ObProperty Count(ObProperty obProperty)
        {
            obProperty.DbFunc = DbFunc.Count;
            obProperty.FuncBrotherCount = obProperty.Brothers.Count;
            return obProperty;
        }

        public static ObProperty Max(ObProperty obProperty)
        {
            obProperty.DbFunc = DbFunc.Max;
            obProperty.FuncBrotherCount = obProperty.Brothers.Count;
            return obProperty;
        }

        public static ObProperty Min(ObProperty obProperty)
        {
            obProperty.DbFunc = DbFunc.Min;
            obProperty.FuncBrotherCount = obProperty.Brothers.Count;
            return obProperty;
        }

        public static ObProperty Sum(ObProperty obProperty)
        {
            obProperty.DbFunc = DbFunc.Sum;
            obProperty.FuncBrotherCount = obProperty.Brothers.Count;
            return obProperty;
        }

        public static ObProperty Custom(string func, params object[] parameters)
        {
            if (!(parameters[0] is IObProperty))
                throw new Exception("自定义函数首个参数必须为IObProperty");
            var obProperty = new ObProperty((IObProperty)parameters[0])
            {
                DbFunc = DbFunc.Custom,
                FuncName = func,
                CustomParams = parameters
            };
            return obProperty;
        }

        #endregion

        #region 扩展方法

        public static ObProperty<TSource> Top<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase, new()
        {
            var iObProperty = keySelector(source);
            iObProperty.DbFunc = DbFunc.Null;
            iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
            return new ObProperty<TSource>(iObProperty); ;
        }

        public static ObProperty<TSource> Avg<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase, new()
        {
            var iObProperty = keySelector(source);
            iObProperty.DbFunc = DbFunc.Avg;
            iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
            return new ObProperty<TSource>(iObProperty); ;
        }

        public static ObProperty<TSource> Count<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase, new()
        {
            var iObProperty = keySelector(source);
            iObProperty.DbFunc = DbFunc.Count;
            iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
            return new ObProperty<TSource>(iObProperty); ;
        }

        public static ObProperty<TSource> Max<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase, new()
        {
            var iObProperty = keySelector(source);
            iObProperty.DbFunc = DbFunc.Max;
            iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
            return new ObProperty<TSource>(iObProperty);
        }

        public static ObProperty<TSource> Min<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase, new()
        {
            var iObProperty = keySelector(source);
            iObProperty.DbFunc = DbFunc.Min;
            iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
            return new ObProperty<TSource>(iObProperty); ;
        }

        public static ObProperty<TSource> Sum<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase, new()
        {
            var iObProperty = keySelector(source);
            iObProperty.DbFunc = DbFunc.Sum;
            iObProperty.FuncBrotherCount = iObProperty.Brothers.Count;
            return new ObProperty<TSource>(iObProperty); ;
        }

        public static ObProperty<TSource> RowNumber<TSource>(this TSource source, Func<TSource, IObSort> keySelector)
            where TSource : ObTermBase, new()
        {
            var iObSort = keySelector(source);
            var iObProperty = new ObProperty<TSource>(iObSort.List.First().ObProperty)
            {
                DbFunc = DbFunc.RowNumber,
                Sort = iObSort
            };
            return iObProperty;
        }

        public static ObProperty<TSource> RowNumber<TSource>(this TSource source, Func<TSource, IObGroup> keySelector, Func<TSource, IObSort> keySelector2)
            where TSource : ObTermBase, new()
        {
            var iObSort = keySelector2(source);
            var iObGroup = keySelector(source);
            var iObProperty = new ObProperty<TSource>(iObSort.List.First().ObProperty)
            {
                DbFunc = DbFunc.RowNumber,
                Sort = iObSort,
                Group = iObGroup
            };
            return iObProperty;
        }

        [Obsolete]
        public static ObProperty<TSource> Custom<TSource>(this TSource source, string func, Func<TSource, object[]> keySelector)
            where TSource : ObTermBase, new()
        {
            var parameters = keySelector(source);
            if (!(parameters[0] is IObProperty))
                throw new Exception("自定义函数首个参数必须为IObProperty");
            var obProperty = new ObProperty<TSource>((IObProperty)parameters[0])
            {
                DbFunc = DbFunc.Custom,
                FuncName = func,
                CustomParams = parameters
            };
            return obProperty;
        }

        public static ObProperty<TSource> Custom<TSource, TKey>(this TSource source, string func, Func<TSource, TKey> keySelector)
            where TSource : ObTermBase, new()
        {
            var key = keySelector(source);
            var parameters = new List<object>();
            foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var k = propertyInfo.GetValue(key);
                parameters.Add(k);
            }
            if (!(parameters.First() is IObProperty property))
                throw new Exception("自定义函数首个参数必须为IObProperty");
            var obProperty = new ObProperty<TSource>(property)
            {
                DbFunc = DbFunc.Custom,
                FuncName = func,
                CustomParams = parameters.ToArray()
            };
            return obProperty;
        }


        #endregion

        /*        /// <summary>
                /// 创建显示属性
                /// </summary>
                /// <param name="obProperty">属性</param>
                /// <param name="dbFunc"></param>
                /// <returns></returns>
                private static IObProperty ObFun_Create(IObProperty obProperty, DbFunc dbFunc)
                {
                    Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
                    var parameters = new object[]
                                         {
                                             obProperty.ModelType,
                                             obProperty.TableName,
                                             obProperty.Brothers,
                                             obProperty.AriSymbol,
                                             obProperty.ColumnName,
                                             dbFunc
                                         };
                    return (IObProperty)Activator.CreateInstance(t, parameters);
                }*/
    }
}
