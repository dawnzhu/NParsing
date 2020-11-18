using System;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObRedefine
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObRedefine";

        #region 基本方法

/*        /// <summary>
        /// 创建空的模型类名称重定义
        /// </summary>
        /// <returns></returns>
        public static IObRedefine Create()
        {
            return ObRedefine_Create();
        }*/

        /// <summary>
        /// 创建模型类名称重定义
        /// </summary>
        /// <param name="rename"></param>
        /// <returns></returns>
        public static IObRedefine Create<TModel>(string rename)
        {
            return Create(typeof(TModel), null, rename);
        }

        public static IObRedefine Create(Type mt, string rename)
        {
            return Create(mt, null, rename);
        }

        /*public static IObRedefine Create<TModel>(ObTermBase parent, string rename)
        {
            return Create(typeof(TModel), parent, rename);
        }*/

        internal static IObRedefine Create(Type mt, ObTermBase parent, string rename)
        {
            if (parent != null)
                rename = parent.ObTableName + "_" + rename;
            return ObRedefine_Create(mt, rename);
        }

        /*public static IObRedefine Create<TModel>(MethodBase currentMethod)
        {
            return Create(typeof(TModel), null, currentMethod.Name.Substring(4));
        }

        internal static IObRedefine Create(Type mt, MethodBase currentMethod)
        {
            return Create(mt, null, currentMethod.Name.Substring(4));
        }*/

        public static IObRedefine Create<TModel>(ObTermBase parent, MethodBase currentMethod)
        {
            return Create(typeof(TModel), parent, currentMethod.Name.Substring(4));
        }

        internal static IObRedefine Create(Type mt, ObTermBase parent, MethodBase currentMethod)
        {
            return Create(mt, parent, currentMethod.Name.Substring(4));
        }

        #endregion

        #region 扩展方法

        /*        /// <summary>
                /// 创建空的模型类名称重定义
                /// </summary>
                /// <returns></returns>
                public static IObRedefine Redefine()
                {
                    return ObRedefine_Create();
                }*/

        /// <summary>
        /// 创建模型类名称重定义
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="rename"></param>
        /// <returns></returns>
        public static IObRedefine Redefine<TModel>(this TModel model, string rename)
        {
            return ObRedefine_Create(typeof(TModel), rename);
        }

        public static IObRedefine Redefine<TModel>(this TModel model, MethodBase currentMethod)
        {
            return ObRedefine_Create(typeof(TModel), currentMethod.Name.Substring(4));
        }

        #endregion

        private static IObRedefine ObRedefine_Create(Type mt, string rename)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     mt,
                                     rename
                                 };
            return (IObRedefine)Activator.CreateInstance(t, parameters);
        }

/*        private static IObRedefine ObRedefine_Create()
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            return (IObRedefine)Activator.CreateInstance(t);
        }*/
    }
}
