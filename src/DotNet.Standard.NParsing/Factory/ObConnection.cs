/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-27 09:36:26
* 版 本 号：1.0.0
* 功能说明：实现本框架事务接口创建的工厂类
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 09:31:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Frameworks.Hibernate.Factory->DotNet.Frameworks.Transport.Factory)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-31 10:00:00
* 版 本 号：1.0.2
* 修改内容：实例化DotNet.Frameworks.Transport.DbUtilities.ObTransaction类
*/
using System;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObConnection
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObTransaction";

        public static IObTransaction BeginTransaction(string connectionString, string providerName)
        {
            return ObConnection_Create(connectionString, providerName);
        }

        private static IObTransaction ObConnection_Create(string connectionString, string providerName)
        {
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     connectionString,
                                     providerName
                                 };
            return (IObTransaction)Activator.CreateInstance(t, parameters);
        }
    }
}