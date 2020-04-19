/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-03-29 13:33:00
* 版 本 号：1.0.0
* 功能说明：创建排序接口(数据库ORDER BY)
* ----------------------------------
 */

using System;
using System.Collections.Generic;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObSort
    {
        /// <summary>
        /// 排序字段列表
        /// </summary>
        IList<DbSort> List { get; }

        /// <summary>
        /// 添加一个排序参数
        /// </summary>
        /// <param name="obProperty">属性</param>
        /// <returns></returns>
        IObSort AddOrderBy(ObProperty obProperty);
        IObSort AddOrderByDescending(ObProperty obProperty);

        /// <summary>
        /// 添加多个排序参数
        /// </summary>
        /// <param name="obPropertys"></param>
        /// <returns></returns>
        IObSort AddOrderBy(params ObProperty[] obPropertys);
        IObSort AddOrderByDescending(params ObProperty[] obPropertys);

        IObSort Add(IObSort obSort);
        /// <summary>
        /// 添加多个排序参数
        /// </summary>
        /// <param name="obSorts"></param>
        /// <returns></returns>
        IObSort Add(params IObSort[] obSorts);

        /// <summary>
        /// 获取排序字符串
        /// </summary>
        /// <returns></returns>
        string ToString();

        string ToString(char separator);

        string ToString(IList<string> columnNames);

        string ToString(char separator, IList<string> columnNames);

        /// <summary>
        /// 标识
        /// </summary>
        string Key { get; }
    }

    public interface IObSort<out TTerm> : IObSort
        where TTerm : ObTermBase
    {
        /// <summary>
        /// 添加一个排序参数
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IObSort<TTerm> AddOrderBy(Func<TTerm, ObProperty> keySelector);
        IObSort<TTerm> AddOrderByDescending(Func<TTerm, ObProperty> keySelector);

        /// <summary>
        /// 添加多个排序参数
        /// </summary>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IObSort<TTerm> AddOrderBy(Func<TTerm, ObProperty[]> keySelector);
        IObSort<TTerm> AddOrderByDescending(Func<TTerm, ObProperty[]> keySelector);

        /// <summary>
        /// 添加多个排序参数
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        IObSort<TTerm> AddOrderBy<TKey>(Func<TTerm, TKey> keySelector);
        IObSort<TTerm> AddOrderByDescending<TKey>(Func<TTerm, TKey> keySelector);
    }
}