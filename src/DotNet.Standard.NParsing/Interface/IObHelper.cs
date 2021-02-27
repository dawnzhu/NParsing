/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-25 16:30:28
* 版 本 号：1.0.0
* 功能说明：对象模型/关系模型互转接口(数据库操作接口)
* ----------------------------------
* 修改标识：增加功能接口
* 修 改 人：朱晓春
* 日    期：2009-08-28 14:25:20
* 版 本 号：1.0.0
* 修改内容：增加了获取记录数的接口
* ----------------------------------
* 修改标识：增加功能接口
* 修 改 人：朱晓春
* 日    期：2009-09-04 16:55:20
* 版 本 号：1.0.0
* 修改内容：增加了获取最大值的接口 object GetMaxScalar
* ----------------------------------
* 修改标识：增加功能接口
* 修 改 人：朱晓春
* 日    期：2010-03-09 12:29:00
* 版 本 号：1.0.0
* 修改内容：增加了检查是否存在的接口 bool Exists
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-11 22:21:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Frameworks.Hibernate.Interface->DotNet.Frameworks.Transport.Interface)和接口名(IObjectToRelational->IObHelper)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 21:45:00
* 版 本 号：1.0.1
* 修改内容：增加了获取最小值的接口 object GetMinScalar
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-16 22:24:00
* 版 本 号：1.0.1
* 修改内容：增加了IList<M> GetList排序功能
 * ----------------------------------
* 修改标识：增加功能接口
* 修 改 人：朱晓春
* 日    期：2010-03-09 12:29:00
* 版 本 号：1.0.0
* 修改内容：增加了分组的接口
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
* 日    期：2010-08-06 17:15:00
* 版 本 号：2.1.0
* 修改内容：增加分组分页功能
 * IList<M> GetList(int pageSize, int pageIndex, IObParameter iObParameter, IObSort iObSort, out int count);
 * IList<M> GetList(int pageSize, int pageIndex, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort, out int count);
* ----------------------------------
* 修改标识：删除过期接口
* 修 改 人：朱晓春
* 日    期：2011-11-08 10:35:00
* 版 本 号：2.2.0
* 修改内容：所有过期查询接口取消，查询统一使用Query
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using DotNet.Standard.NParsing.Factory;

namespace DotNet.Standard.NParsing.Interface
{
    /// <summary>
    /// 操作库
    /// </summary>
    public interface IObHelper
    {
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Create(string name);
        bool Create(IObTransaction iObTransaction, string name);

        /// <summary>
        /// 删除数据库
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Drop(string name);
        bool Drop(IObTransaction iObTransaction, string name);

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        int NonQuery(string sqlText, params DbParameter[] commandParameters);
        int NonQuery(IObTransaction iObTransaction, string sqlText, params DbParameter[] commandParameters);
        int NonQueryByStoredProcedure(string spName, params DbParameter[] commandParameters);
        int NonQueryByStoredProcedure(IObTransaction iObTransaction, string spName, params DbParameter[] commandParameters);

        /// <summary>
        /// 获取一个DataSet
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        DataSet GetDataset(string sqlText, params DbParameter[] commandParameters);
        DataSet GetDataset(IObTransaction iObTransaction, string sqlText, params DbParameter[] commandParameters);
        DataSet GetDatasetByStoredProcedure(string spName, params DbParameter[] commandParameters);
        DataSet GetDatasetByStoredProcedure(IObTransaction iObTransaction, string spName, params DbParameter[] commandParameters);

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        object GetScalar(string sqlText, params DbParameter[] commandParameters);
        object GetScalar(IObTransaction iObTransaction, string sqlText, params DbParameter[] commandParameters);
        object GetScalarByStoredProcedure(string spName, params DbParameter[] commandParameters);
        object GetScalarByStoredProcedure(IObTransaction iObTransaction, string spName, params DbParameter[] commandParameters);
    }

