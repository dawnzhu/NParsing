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
* 日    期：2010-03-31 10:00:00
* 版 本 号：1.0.2
* 修改内容：实例化DotNet.Frameworks.Transport.DbUtilities.ObHelper类
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2011-12-02 17:51:00
* 版 本 号：2.3.0
* 修改内容：代码整理
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2013-06-17 19:51:00
* 版 本 号：2.5.0
* 修改内容：增加模型类名称重定义功能
*/
using System;
using System.Collections.Generic;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObHelper
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObHelper";

        #region 基础方法

        public static IObHelper Create(string connectionString, string providerName)
        {
            return ObHelper_Create(connectionString, providerName);
        }

        public static IObHelper Create(string readConnectionString, string writeConnectionString, string providerName)
        {
            return ObHelper_Create(readConnectionString, writeConnectionString, providerName);
        }

        public static IObHelper<TModel> Create<TModel>(string connectionString, string providerName)
        {
            return ObHelper_Create<TModel>(connectionString, providerName, null, null);
        }

        public static IObHelper<TModel> Create<TModel>(string connectionString, string providerName, IObRedefine iObRedefine)
        {
            return ObHelper_Create<TModel>(connectionString, providerName, iObRedefine, null);
        }

        public static IObHelper<TModel> Create<TModel>(string connectionString, string providerName, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(connectionString, providerName, null, notJoinModels);
        }

        /// <summary>
        /// 创建数据库操作接口，传入数据库连接字符串和数据库操作类库名称，方便数据库连接字符串加密存储
        /// </summary>
        /// <typeparam name="TModel">对象模型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">数据库操作类库名称</param>
        /// <param name="iObRedefine"></param>
        /// <param name="notJoinModels"></param>
        /// <returns></returns>
        public static IObHelper<TModel> Create<TModel>(string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(connectionString, providerName, iObRedefine, notJoinModels);
        }

        internal static IObHelper<IObModel> Create(Type mt, string connectionString, string providerName)
        {
            return ObHelper_Create(mt, connectionString, providerName, null, null);
        }

        internal static IObHelper<IObModel> Create(Type mt, string connectionString, string providerName, IObRedefine iObRedefine)
        {
            return ObHelper_Create(mt, connectionString, providerName, iObRedefine, null);
        }

        internal static IObHelper<IObModel> Create(Type mt, string connectionString, string providerName, IList<string> notJoinModels)
        {
            return ObHelper_Create(mt, connectionString, providerName, null, notJoinModels);
        }

        internal static IObHelper<IObModel> Create(Type mt, string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            return ObHelper_Create(mt, connectionString, providerName, iObRedefine, notJoinModels);
        }

        public static IObHelper<TModel> Create<TModel>(string readConnectionString, string writeConnectionString, string providerName)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, null, null);
        }

        public static IObHelper<TModel> Create<TModel>(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, iObRedefine, null);
        }

        public static IObHelper<TModel> Create<TModel>(string readConnectionString, string writeConnectionString, string providerName, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, null, notJoinModels);
        }

        public static IObHelper<TModel> Create<TModel>(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, iObRedefine, notJoinModels);
        }

        internal static IObHelper<IObModel> Create(Type mt, string readConnectionString, string writeConnectionString, string providerName)
        {
            return ObHelper_Create(mt, readConnectionString, writeConnectionString, providerName, null, null);
        }

        internal static IObHelper<IObModel> Create(Type mt, string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine)
        {
            return ObHelper_Create(mt, readConnectionString, writeConnectionString, providerName, iObRedefine, null);
        }

        internal static IObHelper<IObModel> Create(Type mt, string readConnectionString, string writeConnectionString, string providerName, IList<string> notJoinModels)
        {
            return ObHelper_Create(mt, readConnectionString, writeConnectionString, providerName, null, notJoinModels);
        }

        internal static IObHelper<IObModel> Create(Type mt, string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            return ObHelper_Create(mt, readConnectionString, writeConnectionString, providerName, iObRedefine, notJoinModels);
        }



        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string connectionString, string providerName)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(connectionString, providerName, null, null);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string connectionString, string providerName, IObRedefine iObRedefine)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(connectionString, providerName, iObRedefine, null);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string connectionString, string providerName, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(connectionString, providerName, null, notJoinModels);
        }

        /// <summary>
        /// 创建数据库操作接口，传入数据库连接字符串和数据库操作类库名称，方便数据库连接字符串加密存储
        /// </summary>
        /// <typeparam name="TModel">对象模型</typeparam>
        /// <typeparam name="TTerm"></typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">数据库操作类库名称</param>
        /// <param name="iObRedefine"></param>
        /// <param name="notJoinModels"></param>
        /// <returns></returns>
        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(connectionString, providerName, iObRedefine, notJoinModels);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string readConnectionString, string writeConnectionString, string providerName)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(readConnectionString, writeConnectionString, providerName, null, null);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(readConnectionString, writeConnectionString, providerName, iObRedefine, null);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string readConnectionString, string writeConnectionString, string providerName, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(readConnectionString, writeConnectionString, providerName, null, notJoinModels);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(readConnectionString, writeConnectionString, providerName, iObRedefine, notJoinModels);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(TTerm term, string connectionString, string providerName)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(term, connectionString, providerName, term.ObRedefine, term.NotJoinModels);
        }

        public static IObHelper<TModel, TTerm> Create<TModel, TTerm>(TTerm term, string readConnectionString, string writeConnectionString, string providerName)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(term, readConnectionString, writeConnectionString, providerName, term.ObRedefine, term.NotJoinModels);
        }

        #endregion

        #region 扩展方法

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string connectionString, string providerName)
            where TModel : ObModelBase
        {
            return ObHelper_Create<TModel>(connectionString, providerName, null, null);
        }

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string connectionString, string providerName, IObRedefine iObRedefine)
        {
            return ObHelper_Create<TModel>(connectionString, providerName, iObRedefine, null);
        }

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string connectionString, string providerName, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(connectionString, providerName, null, notJoinModels);
        }

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(connectionString, providerName, iObRedefine, notJoinModels);
        }

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string readConnectionString, string writeConnectionString, string providerName)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, null, null);
        }

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, iObRedefine, null);
        }

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string readConnectionString, string writeConnectionString, string providerName, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, null, notJoinModels);
        }

        public static IObHelper<TModel> Helper<TModel>(this TModel m, string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            return ObHelper_Create<TModel>(readConnectionString, writeConnectionString, providerName, iObRedefine, notJoinModels);
        }


        public static IObHelper<TModel, TTerm> Helper<TModel, TTerm>(this TTerm term, string connectionString, string providerName)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(term, connectionString, providerName, term.ObRedefine, term.NotJoinModels);
        }

        public static IObHelper<TModel, TTerm> Helper<TModel, TTerm>(this TTerm term, string readConnectionString, string writeConnectionString, string providerName)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            return ObHelper_Create<TModel, TTerm>(term, readConnectionString, writeConnectionString, providerName, term.ObRedefine, term.NotJoinModels);
        }

        #endregion

        /// <summary>
        /// 创建数据库操作接口，传入数据库连接字符串和数据库操作类库名称，方便数据库连接字符串加密存储
        /// </summary>
        /// <typeparam name="TModel">对象模型</typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">数据库操作类库名称</param>
        /// <param name="iObRedefine"></param>
        /// <param name="notJoinModels"></param>
        /// <returns></returns>
        private static IObHelper<TModel> ObHelper_Create<TModel>(string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            var t = typeof(TModel);
            var className = CLASS_NAME + "`1[[" + t.FullName + "," + t.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
                                 {
                                     connectionString,
                                     providerName,
                                     iObRedefine,
                                     notJoinModels
                                 };
            return (IObHelper<TModel>)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper<TModel> ObHelper_Create<TModel>(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            var t = typeof(TModel);
            var className = CLASS_NAME + "`1[[" + t.FullName + "," + t.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                readConnectionString,
                writeConnectionString,
                providerName,
                iObRedefine,
                notJoinModels
            };
            return (IObHelper<TModel>)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper<IObModel> ObHelper_Create(Type t, string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            var className = CLASS_NAME + "`1[[" + t.FullName + "," + t.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
                                 {
                                     connectionString,
                                     providerName,
                                     iObRedefine,
                                     notJoinModels
                                 };
            return (IObHelper<IObModel>)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper<IObModel> ObHelper_Create(Type t, string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            var className = CLASS_NAME + "`1[[" + t.FullName + "," + t.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                readConnectionString,
                writeConnectionString,
                providerName,
                iObRedefine,
                notJoinModels
            };
            return (IObHelper<IObModel>)Activator.CreateInstance(t, parameters);
        }

        /// <summary>
        /// 创建数据库操作接口，传入数据库连接字符串和数据库操作类库名称，方便数据库连接字符串加密存储
        /// </summary>
        /// <typeparam name="TModel">对象模型</typeparam>
        /// <typeparam name="TTerm"></typeparam>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="providerName">数据库操作类库名称</param>
        /// <param name="iObRedefine"></param>
        /// <param name="notJoinModels"></param>
        /// <returns></returns>
        private static IObHelper<TModel, TTerm> ObHelper_Create<TModel, TTerm>(string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            var t = typeof(TModel);
            var t2 = typeof(TTerm);
            var className = CLASS_NAME + "`2[[" + t.FullName + "," + t.Assembly.FullName + "],[" + t2.FullName + "," + t2.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
                                 {
                                     connectionString,
                                     providerName,
                                     iObRedefine,
                                     notJoinModels
                                 };
            return (IObHelper<TModel, TTerm>)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper<TModel, TTerm> ObHelper_Create<TModel, TTerm>(TTerm term, string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            var t = typeof(TModel);
            var t2 = typeof(TTerm);
            var className = CLASS_NAME + "`2[[" + t.FullName + "," + t.Assembly.FullName + "],[" + t2.FullName + "," + t2.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                term,
                connectionString,
                providerName,
                iObRedefine,
                notJoinModels
            };
            return (IObHelper<TModel, TTerm>)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper<TModel, TTerm> ObHelper_Create<TModel, TTerm>(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            var t = typeof(TModel);
            var t2 = typeof(TTerm);
            var className = CLASS_NAME + "`2[[" + t.FullName + "," + t.Assembly.FullName + "],[" + t2.FullName + "," + t2.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                readConnectionString,
                writeConnectionString,
                providerName,
                iObRedefine,
                notJoinModels
            };
            return (IObHelper<TModel, TTerm>)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper<TModel, TTerm> ObHelper_Create<TModel, TTerm>(TTerm term, string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
            where TModel : ObModelBase
            where TTerm : ObTermBase
        {
            var t = typeof(TModel);
            var t2 = typeof(TTerm);
            var className = CLASS_NAME + "`2[[" + t.FullName + "," + t.Assembly.FullName + "],[" + t2.FullName + "," + t2.Assembly.FullName + "]]";
            t = Assembly.Load(ASSEMBLY_STRING).GetType(className);
            var parameters = new object[]
            {
                term,
                readConnectionString,
                writeConnectionString,
                providerName,
                iObRedefine,
                notJoinModels
            };
            return (IObHelper<TModel, TTerm>)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper ObHelper_Create(string connectionString, string providerName)
        {
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     connectionString,
                                     providerName
                                 };
            return (IObHelper)Activator.CreateInstance(t, parameters);
        }

        private static IObHelper ObHelper_Create(string readConnectionString, string writeConnectionString, string providerName)
        {
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
            {
                readConnectionString,
                writeConnectionString,
                providerName
            };
            return (IObHelper)Activator.CreateInstance(t, parameters);
        }
    }
}