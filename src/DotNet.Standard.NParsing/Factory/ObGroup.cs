/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-05-14 16:35:12
* 版 本 号：1.0.0
* 功能说明：实现创建分组的工厂类
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
    public static class ObGroup
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObGroup";

        #region 基本方法

        /// <summary>
        /// 创建一个空的分组
        /// </summary>
        /// <returns></returns>
        public static IObGroup Create()
        {
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            return (IObGroup)Activator.CreateInstance(t);
        }

        /// <summary>
        /// 创建单个属性分组
        /// </summary>
        /// <param name="obProperty"></param>
        /// <returns></returns>
        public static IObGroup Create(ObProperty obProperty)
        {
            return ObGroup_Create(obProperty);
        }

        /// <summary>
        /// 创建多个属性分组
        /// </summary>
        /// <param name="obPropertys"></param>
        /// <returns></returns>
        public static IObGroup Create(ObProperty[] obPropertys)
        {
            return ObGroup_Create(obPropertys);
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 创建单个属性分组
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IObGroup<TSource> GroupBy<TSource>(this TSource source, Func<TSource, ObProperty> keySelector)
            where TSource : ObTermBase
        {
            return ObGroup_Create(source, keySelector(source));
        }

        /// <summary>
        /// 创建多个属性分组
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IObGroup<TSource> GroupBy<TSource>(this TSource source, Func<TSource, ObProperty[]> keySelector)
            where TSource : ObTermBase
        {
            return ObGroup_Create(source, keySelector(source));
        }

        /// <summary>
        /// 创建多个属性分组
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IObGroup<TSource> GroupBy<TSource, TKey>(this TSource source, Func<TSource, TKey> keySelector)
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
            return ObGroup_Create(source, list.ToArray());
        }

        #endregion

        /// <summary>
        /// 创建分组
        /// </summary>
        /// <param name="obProperty"></param>
        /// <returns></returns>
        private static IObGroup ObGroup_Create(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty
                                 };
            return (IObGroup)Activator.CreateInstance(t, parameters);
        }

        private static IObGroup ObGroup_Create(ObProperty[] obPropertys)
        {
#if DEBUG
            if (obPropertys.Length == 0)
                throw new Exception("至少要有一个ObProperty参数");
#endif
            IObGroup obGroup = null;
            foreach (var obProperty in obPropertys)
            {
                if (obGroup == null)
                    obGroup = ObGroup_Create(obProperty);
                else
                    obGroup.AddGroupBy(obProperty);
            }
            return obGroup;
        }

        /// <summary>
        /// 创建分组
        /// </summary>
        /// <param name="source"></param>
        /// <param name="obProperty"></param>
        /// <returns></returns>
        private static IObGroup<TSource> ObGroup_Create<TSource>(TSource source, ObProperty obProperty)
            where TSource : ObTermBase
        {
            var type = typeof(TSource);
            var className = CLASS_NAME + "`1[[" + type.FullName + "," + type.Assembly.FullName + "]]";
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                source,
                obProperty
            };
            return (IObGroup<TSource>)Activator.CreateInstance(t, parameters);
        }

        private static IObGroup<TSource> ObGroup_Create<TSource>(TSource source, ObProperty[] obPropertys)
            where TSource : ObTermBase
        {
#if DEBUG
            if (obPropertys.Length == 0)
                throw new Exception("至少要有一个ObProperty参数");
#endif
            IObGroup<TSource> obGroup = null;
            foreach (var obProperty in obPropertys)
            {
                if (obGroup == null)
                    obGroup = ObGroup_Create(source, obProperty);
                else
                    obGroup.AddGroupBy(obProperty);
            }
            return obGroup;
        }
    }
}
