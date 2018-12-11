/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-26 23:26:21
* 版 本 号：1.0.0
* 功能说明：本框架事务实现类
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 09:03:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Standard.Hibernate.SQLServer->DotNet.Standard.Transport.SQLServer)和类名(O2rTransaction->ObTransaction)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-31 09:24:00
* 版 本 号：1.0.2
* 修改内容：类移至DotNet.Standard.Transport.DbUtilities程序集，提供公用的事务服务
*/
using System;
using System.Data.Common;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObTransaction : IObTransaction
    {
        private readonly DbConnection _dbConnection;

        public ObTransaction(string connectionString, string providerName)
        {
            ProviderName = providerName;
            ConnectionString = connectionString;
            var className = providerName + ".DbHelper";
            var t = Assembly.Load(providerName).GetType(className);
            var iDbHelper = (IDbHelper)Activator.CreateInstance(t, connectionString);
            _dbConnection = iDbHelper.DbConnection();
            _dbConnection.Open();
            DbTransaction = _dbConnection.BeginTransaction();
        }

        #region IObTransaction Members

        public void Dispose()
        {
            DbTransaction?.Dispose();
            _dbConnection?.Close();
        }

        public void Commit()
        {
            DbTransaction.Commit();
        }   

        public void Rollback()
        {
            DbTransaction.Rollback();
        }

        public DbTransaction DbTransaction
        {
            get;
        }

        public string ConnectionString
        {
            get;
        }

        public string ProviderName
        {
            get;
        }

        #endregion
    }
}