using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObParameter : ObParameterBase
    {
        #region IObParameter Members

        /// <summary>
        /// 创建子参数
        /// </summary>
        /// <param name="iObParameter">参数</param>
        public ObParameter(IObParameter iObParameter)
        {
            Brothers.Add(iObParameter);
            Value = null;
        }

        /// <summary>
        /// 创建带符号参数
        /// </summary>
        /// <param name="srcValue"></param>
        /// <param name="dbSymbol"></param>
        /// <param name="dstValue"></param>
        public ObParameter(IObProperty srcValue, DbSymbol dbSymbol, object dstValue)
        {
            var dbTrem = new DbTerm3(srcValue, dbSymbol, dstValue);
            Value = dbTrem;
        }

        /// <summary>
        /// 创建IS NULL, IS NOT NULL参数
        /// </summary>
        /// <param name="value"></param>
        /// <param name="dbValue"></param>
        public ObParameter(IObProperty value, DbValue dbValue)
        {
            var dbTrem = new DbNTerm2(value, dbValue);
            Value = dbTrem;
        }

        /// <summary>
        /// 创建带符号参数
        /// </summary>
        /// <param name="tableName">对象类型名(表名)</param>
        /// <param name="columnName">属性名(字段名)</param>
        /// <param name="dbSymbol">符号</param>
        /// <param name="value">值</param>
        public ObParameter(string tableName, string columnName, DbSymbol dbSymbol, object value)
        {
            var dbTrem = new DbTerm(tableName, columnName, dbSymbol, value);
            Value = dbTrem;
        }

        /// <summary>
        /// 创建带符号参数
        /// </summary>
        /// <param name="tableName">对象类型名(表名)</param>
        /// <param name="columnName">属性名(字段名)</param>
        /// <param name="dbSymbol">符号</param>
        /// <param name="tableName2">对象类型名(表名)</param>
        /// <param name="columnName2">属性名(字段名)</param>
        public ObParameter(string tableName, string columnName, DbSymbol dbSymbol, string tableName2, string columnName2)
        {
            var dbTrem = new DbTerm2(tableName, columnName, dbSymbol, tableName2, columnName2);
            Value = dbTrem;
        }

        /// <summary>
        /// 创建IS NULL, IS NOT NULL参数
        /// </summary>
        /// <param name="tableName">对象类型名(表名)</param>
        /// <param name="columnName">属性名(字段名)</param>
        /// <param name="dbValue">IS NULL, IS NOT NULL</param>
        public ObParameter(string tableName, string columnName, DbValue dbValue)
        {
            var dbNTrem = new DbNTerm(tableName, columnName, dbValue);
            Value = dbNTrem;
        }

        /// <summary>
        /// 平级AND条件
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <returns></returns>
        public override ObParameterBase And(ObParameterBase iObParameter)
        {
            ((ObParameter)iObParameter).BrotherType = 1;
            Brothers.Add(iObParameter);
            return this;
        }

        /// <summary>
        /// 平级OR条件
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <returns></returns>
        public override ObParameterBase Or(ObParameterBase iObParameter)
        {
            ((ObParameter)iObParameter).BrotherType = 2;
            Brothers.Add(iObParameter);
            return this;
        }

        #endregion
    }
}
