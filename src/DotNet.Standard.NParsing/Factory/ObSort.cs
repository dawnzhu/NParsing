/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-25 16:35:12
* 版 本 号：1.0.0
* 功能说明：实现接口创建的工厂类
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 09:30:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Frameworks.Hibernate.Factory->DotNet.Frameworks.Transport.Factory)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-29 14:30:00
* 版 本 号：1.0.1
* 修改内容：增加排序接口创建
* ----------------------------------
* 修改标识：创建
* 修 改 人：朱晓春
* 日    期：2010-03-29 16:00:00
* 版 本 号：1.0.1
* 修改内容：从DataAccess中分离
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2011-12-02 17:51:00
* 版 本 号：2.3.0
* 修改内容：代码整理
*/
using System;
using System.Collections.Generic;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObSort
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObSort";

        #region 基本方法

        /// <summary>
        /// 创建一个空的排序
        /// </summary>
        /// <returns></returns>
        public static IObSort Create()
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            return (IObSort)Activator.CreateInstance(t);
        }

/*        public static IObSort Create(ObProperty obProperty)
        {
            return ObSort_Create(obProperty, true);
        }*/

        /// <summary>
        /// 创建单个属性排序
        /// </summary>
        /// <param name="obProperty"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static IObSort Create(ObProperty obProperty, Sort sort = Sort.Ascending)
        {
            return ObSort_Create(obProperty, sort == Sort.Ascending);
        }

        /*public static IObSort Create(ObProperty[] obPropertys)
        {
            return ObSort_Create(obPropertys, true);
        }*/

        /// <summary>
        /// 创建多个属性排序
        /// </summary>
        /// <param name="obPropertys"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static IObSort Create(ObProperty[] obPropertys, Sort sort = Sort.Ascending)
        {
            return ObSort_Create(obPropertys, sort == Sort.Ascending);
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 创建单个属性排序
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IObSort<TSource> OrderBy<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            return ObSort_Create(source, keySelector(source), true);
        }

        public static IObSort<TSource> OrderByDescending<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            return ObSort_Create(source, keySelector(source), false);
        }

        /// <summary>
        /// 创建多个属性排序
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IObSort<TSource> OrderBy<TSource>(this TSource source, Func<TSource, ObProperty[]> keySelector)
            where TSource : ObTermBase
        {
            return ObSort_Create(source, keySelector(source), true);
        }

        public static IObSort<TSource> OrderBy<TSource, TKey>(this TSource source, Func<TSource, TKey> keySelector)
            where TSource : ObTermBase
        {
            var list = new List<ObProperty>();
            var key = keySelector(source);
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
            return ObSort_Create(source, list.ToArray(), true);
        }

        public static IObSort<TSource> OrderByDescending<TSource>(this TSource source, Func<TSource, ObProperty[]> keySelector)
            where TSource : ObTermBase
        {
            return ObSort_Create(source, keySelector(source), false);
        }

        public static IObSort<TSource> OrderByDescending<TSource, TKey>(this TSource source, Func<TSource, TKey> keySelector)
            where TSource : ObTermBase
        {
            var list = new List<ObProperty>();
            var key = keySelector(source);
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
            return ObSort_Create(source, list.ToArray(), false);
        }

        #endregion

        private static IObSort ObSort_Create(ObProperty obProperty, bool isAsc)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     isAsc
                                 };
            return (IObSort)Activator.CreateInstance(t, parameters);
        }

        private static IObSort ObSort_Create(ObProperty[] obPropertys, bool isAsc)
        {
#if DEBUG
            if (obPropertys.Length == 0)
                throw new Exception("至少要有一个ObProperty参数");
#endif
            IObSort obSort = null;
            foreach (var obProperty in obPropertys)
            {
                if (obSort == null)
                    obSort = ObSort_Create(obProperty, isAsc);
                else
                    obSort.AddOrderByDescending(obProperty);
            }
            return obSort;
        }

        private static IObSort<TSource> ObSort_Create<TSource>(TSource source, ObProperty obProperty, bool isAsc)
            where TSource : ObTermBase
        {
            var type = typeof(TSource);
            var className = CLASS_NAME + "`1[[" + type.FullName + "," + type.Assembly.FullName + "]]";
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                source,
                obProperty,
                isAsc
            };
            return (IObSort<TSource>)Activator.CreateInstance(t, parameters);
        }

        private static IObSort<TSource> ObSort_Create<TSource>(TSource source, ObProperty[] obPropertys, bool isAsc)
            where TSource : ObTermBase
        {
#if DEBUG
            if (obPropertys.Length == 0)
                throw new Exception("至少要有一个ObProperty参数");
#endif
            IObSort<TSource> obSort = null;
            foreach (var obProperty in obPropertys)
            {
                if (obSort == null)
                    obSort = ObSort_Create(source, obProperty, isAsc);
                else
                    obSort.AddOrderByDescending(obProperty);
            }
            return obSort;
        }
    }
}