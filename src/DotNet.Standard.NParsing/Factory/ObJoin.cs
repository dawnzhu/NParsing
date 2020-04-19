using System;
using System.Collections.Generic;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObJoin
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObJoin";

        #region 基本方法

        /// <summary>
        /// 创建一个空的关联
        /// </summary>
        /// <returns></returns>
        public static IObJoin Create()
        {
            return ObJoin_Create();
        }

        /// <summary>
        /// 创建单个属性关联
        /// </summary>
        /// <param name="obTermBase"></param>
        /// <returns></returns>
        public static IObJoin Create(ObTermBase obTermBase)
        {
            return ObJoin_Create(obTermBase);
        }

        /// <summary>
        /// 创建多个属性关联
        /// </summary>
        /// <param name="obTermBases"></param>
        /// <returns></returns>
        public static IObJoin Create(ObTermBase[] obTermBases)
        {
            return ObJoin_Create(obTermBases);
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 创建一个空属性关联
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObJoin Join<TSource>(this TSource source)
            where TSource : ObTermBase
        {
            return ObJoin_Create(source);
        }

        /// <summary>
        /// 创建单个属性关联
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IObJoin<TSource> Join<TSource>(this TSource source, Func<TSource, ObTermBase> keySelector)
            where TSource : ObTermBase
        {
            return ObJoin_Create(source, keySelector(source));
        }

        /// <summary>
        /// 创建多个属性关联
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IObJoin<TSource> Join<TSource>(this TSource source, Func<TSource, ObTermBase[]> keySelector)
            where TSource : ObTermBase
        {
            return ObJoin_Create(source, keySelector(source));
        }

        public static IObJoin<TSource> Join<TSource, TKey>(this TSource source, Func<TSource, TKey> keySelector)
            where TSource : ObTermBase
        {
            var list = new List<ObTermBase>();
            var key = keySelector(source);
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
            return ObJoin_Create(source, list.ToArray());
        }

        #endregion

        /// <summary>
        /// 创建关联
        /// </summary>
        /// <returns></returns>
        private static IObJoin<TSource> ObJoin_Create<TSource>(TSource source)
            where TSource : ObTermBase
        {
            var type = typeof(TSource);
            var className = CLASS_NAME + "`1[[" + type.FullName + "," + type.Assembly.FullName + "]]";
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                source
            };
            return (IObJoin<TSource>)Activator.CreateInstance(t, parameters);
        }

        private static IObJoin<TSource> ObJoin_Create<TSource>(TSource source, ObTermBase obTermBase)
            where TSource : ObTermBase
        {
            var type = typeof(TSource);
            var className = CLASS_NAME + "`1[[" + type.FullName + "," + type.Assembly.FullName + "]]";
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                source,
                obTermBase
            };
            return (IObJoin<TSource>)Activator.CreateInstance(t, parameters);
        }

        private static IObJoin<TSource> ObJoin_Create<TSource>(TSource source, ObTermBase[] obTermBases)
            where TSource : ObTermBase
        {
#if DEBUG
            if (obTermBases.Length == 0)
                throw new Exception("至少要有一个ObTermBase参数");
#endif
            var obJoin = ObJoin_Create<TSource>(source);
            obJoin.AddJoin(obTermBases);
            return obJoin;
        }

        /// <summary>
        /// 创建关联
        /// </summary>
        /// <returns></returns>
        private static IObJoin ObJoin_Create()
        {
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            return (IObJoin)Activator.CreateInstance(t);
        }

        private static IObJoin ObJoin_Create(ObTermBase obTermBase)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
            {
                obTermBase
            };
            return (IObJoin)Activator.CreateInstance(t, parameters);
        }

        private static IObJoin ObJoin_Create(ObTermBase[] obTermBases)
        {
#if DEBUG
            if (obTermBases.Length == 0)
                throw new Exception("至少要有一个ObTermBase参数");
#endif
            var obJoin = ObJoin_Create();
            obJoin.AddJoin(obTermBases);
            return obJoin;
        }
    }
}
