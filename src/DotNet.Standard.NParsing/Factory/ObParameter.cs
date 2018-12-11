/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-25 16:35:12
* 版 本 号：1.0.0
* 功能说明：实现接口创建的工厂类
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 09:30:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Frameworks.Hibernate.Factory->DotNet.Frameworks.Transport.Factory)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-29 14:30:00
* 版 本 号：1.0.1
* 修改内容：增加排序接口创建
* ----------------------------------
* 修改标识：创建
* 修 改 人：朱晓春
* 日    期：2010-03-29 16:00:00
* 版 本 号：1.0.1
* 修改内容：从DataAccess中分离
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-04-07 15:48:00
* 版 本 号：1.0.2
* 修改内容：增加创建子参数功能 public static IObParameter Create(IObParameter iObParameter)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2011-12-02 17:51:00
* 版 本 号：2.3.0
* 修改内容：代码整理
*/
using System;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Factory
{
    public static class ObParameter
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObParameter";

        /// <summary>
        /// 创建永远成立的条件
        /// </summary>
        /// <returns></returns>
        public static ObParameterBase Create()
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     "1",
                                     "0",
                                     DbSymbol.Equal,
                                     "1.0"
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        /// <summary>
        /// 创建子条件
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <returns></returns>
        public static ObParameterBase Create(IObParameter iObParameter)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            return (ObParameterBase)Activator.CreateInstance(t, iObParameter);
        }

        /// <summary>
        /// 创建特殊条件(IS NULL、IS NOT NULL)
        /// </summary>
        /// <param name="obProperty">属性</param>
        /// <param name="dbValue">枚举值,IS NULL、IS NOT NULL</param>
        /// <returns></returns>
        public static ObParameterBase Create(ObProperty obProperty, DbValue dbValue)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     dbValue
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        /// <summary>
        /// 创建条件
        /// </summary>
        /// <param name="obProperty">属性</param>
        /// <param name="dbSymbol">条件符号</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ObParameterBase Create(ObProperty obProperty, DbSymbol dbSymbol, object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     obProperty,
                                     dbSymbol,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        /// <summary>
        /// 创建条件
        /// </summary>
        /// <param name="obProperty">属性</param>
        /// <param name="dbSymbol">条件符号</param>
        /// <param name="obProperty2">属性2</param>
        /// <returns></returns>
        public static ObParameterBase Create(ObProperty obProperty, DbSymbol dbSymbol, ObProperty obProperty2)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     dbSymbol,
                                     obProperty2
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }
    }
}