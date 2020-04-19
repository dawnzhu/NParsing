using System;
using System.Collections.Generic;
using System.Data.Common;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObGroup
    {
        /// <summary>
        /// 显示属性列表
        /// </summary>
        IList<IObProperty> ObProperties { get; }

        /// <summary>
        /// 分组字段列表
        /// </summary>
        IList<DbGroup> DbGroups { get; }

        /// <summary>
        /// 分组字段列表
        /// </summary>
        IList<IObProperty> ObGroupProperties { get; }

        /// <summary>
        /// 添加一个分组参数
        /// </summary>
        /// <param name="obProperty">属性</param>
        /// <returns></returns>
        IObGroup AddGroupBy(ObProperty obProperty);

        /// <summary>
        /// 添加多个分组参数
        /// </summary>
        /// <param name="obPropertys"></param>
        /// <returns></returns>
        IObGroup AddGroupBy(params ObProperty[] obPropertys);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbParameter"></param>
        /// <param name="columns"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        string ToString(ref IList<DbParameter> dbParameter, out string columns, out IList<string> columnNames);

        /// <summary>
        /// 标识
        /// </summary>
        string Key { get; }
    }

    public interface IObGroup<out TTerm> : IObGroup
        where TTerm : ObTermBase
    {
        /// <summary>
        /// 添加一个分组参数
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IObGroup<TTerm> AddGroupBy(Func<TTerm, ObProperty> keySelector);

        /// <summary>
        /// 添加多个分组参数
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IObGroup<TTerm> AddGroupBy(Func<TTerm, ObProperty[]> keySelector);

        IObGroup<TTerm> AddGroupBy<TKey>(Func<TTerm, TKey> keySelector);
    }
}
