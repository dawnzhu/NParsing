/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-25 16:47:56
* 版 本 号：1.0.0
* 功能说明：对象模型/关系模型互转实现类(数据库操作实现类)
* ----------------------------------
* 修改标识：增加功能
* 修 改 人：朱晓春
* 日    期：2009-08-28 14:32:50
* 版 本 号：1.0.0
* 修改内容：增加了获取记录数的功能 GetCount
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2009-09-01 15:35:01
* 版 本 号：1.0.0
* 修改内容：修改以参数方式访问数据库
* ----------------------------------
* 修改标识：增加功能接口
* 修 改 人：朱晓春
* 日    期：2009-09-04 16:55:20
* 版 本 号：1.0.0
* 修改内容：增加了获取最大值方法 object GetMaxScalar
* ----------------------------------
* 修改标识：增加功能接口
* 修 改 人：朱晓春
* 日    期：2010-03-09 12:29:00
* 版 本 号：1.0.0
* 修改内容：增加了检查是否存方法 bool Exists
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 09:11:01
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Standard.Hibernate->DotNet.Standard.Transport.SQLServer)和类名(ObjectToRelational->ObHelper)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 21:45:00
* 版 本 号：1.0.1
* 修改内容：增加了获取最小值方法 object GetMinScalar
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-16 22:24:00
* 版 本 号：1.0.1
* 修改内容：增加了IList<M> GetList排序功能
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-31 09:24:00
* 版 本 号：1.0.2
* 修改内容：类移至DotNet.Standard.Transport.DbUtilities程序集，提供公用的OR映射服务
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-16 22:24:00
* 版 本 号：1.0.1
* 修改内容：增加了IList<M> GetList分组功能
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-07-27 12:18:00
* 版 本 号：2.0.1
* 修改内容：修改添加操作返回值 bool -> object
*           修改更新操作返回值 bool -> int
*           修改删除操作返回值 bool -> int
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2013-06-17 19:51:00
* 版 本 号：2.5.0
* 修改内容：增加模型类名称重定义功能
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObHelper : IObHelper
    {
        private readonly IDbHelper _iReadDbHelper;
        private readonly IDbHelper _iWriteDbHelper;
        private readonly ISqlBuilder _iSqlBuilder;
        public ObHelper(string connectionString, string providerName)
        {
            var className = providerName + ".SqlBuilder";
            var t = Assembly.Load(providerName).GetType(className);
            _iSqlBuilder = (ISqlBuilder)Activator.CreateInstance(t);

            className = providerName + ".DbHelper";
            t = Assembly.Load(providerName).GetType(className);
            _iWriteDbHelper = (IDbHelper)Activator.CreateInstance(t, connectionString);
            _iReadDbHelper = _iWriteDbHelper;
        }

        public ObHelper(string readConnectionString, string writeConnectionString, string providerName)
        {
            var className = providerName + ".SqlBuilder";
            var t = Assembly.Load(providerName).GetType(className);
            _iSqlBuilder = (ISqlBuilder)Activator.CreateInstance(t);

            className = providerName + ".DbHelper";
            t = Assembly.Load(providerName).GetType(className);
            _iWriteDbHelper = (IDbHelper)Activator.CreateInstance(t, writeConnectionString);
            _iReadDbHelper = (IDbHelper) Activator.CreateInstance(t, readConnectionString);
        }

        #region 创建数据库 public bool Create(string name)

        public bool Create(string name)
        {
            var sql = _iSqlBuilder.CreateDatabase(name);
            using (var dbHelper = new DbHelper(_iWriteDbHelper))
            {
                return dbHelper.ExecuteScalar(sql) != null;
            }
        }

        public bool Create(IObTransaction iObTransaction, string name)
        {
            var sql = _iSqlBuilder.CreateDatabase(name);
            return DbHelper.ExecuteScalar(_iWriteDbHelper, iObTransaction.DbTransaction, sql) != null;
        }

        #endregion

        #region 删除数据库 public bool Drop(string name)

        public bool Drop(string name)
        {
            var sql = _iSqlBuilder.DropDatabase(name);
            using (var dbHelper = new DbHelper(_iWriteDbHelper))
            {
                return dbHelper.ExecuteScalar(sql) != null;
            }
        }

        public bool Drop(IObTransaction iObTransaction, string name)
        {
            var sql = _iSqlBuilder.DropDatabase(name);
            return DbHelper.ExecuteScalar(_iWriteDbHelper, iObTransaction.DbTransaction, sql) != null;
        }

        #endregion

        #region NonQuery

        public int NonQuery(string sqlText, params DbParameter[] commandParameters)
        {
            using (var dbHelper = new DbHelper(_iWriteDbHelper))
            {
                return dbHelper.ExecuteNonQuery(sqlText, commandParameters);
            }
        }

        public int NonQuery(IObTransaction iObTransaction, string sqlText, params DbParameter[] commandParameters)
        {
            return DbHelper.ExecuteNonQuery(_iWriteDbHelper, iObTransaction.DbTransaction, sqlText, commandParameters);
        }

        public int NonQueryByStoredProcedure(string spName, params DbParameter[] commandParameters)
        {
            using (var dbHelper = new DbHelper(_iWriteDbHelper))
            {
                return dbHelper.ExecuteNonQuery(CommandType.StoredProcedure, spName, commandParameters);
            }
        }

        public int NonQueryByStoredProcedure(IObTransaction iObTransaction, string spName, params DbParameter[] commandParameters)
        {
            return DbHelper.ExecuteNonQuery(_iWriteDbHelper, iObTransaction.DbTransaction, CommandType.StoredProcedure, spName, commandParameters);
        }

        #endregion

        #region GetDataset

        public DataSet GetDataset(string sqlText, params DbParameter[] commandParameters)
        {
            using (var dbHelper = new DbHelper(_iReadDbHelper))
            {
                return dbHelper.ExecuteDataset(sqlText, commandParameters);
            }
        }

        public DataSet GetDataset(IObTransaction iObTransaction, string sqlText, params DbParameter[] commandParameters)
        {
            return DbHelper.ExecuteDataset(_iReadDbHelper, iObTransaction.DbTransaction, sqlText, commandParameters);
        }

        public DataSet GetDatasetByStoredProcedure(string spName, params DbParameter[] commandParameters)
        {
            using (var dbHelper = new DbHelper(_iReadDbHelper))
            {
                return dbHelper.ExecuteDataset(CommandType.StoredProcedure, spName, commandParameters);
            }
        }

        public DataSet GetDatasetByStoredProcedure(IObTransaction iObTransaction, string spName, params DbParameter[] commandParameters)
        {
            return DbHelper.ExecuteDataset(_iReadDbHelper, iObTransaction.DbTransaction, CommandType.StoredProcedure, spName, commandParameters);
        }

        #endregion

        #region GetScalar

        public object GetScalar(string sqlText, params DbParameter[] commandParameters)
        {
            using (var dbHelper = new DbHelper(_iReadDbHelper))
            {
                return dbHelper.ExecuteScalar(sqlText, commandParameters);
            }
        }

        public object GetScalar(IObTransaction iObTransaction, string sqlText, params DbParameter[] commandParameters)
        {
            return DbHelper.ExecuteScalar(_iReadDbHelper, iObTransaction.DbTransaction, sqlText, commandParameters);
        }

        public object GetScalarByStoredProcedure(string spName, params DbParameter[] commandParameters)
        {
            using (var dbHelper = new DbHelper(_iReadDbHelper))
            {
                return dbHelper.ExecuteScalar(CommandType.StoredProcedure, spName, commandParameters);
            }
        }

        public object GetScalarByStoredProcedure(IObTransaction iObTransaction, string spName, params DbParameter[] commandParameters)
        {
            return DbHelper.ExecuteScalar(_iReadDbHelper, iObTransaction.DbTransaction, CommandType.StoredProcedure, spName, commandParameters);
        }

        #endregion
    }

    public class ObHelper<TModel> : IObHelper<TModel> where TModel : class, new()
    {
        private IObJoin _iObJoin;
        public IObJoin ObJoin
        {
            get => _iObJoin;
            protected set
            {
                _iObJoin = value;
                if (value != null)
                {
                    SqlBuilder.JoinModels = value.JoinModels;
                }
            }
        }

        protected IDbHelper ReadDbHelper { get; }
        protected IDbHelper WriteDbHelper { get; }
        protected ISqlBuilder<TModel> SqlBuilder { get; }
        protected string ProviderName { get; }
        public ObHelper(string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            ProviderName = providerName;
            if(iObRedefine == null)
               iObRedefine = new ObRedefine(); 
            var className = providerName + ".SqlBuilder`1";
            var t = typeof(TModel);
            className += "[[" + t.FullName + "," + t.Assembly.FullName + "]]";
            t = Assembly.Load(providerName).GetType(className);
            SqlBuilder = (ISqlBuilder<TModel>)Activator.CreateInstance(t, iObRedefine, notJoinModels);

            className = providerName + ".DbHelper";
            t = Assembly.Load(providerName).GetType(className);
            WriteDbHelper = (IDbHelper)Activator.CreateInstance(t, connectionString);
            ReadDbHelper = WriteDbHelper;
        }

        public ObHelper(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            ProviderName = providerName;
            if (iObRedefine == null)
                iObRedefine = new ObRedefine();
            var className = providerName + ".SqlBuilder`1";
            var t = typeof(TModel);
            className += "[[" + t.FullName + "," + t.Assembly.FullName + "]]";
            t = Assembly.Load(providerName).GetType(className);
            SqlBuilder = (ISqlBuilder<TModel>)Activator.CreateInstance(t, iObRedefine, notJoinModels);

            className = providerName + ".DbHelper";
            t = Assembly.Load(providerName).GetType(className);
            ReadDbHelper = (IDbHelper)Activator.CreateInstance(t, readConnectionString);
            WriteDbHelper = (IDbHelper)Activator.CreateInstance(t, writeConnectionString);
        }

        #region 创建表 public bool Create()

        public bool Create()
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                string sql = SqlBuilder.CreateTable();
                return dbHelper.ExecuteScalar(sql) != null;
            }
        }

        public bool Create(IObTransaction iObTransaction)
        {
            string sql = SqlBuilder.CreateTable();
            return DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sql) != null;
        }

        public bool Create(string name)
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                string sql = SqlBuilder.CreateTable(name);
                return dbHelper.ExecuteScalar(sql) != null;
            }
        }

        public bool Create(IObTransaction iObTransaction, string name)
        {
            string sql = SqlBuilder.CreateTable(name);
            return DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sql) != null;
        }

        #endregion

        #region 删除表 public bool Drop()

        public bool Drop()
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                string sql = SqlBuilder.DropTable();
                return dbHelper.ExecuteScalar(sql) != null;
            }
        }

        public bool Drop(IObTransaction iObTransaction)
        {
            string sql = SqlBuilder.DropTable();
            return DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sql) != null;
        }

        #endregion

        #region 创建表索引 CreateIndex(string name, IObSort iObSort, string fileGroup)

        public bool CreateIndex(string name, IObSort iObSort, string fileGroup)
        {
            foreach (var dbSort in iObSort.List)
            {
                dbSort.TableName = "";
            }
            var obSort = ObConvert.ToObSort(ProviderName, iObSort);
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                string sql = SqlBuilder.CreateIndex(name, obSort, fileGroup);
                return dbHelper.ExecuteScalar(sql) != null;
            }
        }

        public bool CreateIndex(IObTransaction iObTransaction, string name, IObSort iObSort, string fileGroup)
        {
            foreach (var dbSort in iObSort.List)
            {
                dbSort.TableName = "";
            }
            var obSort = ObConvert.ToObSort(ProviderName, iObSort);
            string sql = SqlBuilder.CreateIndex(name, obSort, fileGroup);
            return DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sql) != null;
        }

        #endregion

        #region 删除表索引 public bool DropIndex(string name)

        public bool DropIndex(string name)
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                string sql = SqlBuilder.DropIndex(name);
                return dbHelper.ExecuteScalar(sql) != null;
            }
        }

        public bool DropIndex(IObTransaction iObTransaction, string name)
        {
            string sql = SqlBuilder.DropIndex(name);
            return DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sql) != null;
        }

        #endregion

        #region 检查是否存在 public bool Exists(IObParameter iObParameter)

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public bool Exists(IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(ProviderName, iObParameter);
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                string sql = SqlBuilder.ExistsSelect(iObParameter, ref dbParameters);
                return dbHelper.Exists(sql, ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region  检查是否存在 public bool Exists(IObTransaction iObTransaction, IObParameter iObParameter)

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="iObTransaction"></param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public bool Exists(IObTransaction iObTransaction, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(iObTransaction.ProviderName, iObParameter);
            IList<DbParameter> dbParameters = new List<DbParameter>();
            string sql = SqlBuilder.ExistsSelect(iObParameter, ref dbParameters);
            return DbHelper.Exists(WriteDbHelper, iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion


        #region 添加一个对象到数据库 public object Add(M model)

        /// <summary>
        /// 添加一个对象到数据库
        /// </summary>
        /// <param name="model">对象</param>
        /// <returns>
        /// 如果有标识列，返回标识列的值
        /// 没有，则返回影响行数
        /// </returns>
        public object Add(TModel model)
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                var sqls = SqlBuilder.Insert(model, ref dbParameters);

                #region 解决Oracle序列返回问题（使用输出参数返回）

                DbParameter outputParameter = null;
                foreach (var dbParameter in dbParameters)
                {
                    if (dbParameter.Direction != ParameterDirection.Output) continue;
                    outputParameter = dbParameter;
                    break;
                }
                if (outputParameter != null)
                {
                    dbHelper.ExecuteNonQuery(sqls, ((List<DbParameter>) dbParameters).ToArray());
                    return outputParameter.Value;
                }

                #endregion

                return sqls.Length > 1 ? 
                    dbHelper.ExecuteScalar(sqls, ((List<DbParameter>)dbParameters).ToArray()) :
                    dbHelper.ExecuteNonQuery(sqls[0], ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region 添加一个对象到数据库 public object Add(IObTransaction iObTransaction, M model)

        /// <summary>
        /// 添加一个对象到数据库
        /// </summary>
        /// <param name="iObTransaction"></param>
        /// <param name="model">对象</param>
        /// <returns>
        /// 如果有标识列，返回标识列的值
        /// 没有，则返回影响行数
        /// </returns>
        public object Add(IObTransaction iObTransaction, TModel model)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            var sqls = SqlBuilder.Insert(model, ref dbParameters);

            #region 解决Oracle序列返回问题（使用输出参数返回）

            DbParameter outputParameter = null;
            foreach (var dbParameter in dbParameters)
            {
                if (dbParameter.Direction != ParameterDirection.Output) continue;
                outputParameter = dbParameter;
                break;
            }
            if (outputParameter != null)
            {
                DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sqls, ((List<DbParameter>)dbParameters).ToArray());
                return outputParameter.Value;
            }

            #endregion

            return sqls.Length > 1 ?
                DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sqls, ((List<DbParameter>)dbParameters).ToArray()) :
                DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sqls[0], ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion

        #region 添加多个对象到数据库 public object Add(IList<TModel> models)

        public object Add(IList<TModel> models)
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                var sqls = SqlBuilder.Insert(models, ref dbParameters);

                #region 解决Oracle序列返回问题（使用输出参数返回）

                DbParameter outputParameter = null;
                foreach (var dbParameter in dbParameters)
                {
                    if (dbParameter.Direction != ParameterDirection.Output) continue;
                    outputParameter = dbParameter;
                    break;
                }
                if (outputParameter != null)
                {
                    dbHelper.ExecuteNonQuery(sqls, ((List<DbParameter>) dbParameters).ToArray());
                    return outputParameter.Value;
                }

                #endregion

                return dbHelper.ExecuteNonQuery(sqls, ((List<DbParameter>) dbParameters).ToArray());
            }
        }

        #endregion

        #region 添加多个对象到数据库 public object Add(IObTransaction iObTransaction, IList<TModel> models)

        public object Add(IObTransaction iObTransaction, IList<TModel> models)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            var sqls = SqlBuilder.Insert(models, ref dbParameters);

            #region 解决Oracle序列返回问题（使用输出参数返回）

            DbParameter outputParameter = null;
            foreach (var dbParameter in dbParameters)
            {
                if (dbParameter.Direction != ParameterDirection.Output) continue;
                outputParameter = dbParameter;
                break;
            }
            if (outputParameter != null)
            {
                DbHelper.ExecuteScalar(WriteDbHelper, iObTransaction.DbTransaction, sqls,
                    ((List<DbParameter>) dbParameters).ToArray());
                return outputParameter.Value;
            }

            #endregion

            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sqls,
                ((List<DbParameter>) dbParameters).ToArray());
        }

        #endregion


        #region 删除除所有数据 public int Delete()

        public int Delete()
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                string sql = SqlBuilder.Delete(ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>) dbParameters).ToArray());
            }
        }

        #endregion

        #region 删除除所有数据 public int Delete(IObTransaction iObTransaction)

        public int Delete(IObTransaction iObTransaction)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            string sql = SqlBuilder.Delete(ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql,
                ((List<DbParameter>) dbParameters).ToArray());
        }

        #endregion

        #region 从数据库删除一个对象 public int Delete(IObParameter iObParameter)

        /// <summary>
        /// 从数据库删除一个对象
        /// </summary>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Delete(IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(ProviderName, iObParameter);
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                string sql = SqlBuilder.Delete(iObParameter, ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region 从数据库删除一个对象 public int Delete(IObTransaction iObTransaction, IObParameter iObParameter)

        /// <summary>
        /// 从数据库删除一个对象
        /// </summary>
        /// <param name="iObTransaction"></param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Delete(IObTransaction iObTransaction, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(iObTransaction.ProviderName, iObParameter);
            IList<DbParameter> dbParameters = new List<DbParameter>();
            string sql = SqlBuilder.Delete(iObParameter, ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion

        #region 删除除所有数据 public int Delete(IObJoin iObJoin)

        public int Delete(IObJoin iObJoin)
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                ObJoin = iObJoin;
                string sql = SqlBuilder.Delete(ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region 删除除所有数据 public int Delete(IObTransaction iObTransaction, IObJoin iObJoin)

        public int Delete(IObTransaction iObTransaction, IObJoin iObJoin)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            ObJoin = iObJoin;
            string sql = SqlBuilder.Delete(ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql,
                ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion

        #region 从数据库删除一个对象 public int Delete(IObJoin iObJoin, IObParameter iObParameter)

        /// <summary>
        /// 从数据库删除一个对象
        /// </summary>
        /// <param name="iObJoin"></param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Delete(IObJoin iObJoin, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(ProviderName, iObParameter);
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                ObJoin = iObJoin;
                string sql = SqlBuilder.Delete(iObParameter, ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region 从数据库删除一个对象 public int Delete(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter)

        /// <summary>
        /// 从数据库删除一个对象
        /// </summary>
        /// <param name="iObTransaction"></param>
        /// <param name="iObJoin"></param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Delete(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(iObTransaction.ProviderName, iObParameter);
            IList<DbParameter> dbParameters = new List<DbParameter>();
            ObJoin = iObJoin;
            string sql = SqlBuilder.Delete(iObParameter, ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion


        #region 更新所有数据 public int Update(TModel model)

        public int Update(TModel model)
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                string sql = SqlBuilder.Update(model, ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>) dbParameters).ToArray());
            }
        }

        #endregion

        #region 更新所有数据 public int Update(IObTransaction iObTransaction, TModel model)

        public int Update(IObTransaction iObTransaction, TModel model)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            string sql = SqlBuilder.Update(model, ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql,
                ((List<DbParameter>) dbParameters).ToArray());
        }

        #endregion

        #region 更新一个数据库对象 public int Update(M model, IObParameter iObParameter)

        /// <summary>
        /// 更新一个数据库对象
        /// </summary>
        /// <param name="model">对象</param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Update(TModel model, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(ProviderName, iObParameter);
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                string sql = SqlBuilder.Update(model, iObParameter, ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region 更新一个数据库对象 public int Update(IObTransaction iObTransaction, M model, IObParameter iObParameter)

        /// <summary>
        /// 更新一个数据库对象
        /// </summary>
        /// <param name="iObTransaction"></param>
        /// <param name="model">对象</param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Update(IObTransaction iObTransaction, TModel model, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(iObTransaction.ProviderName, iObParameter);
            IList<DbParameter> dbParameters = new List<DbParameter>();
            string sql = SqlBuilder.Update(model, iObParameter, ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion

        #region 更新所有数据 public int Update(TModel model, IObJoin iObJoin)

        public int Update(TModel model, IObJoin iObJoin)
        {
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                ObJoin = iObJoin;
                string sql = SqlBuilder.Update(model, ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region 更新所有数据 public int Update(IObTransaction iObTransaction, TModel model, IObJoin iObJoin)

        public int Update(IObTransaction iObTransaction, TModel model, IObJoin iObJoin)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            ObJoin = iObJoin;
            string sql = SqlBuilder.Update(model, ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql,
                ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion

        #region 更新一个数据库对象 public int Update(M model, IObJoin iObJoin, IObParameter iObParameter)

        /// <summary>
        /// 更新一个数据库对象
        /// </summary>
        /// <param name="model">对象</param>
        /// <param name="iObJoin"></param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Update(TModel model, IObJoin iObJoin, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(ProviderName, iObParameter);
            using (var dbHelper = new DbHelper(WriteDbHelper))
            {
                IList<DbParameter> dbParameters = new List<DbParameter>();
                ObJoin = iObJoin;
                string sql = SqlBuilder.Update(model, iObParameter, ref dbParameters);
                return dbHelper.ExecuteNonQuery(sql, ((List<DbParameter>)dbParameters).ToArray());
            }
        }

        #endregion

        #region 更新一个数据库对象 public int Update(IObTransaction iObTransaction, M model, IObParameter iObParameter)

        /// <summary>
        /// 更新一个数据库对象
        /// </summary>
        /// <param name="iObTransaction"></param>
        /// <param name="model">对象</param>
        /// <param name="iObJoin"></param>
        /// <param name="iObParameter">条件</param>
        /// <returns></returns>
        public int Update(IObTransaction iObTransaction, TModel model, IObJoin iObJoin, IObParameter iObParameter)
        {
            iObParameter = ObConvert.ToObParameter(iObTransaction.ProviderName, iObParameter);
            IList<DbParameter> dbParameters = new List<DbParameter>();
            ObJoin = iObJoin;
            string sql = SqlBuilder.Update(model, iObParameter, ref dbParameters);
            return DbHelper.ExecuteNonQuery(WriteDbHelper, iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray());
        }

        #endregion


        #region 获取查询接口 public IObQuery<M> Query()

        public IObQuery<TModel> Query()
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, null, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin)

        public IObQuery<TModel> Query(IObJoin iObJoin)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, null, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObParameter iObParameter)

        public IObQuery<TModel> Query(IObParameter iObParameter)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, iObParameter, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObParameter iObParameter)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, iObParameter, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObSort iObSort)

        public IObQuery<TModel> Query(IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, null, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObSort iObSort)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, null, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObParameter iObParameter, IObSort iObSort)

        public IObQuery<TModel> Query(IObParameter iObParameter, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, iObParameter, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObParameter iObParameter, IObSort iObSort)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, iObParameter, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction)

        public IObQuery<TModel> Query(IObTransaction iObTransaction)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, null, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, null, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObParameter iObParameter)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, iObParameter, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, iObParameter, null, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, null, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, null, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, iObParameter, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, iObParameter, null, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObGroup iObGroup)

        public IObQuery<TModel> Query(IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, null, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObGroup iObGroup)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, null, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObParameter iObParameter, IObGroup iObGroup)

        public IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, iObParameter, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, iObParameter, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, null, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, null, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, iObParameter, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, iObParameter, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObGroup iObGroup)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, null, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, null, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, iObParameter, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, iObParameter, iObGroup, null, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, null, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, null, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, iObParameter, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, iObParameter, iObGroup, null, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, null, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, null, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, iObParameter, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, iObParameter, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, null, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, null, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, null, iObParameter, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, null, iObJoin, iObParameter, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, null, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, null, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, iObParameter, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, iObParameter, iObGroup, iObParameter2, null);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, null, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, null, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, null, iObParameter, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        #region 获取查询接口 public IObQuery<M> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)

        public IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort)
        {
            return new ObQuery<TModel>(ReadDbHelper, SqlBuilder, ProviderName, iObTransaction, iObJoin, iObParameter, iObGroup, iObParameter2, iObSort);
        }

        #endregion

        public IObSql<TModel> SqlText(string commandText, params DbParameter[] commandParameters)
        {
            return new ObSql<TModel>(WriteDbHelper, null, CommandType.Text, commandText, commandParameters);
        }

        public IObSql<TModel> SqlText(IObTransaction iObTransaction, string commandText, params DbParameter[] commandParameters)
        {
            return new ObSql<TModel>(WriteDbHelper, iObTransaction, CommandType.Text, commandText, commandParameters);
        }

        public IObSql<TModel> SqlStoredProcedure(string commandText, params DbParameter[] commandParameters)
        {
            return new ObSql<TModel>(WriteDbHelper, null, CommandType.StoredProcedure, commandText, commandParameters);
        }

        public IObSql<TModel> SqlStoredProcedure(IObTransaction iObTransaction, string commandText, params DbParameter[] commandParameters)
        {
            return new ObSql<TModel>(WriteDbHelper, iObTransaction, CommandType.StoredProcedure, commandText, commandParameters);
        }

        //#region 创建当前数据库的参数

        //private static IObParameter ObConvert.ToObParameter(string providerName, IObParameter iObParameter)
        //{
        //    #region 循环创建 注释

        //    /*IObParameter p = null;
        //    string className = providerName + ".ObParameter";
        //    Type t = Assembly.Load(providerName).GetType(className);
        //    if(iObParameter.Value == null)
        //    {

        //        p = (IObParameter)Activator.CreateInstance(t, iObParameter);
        //    }
        //    else if(iObParameter.Value is DbTerm)
        //    {
        //        var dbTerm = (DbTerm) iObParameter.Value;
        //        var parameters = new object[]
        //                         {
        //                             dbTerm.TableName,
        //                             dbTerm.ColumnName,
        //                             dbTerm.DbSymbol,
        //                             dbTerm.Value
        //                         };
        //        p = (IObParameter)Activator.CreateInstance(t, parameters);
        //    }
        //    else if(iObParameter.Value is DbNTerm)
        //    {
        //        var dbNTerm = (DbNTerm)iObParameter.Value;
        //        var parameters = new object[]
        //                         {
        //                             dbNTerm.TableName,
        //                             dbNTerm.ColumnName,
        //                             dbNTerm.DbValue
        //                         };
        //        p = (IObParameter)Activator.CreateInstance(t, parameters);
        //    }
        //    foreach (var brother in iObParameter.Brothers)
        //    {
        //        if(brother.BrotherType == 1)
        //        {
        //            p.And(ObConvert.ToObParameter(providerName, brother));
        //        }
        //        else if(brother.BrotherType == 2)
        //        {
        //            p.Or(ObConvert.ToObParameter(providerName, brother));
        //        }
        //    }*/

        //    #endregion

        //    string className = providerName + ".ObParameter";
        //    Type t = Assembly.Load(providerName).GetType(className);
        //    var parameters = new []
        //                         {
        //                             iObParameter.BrotherType,
        //                             iObParameter.Brothers,
        //                             iObParameter.Value
        //                         };
        //    return (IObParameter)Activator.CreateInstance(t, parameters);
        //}

        //#endregion

        //#region 创建当前数据库的排序

        //public static IObSort ObConvert.ToObSort(string providerName, IObSort iObSort)
        //{
        //    string className = providerName + ".ObSort";
        //    Type t = Assembly.Load(providerName).GetType(className);
        //    return (IObSort)Activator.CreateInstance(t, iObSort.List);
        //}

        //#endregion

        //#region 创建当前数据库的分组

        //public static IObGroup ObConvert.ToObGroup(string providerName, IObGroup iObGroup)
        //{
        //    string className = providerName + ".ObGroup";
        //    Type t = Assembly.Load(providerName).GetType(className);
        //    return (IObGroup)Activator.CreateInstance(t, new object[] { iObGroup.DbGroups, iObGroup.ObProperties });
        //}

        //#endregion

        //#region 创建当前数据库函数列

        //public static IObProperty ObConvert.ToObProperty(string providerName, IObProperty iObProperty)
        //{
        //    string className = providerName + ".ObProperty";
        //    Type t = Assembly.Load(providerName).GetType(className);
        //    return (IObProperty)Activator.CreateInstance(t, new object[] { iObProperty.ParentType, iObProperty.ColumnName, iObProperty.DbFun });
        //}

        //#endregion

        public int Delete(Expression<Func<TModel, bool>> keySelector)
        {
            var queryable = new ObQueryable<TModel>(WriteDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.Where(keySelector);
            return Delete(queryable.ObParameter);
        }

        public int Update(TModel model, Expression<Func<TModel, bool>> keySelector)
        {
            var queryable = new ObQueryable<TModel>(WriteDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.Where(keySelector);
            return Update(model, queryable.ObParameter);
        }

        public virtual IObQueryable<TModel> Queryable()
        {
            var queryable = new ObQueryable<TModel>(ReadDbHelper, SqlBuilder, ProviderName, ObJoin);
            return queryable;
        }

        public IObQueryable<TModel> Where(Expression<Func<TModel, bool>> keySelector)
        {
            var queryable = new ObQueryable<TModel>(ReadDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.Where(keySelector);
            return queryable;
        }

        public IObQueryable<TModel> OrderBy<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            var queryable = new ObQueryable<TModel>(ReadDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.OrderBy(keySelector);
            return queryable;
        }

        public IObQueryable<TModel> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            var queryable = new ObQueryable<TModel>(ReadDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.OrderByDescending(keySelector);
            return queryable;
        }

        public IObQueryable<TModel> GroupBy<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            var queryable = new ObQueryable<TModel>(ReadDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.GroupBy(keySelector);
            return queryable;
        }

        public IObQueryable<TModel> DistinctBy<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            var queryable = new ObQueryable<TModel>(ReadDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.DistinctBy(keySelector);
            return queryable;
        }

        public IObHelper<TModel> Join()
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            return this;
        }

        public IObHelper<TModel> Join<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            var queryable = new ObQueryable<TModel>(ReadDbHelper, SqlBuilder, ProviderName, ObJoin);
            queryable.Join(keySelector);
            foreach (var joinModel in queryable.ObJoin.JoinModels)
            {
                ObJoin.JoinModels.Add(joinModel);
            }
            return this;
        }

    }

    public class ObHelper<TModel, TTerm> : ObHelper<TModel>, IObHelper<TModel, TTerm>
        where TModel : ObModelBase, new()
        where TTerm : ObTermBase, new()
    {
        public TTerm Term { get; }

        public ObHelper(string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels) : base(connectionString, providerName, iObRedefine, notJoinModels)
        {
            Term = new TTerm();
        }

        public ObHelper(string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels) : base(readConnectionString, writeConnectionString, providerName, iObRedefine, notJoinModels)
        {
            Term = new TTerm();
        }

        public ObHelper(TTerm term, string connectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels) : base(connectionString, providerName, iObRedefine, notJoinModels)
        {
            Term = term;
        }

        public ObHelper(TTerm term, string readConnectionString, string writeConnectionString, string providerName, IObRedefine iObRedefine, IList<string> notJoinModels) : base(readConnectionString, writeConnectionString, providerName, iObRedefine, notJoinModels)
        {
            Term = term;
        }

        #region 从数据库删除一个对象 public int Delete(Func<TTerm, IObParameter> keySelector)

        /// <summary>
        /// 从数据库删除一个对象
        /// </summary>
        /// <param name="keySelector">条件</param>
        /// <returns></returns>
        public int Delete(Func<TTerm, IObParameter> keySelector)
        {
            /*var iObParameter = keySelector(Term);
            return Delete(iObParameter);*/
            var queryable = new ObQueryable<TModel, TTerm>(WriteDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.Where(keySelector);
            return Delete(queryable.ObParameter);
        }

        #endregion

        #region 更新一个数据库对象 public int Update(M model, Func<TTerm, IObParameter> keySelector)

        /// <summary>
        /// 更新一个数据库对象
        /// </summary>
        /// <param name="model">对象</param>
        /// <param name="keySelector">条件</param>
        /// <returns></returns>
        public int Update(TModel model, Func<TTerm, IObParameter> keySelector)
        {
            /*var iObParameter = keySelector(Term);
            return Update(model, iObParameter);*/
            var queryable = new ObQueryable<TModel, TTerm>(WriteDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.Where(keySelector);
            return Update(model, queryable.ObParameter);
        }

        #endregion

        public override IObQueryable<TModel> Queryable()
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            return queryable;
        }

        public IObQueryable<TModel, TTerm> Where(Func<TTerm, IObParameter> keySelector)
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.Where(keySelector);
            return queryable;
        }

        public IObQueryable<TModel, TTerm> GroupBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.GroupBy(keySelector);
            return queryable;
        }

        public IObQueryable<TModel, TTerm> GroupBy(Func<TTerm, ObProperty> keySelector)
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.GroupBy(keySelector);
            return queryable;
        }

        public IObQueryable<TModel, TTerm> DistinctBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            return GroupBy(keySelector);
        }

        public IObQueryable<TModel, TTerm> DistinctBy(Func<TTerm, ObProperty> keySelector)
        {
            return GroupBy(keySelector);
        }

        public IObQueryable<TModel, TTerm> OrderBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.OrderBy(keySelector);
            return queryable;
        }

        public IObQueryable<TModel, TTerm> OrderBy(Func<TTerm, ObProperty> keySelector)
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.OrderBy(keySelector);
            return queryable;
        }

        public IObQueryable<TModel, TTerm> OrderByDescending<TKey>(Func<TTerm, TKey> keySelector)
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.OrderByDescending(keySelector);
            return queryable;
        }

        public IObQueryable<TModel, TTerm> OrderByDescending(Func<TTerm, ObProperty> keySelector)
        {
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.OrderByDescending(keySelector);
            return queryable;
        }

        public IObHelper<TModel, TTerm> Join<TKey>(Func<TTerm, TKey> keySelector)
        {
            /*var iObJoin = new ObJoin();
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
            iObJoin.AddJoin(list.ToArray());
            ObJoin = iObJoin;*/
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.Join(keySelector);
            foreach (var joinModel in queryable.ObJoin.JoinModels)
            {
                ObJoin.JoinModels.Add(joinModel);
            }
            return this;
        }

        public IObHelper<TModel, TTerm> Join(Func<TTerm, ObTermBase> keySelector)
        {
            /*var iObJoin = new ObJoin();
            iObJoin.AddJoin(keySelector(Term));
            ObJoin = iObJoin;*/
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            var queryable = new ObQueryable<TModel, TTerm>(ReadDbHelper, SqlBuilder, ProviderName, Term, ObJoin);
            queryable.Join(keySelector);
            foreach (var joinModel in queryable.ObJoin.JoinModels)
            {
                ObJoin.JoinModels.Add(joinModel);
            }
            return this;
        }
    }
}