    /// <summary>
    /// 操作表
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IObHelper<TModel>
    {
        /// <summary>
        /// 关联
        /// </summary>
        IObJoin ObJoin { get; }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <returns></returns>
        bool Create();
        bool Create(IObTransaction iObTransaction);
        bool Create(string name);
        bool Create(IObTransaction iObTransaction, string name);

        /// <summary>
        /// 删除数据表
        /// </summary>
        /// <returns></returns>
        bool Drop();
        bool Drop(IObTransaction iObTransaction);

        /// <summary>
        /// 创建表索引
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iObSort"></param>
        /// <param name="fileGroup"></param>
        /// <returns></returns>
        bool CreateIndex(string name, IObSort iObSort, string fileGroup = null);
        bool CreateIndex(IObTransaction iObTransaction, string name, IObSort iObSort, string fileGroup = null);

        /// <summary>
        /// 删除表索引
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool DropIndex(string name);
        bool DropIndex(IObTransaction iObTransaction, string name);

        /// <summary>
        /// 添加一个对象到数据库
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// 如果有标识列，返回标识列的值
        /// 没有，则返回影响行数
        /// </returns>
        object Add(TModel model);
        object Add(IObTransaction iObTransaction, TModel model);
        object Add(IList<TModel> models);
        object Add(IObTransaction iObTransaction, IList<TModel> models);

        /// <summary>
        /// 从数据库删除一个对象
        /// </summary>
        /// <returns>
        /// 影响行数
        /// </returns>
        int Delete();
        int Delete(IObTransaction iObTransaction);
        int Delete(IObParameter iObParameter);
        int Delete(IObTransaction iObTransaction, IObParameter iObParameter);
        int Delete(IObJoin iObJoin);
        int Delete(IObTransaction iObTransaction, IObJoin iObJoin);
        int Delete(IObJoin iObJoin, IObParameter iObParameter);
        int Delete(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter);

        /// <summary>
        /// 更新一个数据库对象
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// 影响行数
        /// </returns>
        int Update(TModel model);
        int Update(IObTransaction iObTransaction, TModel model);
        int Update(TModel model, IObParameter iObParameter);
        int Update(IObTransaction iObTransaction, TModel model, IObParameter iObParameter);
        int Update(TModel model, IObJoin iObJoin);
        int Update(IObTransaction iObTransaction, TModel model, IObJoin iObJoin);
        int Update(TModel model, IObJoin iObJoin, IObParameter iObParameter);
        int Update(IObTransaction iObTransaction, TModel model, IObJoin iObJoin, IObParameter iObParameter);

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        IObQuery<TModel> Query();
        IObQuery<TModel> Query(IObJoin iObJoin);
        IObQuery<TModel> Query(IObParameter iObParameter);
        IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter);
        IObQuery<TModel> Query(IObSort iObSort);
        IObQuery<TModel> Query(IObJoin iObJoin, IObSort iObSort);
        IObQuery<TModel> Query(IObParameter iObParameter, IObSort iObSort);
        IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObSort iObSort);

        /// <summary>
        /// 查询分组
        /// </summary>
        /// <param name="iObGroup"></param>
        /// <returns></returns>
        IObQuery<TModel> Query(IObGroup iObGroup);
        IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup);
        IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup);
        IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup);
        IObQuery<TModel> Query(IObGroup iObGroup, IObSort iObSort);
        IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup, IObSort iObSort);
        IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort);
        IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObSort iObSort);

        /// <summary>
        /// 查询分组
        /// </summary>
        /// <param name="iObGroup"></param>
        /// <param name="iObParameter2"></param>
        /// <returns></returns>
        IObQuery<TModel> Query(IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);
        IObQuery<TModel> Query(IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);
        IObQuery<TModel> Query(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);
        IObQuery<TModel> Query(IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);
        IObQuery<TModel> Query(IObTransaction iObTransaction, IObJoin iObJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort);

        IObSql<TModel> SqlText(string commandText, params DbParameter[] commandParameters);
        IObSql<TModel> SqlText(IObTransaction iObTransaction, string commandText, params DbParameter[] commandParameters);

        IObSql<TModel> SqlStoredProcedure(string commandText, params DbParameter[] commandParameters);
        IObSql<TModel> SqlStoredProcedure(IObTransaction iObTransaction, string commandText, params DbParameter[] commandParameters);

        IObQueryable<TModel> Queryable();
        int Delete(Expression<Func<TModel, bool>> keySelector);
        int Update(TModel model, Expression<Func<TModel, bool>> keySelector);

        IObQueryable<TModel> Where(Expression<Func<TModel, bool>> keySelector);
        IObQueryable<TModel> OrderBy<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObQueryable<TModel> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObQueryable<TModel> GroupBy<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObQueryable<TModel> DistinctBy<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObHelper<TModel> Join();
        IObHelper<TModel> Join<TKey>(Expression<Func<TModel, TKey>> keySelector);
    }

    public interface IObHelper<TModel, TTerm> : IObHelper<TModel>
        where TModel : ObModelBase
        where TTerm : ObTermBase
    {
        TTerm Term { get; }
        //new IObQueryable<TModel> Queryable();
        int Delete(Func<TTerm, IObParameter> keySelector);
        int Update(TModel model, Func<TTerm, IObParameter> keySelector);
        IObQueryable<TModel, TTerm> Where(Func<TTerm, IObParameter> keySelector);
        IObQueryable<TModel, TTerm> GroupBy<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> GroupBy(Func<TTerm, ObProperty> keySelector);
        IObQueryable<TModel, TTerm> DistinctBy<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> DistinctBy(Func<TTerm, ObProperty> keySelector);
        IObQueryable<TModel, TTerm> OrderBy<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> OrderBy(Func<TTerm, ObProperty> keySelector);
        IObQueryable<TModel, TTerm> OrderByDescending<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> OrderByDescending(Func<TTerm, ObProperty> keySelector);
        IObHelper<TModel, TTerm> Join<TKey>(Func<TTerm, TKey> keySelector);
        IObHelper<TModel, TTerm> Join(Func<TTerm, ObTermBase> keySelector);
    }
}