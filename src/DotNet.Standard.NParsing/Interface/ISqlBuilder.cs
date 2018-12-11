using System.Collections.Generic;
using System.Data.Common;

namespace DotNet.Standard.NParsing.Interface
{
    /// <summary>
    /// 成生库语句
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// 成生创建库语句
        /// </summary>
        /// <returns></returns>
        string CreateDatabase(string name);

        /// <summary>
        /// 成生删除库语句
        /// </summary>
        /// <returns></returns>
        string DropDatabase(string name);
    }
    
    /// <summary>
    /// 生成表语句
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface ISqlBuilder<TModel>
    {
        /// <summary>
        /// 表名重定义
        /// </summary>
        IObRedefine ObRedefine { get; }

        /// <summary>
        /// 允许关联
        /// </summary>
        IList<string> JoinModels { get; set; }

        /// <summary>
        /// 成生创建表语句
        /// </summary>
        /// <returns></returns>
        string CreateTable();
        string CreateTable(string name);

        /// <summary>
        /// 成生删除表语句
        /// </summary>
        /// <returns></returns>
        string DropTable();

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iObSort"></param>
        /// <param name="fileGroup"></param>
        /// <returns></returns>
        string CreateIndex(string name, IObSort iObSort, string fileGroup);

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string DropIndex(string name);

        /// <summary>
        /// 生成INSERT语句
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        string[] Insert(TModel model, ref IList<DbParameter> dbParameters);
        string[] Insert(IList<TModel> models, ref IList<DbParameter> dbParameters);

        /// <summary>
        /// 生成DELETE语句
        /// </summary>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        string Delete(ref IList<DbParameter> dbParameters);
        string Delete(IObParameter iObParameter, ref IList<DbParameter> dbParameters);

        /// <summary>
        /// 生成UPDATE语句
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        string Update(TModel model, ref IList<DbParameter> dbParameters);
        string Update(TModel model, IObParameter iObParameter, ref IList<DbParameter> dbParameters);

        /// <summary>
        /// 生成SELECT语句
        /// </summary>
        /// <param name="iObParameter"></param>
        /// <param name="iObGroup"></param>
        /// <param name="iObParameter2"></param>
        /// <param name="dbParameters"></param>
        /// <returns></returns>
        string CountSelect(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, ref IList<DbParameter> dbParameters);
        string ExistsSelect(IObParameter iObParameter, ref IList<DbParameter> dbParameters);
        string Select(int? topSize, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters, out IList<string> columnNames);
        string Select(IObProperty iObProperty, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters);
        string Select(int pageSize, int pageIndex, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters, out IList<string> columnNames);

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="topSize"></param>
        /// <param name="dbParameters"></param>
        void TopParameters(int topSize, ref IList<DbParameter> dbParameters);
        void PageParameters(int pageSize, int pageIndex, ref IList<DbParameter> dbParameters);
    }
}