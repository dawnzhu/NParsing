/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-09-27 12:47:56
* 版 本 号：2.2.0
* 功能说明：数据库查询接口
* ----------------------------------
* 修改标识：
* 修 改 人：
* 日    期：
* 版 本 号：
* 修改内容：
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObQuery<TModel>
    {
        /// <summary>
        /// 过虑条件参数
        /// </summary>
        IObParameter ObParameter { get; }

        /// <summary>
        /// 分组后过虑条件参数
        /// </summary>
        IObParameter ObGroupParameter { get; }

        /// <summary>
        /// 分组
        /// </summary>
        IObGroup ObGroup { get; }

        /// <summary>
        /// 排序
        /// </summary>
        IObSort ObSort { get; }

        /// <summary>
        /// 关联
        /// </summary>
        IObJoin ObJoin { get; }

        /// <summary>
        /// 获得一个对像
        /// </summary>
        /// <returns></returns>
        TModel ToModel();

        /// <summary>
        /// 获得一个对象列表
        /// </summary>
        /// <returns></returns>
        IList<TModel> ToList();

        IList<TModel> ToList(int topSize);

        /// <summary>
        /// 获得一个对象列表分页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IList<TModel> ToList(int pageSize, int pageIndex, out int count);

        /// <summary>
        /// 获得一个数据表
        /// </summary>
        /// <returns></returns>
        DataTable ToTable();

        DataTable ToTable(int topSize);

        /// <summary>
        /// 获得一个数据表分页
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        DataTable ToTable(int pageSize, int pageIndex, out int count);

        /// <summary>
        /// 获得一个值
        /// </summary>
        /// <param name="iObProperty"></param>
        /// <returns></returns>
        object Scalar(IObProperty iObProperty);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>
        /// 获得记录数
        /// </summary>
        /// <returns></returns>
        int Count();

        int PageSize { get; }

        int MaxDegreeOfParallelism { get; }

        (string Key, string Text, IList<DbParameter> Parameters) CurrentExeSql { get; }

        /// <summary>
        /// 通过分页并行获取数据
        /// </summary>
        /// <returns></returns>
        IObQuery<TModel> Parallel();

        IObQuery<TModel> Parallel(int pageSize);

        /// <summary>
        /// 通过分页并行获取数据
        /// </summary>
        /// <returns></returns>
        IObQuery<TModel> Parallel(int pageSize, int maxDegreeOfParallelism);
    }
}