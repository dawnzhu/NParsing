/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-26 23:22:48
* 版 本 号：1.0.0
* 功能说明：本框架事务接口
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-11 22:21:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Frameworks.Hibernate.Interface->DotNet.Frameworks.Transport.Interface)和接口名(IO2rTransaction->IObTransaction)
*/
using System;
using System.Data.Common;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObTransaction : IDisposable
    {
        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();

        DbTransaction DbTransaction { get; }

        string ConnectionString { get; }

        string ProviderName { get;}
    }
}