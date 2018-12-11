/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-26 09:58:57
* 版 本 号：1.0.0
* 功能说明：SQL WHERE 条件成生接口
* ----------------------------------
* 修改标识：修改+增加功能
* 修 改 人：朱晓春
* 日    期：2009-09-01 15:01:00
* 版 本 号：1.0.0
* 修改内容：修改string SqlWhere {get;}属性string GetSqlWhere();为方法,并增加string GetSqlWhere(ref List<DbParameter> dbParameters);
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-11 22:01:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Frameworks.Hibernate.Interface->DotNet.Frameworks.Transport.Interface)和接口名(IO2rParameter->IObParameter)
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-30 15:01:00
* 版 本 号：1.0.1
* 修改内容：修改string GetSqlWhere() string GetSqlWhere(ref List<DbParameter> dbParameters)方法名string ToString(); string ToString(ref List<DbParameter> dbParameters);
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-31 20:48:00
* 版 本 号：1.0.1
* 修改内容：去掉string ToString();非参数化方法;
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-04-07 15:48:00
* 版 本 号：1.0.2
* 修改内容：将TableName, ColumnName, DbSymbol, DbValue, Value 集成至 Value(DbNTerm或DbTerm对象)
*/
using System.Collections.Generic;
using System.Data.Common;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObParameter
    {
        /// <summary>
        /// 值 null, DbTerm, DbNTerm
        /// </summary>
        object Value { get; }

        /// <summary>
        /// 平级兄弟列表
        /// </summary>
        IList<IObParameter> Brothers { get; }

        /// <summary>
        /// 0 无兄弟 1 AND 2 OR
        /// </summary>
        int BrotherType { get; }

        /// <summary>
        /// SQL条件语句
        /// </summary>
        string ToString(ref IList<DbParameter> dbParameters);

        /// <summary>
        /// SQL条件语句
        /// </summary>
        string ToString(char separator, ref IList<DbParameter> dbParameters);

        /// <summary>
        /// 平级AND条件
        /// </summary>
        /// <param name="iObParameter"></param>
        /// <returns></returns>
        ObParameterBase And(ObParameterBase iObParameter);

        /// <summary>
        /// 平级OR条件
        /// </summary>
        /// <param name="iObParameter"></param>
        /// <returns></returns>
        ObParameterBase Or(ObParameterBase iObParameter);

        /// <summary>
        /// 标识
        /// </summary>
        string Key { get; }
    }
}