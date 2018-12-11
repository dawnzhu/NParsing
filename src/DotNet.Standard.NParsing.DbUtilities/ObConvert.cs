/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-09-27 10:47:56
* 版 本 号：2.2.0
* 功能说明：SQL生成参数转换类
* ----------------------------------
* 修改标识：
* 修 改 人：
* 日    期：
* 版 本 号：
* 修改内容：
*/
using System;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public static class ObConvert
    {
        /// <summary>
        /// 转换成当前数据库参数
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="iObParameter"></param>
        /// <returns></returns>
        public static IObParameter ToObParameter(string providerName, IObParameter iObParameter)
        {
            string className = providerName + ".ObParameter";
            Type t = Assembly.Load(providerName).GetType(className);
            var parameters = new[] { iObParameter.BrotherType, iObParameter.Brothers, iObParameter.Value };
            return (IObParameter)Activator.CreateInstance(t, parameters);
        }

        /// <summary>
        /// 转换成当前数据排序
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="iObSort"></param>
        /// <returns></returns>
        public static IObSort ToObSort(string providerName, IObSort iObSort)
        {
            string className = providerName + ".ObSort";
            Type t = Assembly.Load(providerName).GetType(className);
            return (IObSort)Activator.CreateInstance(t, iObSort.List);
        }

        /// <summary>
        /// 转换成当前数据分组
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="iObGroup"></param>
        /// <returns></returns>
        public static IObGroup ToObGroup(string providerName, IObGroup iObGroup)
        {
            string className = providerName + ".ObGroup";
            Type t = Assembly.Load(providerName).GetType(className);
            return (IObGroup)Activator.CreateInstance(t, iObGroup.DbGroups, iObGroup.ObGroupProperties, iObGroup.ObProperties);
        }

        /// <summary>
        /// 转换成当前数据属性
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="iObProperty"></param>
        /// <returns></returns>
        public static IObProperty ToObProperty(string providerName, IObProperty iObProperty)
        {
            string className = providerName + ".ObProperty";
            Type t = Assembly.Load(providerName).GetType(className);
            //return (IObProperty)Activator.CreateInstance(t, new object[] { iObProperty.ModelType, iObProperty.TableName, iObProperty.ColumnName, iObProperty.Brothers, iObProperty.AriSymbol, iObProperty.DbFunc });
            return (IObProperty)Activator.CreateInstance(t, iObProperty);
        }
    }
}
