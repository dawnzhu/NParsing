/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2018-12-22 14.27:00
* 版 本 号：1.0.0
* 功能说明：创建
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Standard.NParsing.SQLite
{
    public class ObParameter : ObParameterBase
    {
        #region IObParameter Members

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="brotherType">0 无兄弟 1 AND 2 OR</param>
        /// <param name="brothers">平级兄弟列表</param>
        /// <param name="value">值</param>
        public ObParameter(int brotherType, IList<IObParameter> brothers, object value)
        {
            Value = value;
            Brothers = brothers;
            BrotherType = brotherType;
        }

        /// <summary>
        /// SQL条件语句
        /// </summary>
        /// <param name="separator">分隔符</param>
        /// <param name="dbParameters">回带数据库参数</param>
        /// <returns></returns>
        public override string ToString(char separator, ref IList<DbParameter> dbParameters)
        {
            return CreateWhere(this, separator, ref dbParameters);
        }

        public override string ToString(ref IList<DbParameter> dbParameters)
        {
            return CreateWhere(this, '.', ref dbParameters);
        }

        #endregion

        #region 创建Where条件语句 private static string CreateWhere(IObParameter iObParameter, ref List<DbParameter> dbParameter)

        /// <summary>
        /// 创建Where条件语句
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <param name="separator">分隔符</param>
        /// <param name="dbParameter">回带数据库参数</param>
        /// <returns></returns>
        private static string CreateWhere(IObParameter iObParameter, char separator, ref IList<DbParameter> dbParameter)
        {
            string sqlWhere = string.Empty;
            if (iObParameter.Value is DbTerm)
            {
                sqlWhere = CreateSymbolWhere(iObParameter, separator, ref dbParameter);
            }
            else if (iObParameter.Value is DbTerm2)
            {
                sqlWhere = CreateSymbolWhere(iObParameter, separator);
            }
            else if (iObParameter.Value is DbTerm3 term3)
            {
                term3.SrcValue = new ObProperty(term3.SrcValue);
                sqlWhere = CreateSymbolWhere2(iObParameter, separator, ref dbParameter);
            }
            else if (iObParameter.Value is DbNTerm)
            {
                sqlWhere = CreateValueWhere(iObParameter, separator);
            }
            else if (iObParameter.Value is DbNTerm2 term2)
            {
                term2.Value = new ObProperty(term2.Value);
                sqlWhere = CreateValueWhere(iObParameter, separator, ref dbParameter);
            }
            int iBrotherCount = iObParameter.Brothers.Count;
            for (int i = 0; i < iBrotherCount; i++)
            {
                var brother = iObParameter.Brothers[i];
                switch (brother.BrotherType)
                {
                    case 1:
                        sqlWhere += " AND ";
                        break;
                    case 2:
                        sqlWhere += " OR ";
                        break;
                }
                string andorWhere = "{0}";
                if (iObParameter.Brothers[i].Brothers.Count > 0)
                {
                    andorWhere = "(" + andorWhere + ")";
                }
                sqlWhere += string.Format(andorWhere, CreateWhere(brother, separator, ref dbParameter));
            }
            return sqlWhere;
        }

        #endregion

        #region 创建IS NULL, IS NOT NULL的Where条件语句 private static string CreateValueWhere(IObParameter iObParameter)

        /// <summary>
        /// 创建IS NULL, IS NOT NULL的Where条件语句
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private static string CreateValueWhere(IObParameter iObParameter, char separator)
        {
            string sqlWhere = string.Empty;
            var dbNTerm = (DbNTerm)iObParameter.Value;
            switch (dbNTerm.DbValue)
            {
                case DbValue.IsNull:
                    sqlWhere += string.Format("{0}{2}{1} IS NULL", dbNTerm.TableName, dbNTerm.ColumnName, separator);
                    break;
                case DbValue.IsNotNull:
                    sqlWhere += string.Format("{0}{2}{1} IS NOT NULL", dbNTerm.TableName, dbNTerm.ColumnName, separator);
                    break;
            }
            return sqlWhere;
        }

        #endregion

        #region 创建IS NULL, IS NOT NULL的Where条件语句 private static string CreateValueWhere(IObParameter iObParameter, ref IList<DbParameter> dbParameter)

        /// <summary>
        /// 创建IS NULL, IS NOT NULL的Where条件语句
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <param name="separator"></param>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        private static string CreateValueWhere(IObParameter iObParameter, char separator, ref IList<DbParameter> dbParameter)
        {
            string sqlWhere = string.Empty;
            var dbNTerm = (DbNTerm2)iObParameter.Value;
            switch (dbNTerm.DbValue)
            {
                case DbValue.IsNull:
                    sqlWhere += $"{dbNTerm.Value.ToString(separator, ref dbParameter)} IS NULL";
                    break;
                case DbValue.IsNotNull:
                    sqlWhere += $"{dbNTerm.Value.ToString(separator, ref dbParameter)} IS NOT NULL";
                    break;
            }
            return sqlWhere;
        }

        #endregion

        #region 创建带符号的Where条件语句 private static string CreateSymbolWhere(IObParameter iObParameter, ref List<DbParameter> dbParameter)

        /// <summary>
        /// 创建带符号的Where条件语句
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <param name="separator"></param>
        /// <param name="dbParameter">回带数据库参数</param>
        /// <returns></returns>
        private static string CreateSymbolWhere(IObParameter iObParameter, char separator, ref IList<DbParameter> dbParameter)
        {
            string sqlWhere = string.Empty;
            var dbTerm = (DbTerm)iObParameter.Value;
            string parameterName = "$" + dbTerm.ColumnName;

            #region 防止重复参数名

            int i = 0;
            foreach (var parameter in dbParameter)
            {
                var pn = parameter.ParameterName;
                if (pn.Length > parameterName.Length && pn.Substring(0, parameterName.Length).Equals(parameterName))
                    i++;
                else if (pn.Length == parameterName.Length && pn.Equals(parameterName))
                    i++;
            }
            parameterName += i == 0 ? "" : i.ToString(CultureInfo.InvariantCulture);

            #endregion

            switch (dbTerm.DbSymbol)
            {
                case DbSymbol.Equal:
                    sqlWhere += string.Format("{0}{3}{1}={2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.NotEqual:
                    sqlWhere += string.Format("{0}{3}{1}<>{2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.LessEqual:
                    sqlWhere += string.Format("{0}{3}{1}<={2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.ThanEqual:
                    sqlWhere += string.Format("{0}{3}{1}>={2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.Less:
                    sqlWhere += string.Format("{0}{3}{1}<{2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.Than:
                    sqlWhere += string.Format("{0}{3}{1}>{2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.Like:
                case DbSymbol.LikeLeft:
                case DbSymbol.LikeRight:
                    sqlWhere += string.Format("{0}{3}{1} LIKE {2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.NotLike:
                case DbSymbol.NotLikeLeft:
                case DbSymbol.NotLikeRight:
                    sqlWhere += string.Format("{0}{3}{1} NOT LIKE {2}", dbTerm.TableName, dbTerm.ColumnName, parameterName, separator);
                    break;
                case DbSymbol.In:
                    if (dbTerm.Value is ICollection vs)
                    {
                        var pns = string.Empty;
                        for (var ii = 0; ii < vs.Count; ii++)
                        {
                            if (ii > 0)
                                pns += ",";
                            pns += parameterName + ii;
                        }
                        sqlWhere += string.Format("{0}{3}{1} IN ({2})", dbTerm.TableName, dbTerm.ColumnName, pns, separator);
                    }
                    /*var vs = (object[])dbTerm.Value;
                    var pns = string.Empty;
                    for (int ii = 0; ii < vs.Length; ii++)
                    {
                        if (ii > 0)
                            pns += ",";
                        pns += parameterName + ii;
                    }
                    sqlWhere += string.Format("{0}.{1} IN ({2})", dbTerm.TableName, dbTerm.ColumnName, pns);*/
                    break;
                case DbSymbol.NotIn:
                    if (dbTerm.Value is ICollection nvs)
                    {
                        var npns = string.Empty;
                        for (var ii = 0; ii < nvs.Count; ii++)
                        {
                            if (ii > 0)
                                npns += ",";
                            npns += parameterName + ii;
                        }
                        sqlWhere += string.Format("{0}{3}{1} NOT IN ({2})", dbTerm.TableName, dbTerm.ColumnName, npns, separator);
                    }
                    /*var nvs = (object[])dbTerm.Value;
                    var npns = string.Empty;
                    for (int ii = 0; ii < nvs.Length; ii++)
                    {
                        if (ii > 0)
                            npns += ",";
                        npns += parameterName + ii;
                    }
                    sqlWhere += string.Format("{0}.{1} NOT IN ({2})", dbTerm.TableName, dbTerm.ColumnName, npns);*/
                    break;
                case DbSymbol.Between:
                    var bvs = dbTerm.Value as ICollection;
                    if (bvs == null || bvs.Count != 2)
                        throw new Exception("当条件运算符为Between时，值必需是长度为2的数组。");
                    sqlWhere += string.Format("{0}{4}{1} BETWEEN {2} AND {3}", dbTerm.TableName, dbTerm.ColumnName, parameterName + 0, parameterName + 1, separator);
                    break;
                case DbSymbol.NotBetween:
                    var nbvs = dbTerm.Value as ICollection;
                    if (nbvs == null || nbvs.Count != 2)
                        throw new Exception("当条件运算符为Not Between时，值必需是长度为2的数组。");
                    sqlWhere += string.Format("{0}{4}{1} NOT BETWEEN {2} AND {3}", dbTerm.TableName, dbTerm.ColumnName, parameterName + 0, parameterName + 1, separator);
                    break;
            }
            switch (dbTerm.DbSymbol)
            {
                case DbSymbol.Like:
                case DbSymbol.NotLike:
                    if (dbTerm.Value.IsEnum())
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    dbParameter.Add(new SQLiteParameter(parameterName, "%" + dbTerm.Value + "%"));
                    break;
                case DbSymbol.LikeLeft:
                case DbSymbol.NotLikeLeft:
                    if (dbTerm.Value.IsEnum())
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    dbParameter.Add(new SQLiteParameter(parameterName, "%" + dbTerm.Value));
                    break;
                case DbSymbol.LikeRight:
                case DbSymbol.NotLikeRight:
                    if (dbTerm.Value.IsEnum())
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    dbParameter.Add(new SQLiteParameter(parameterName, dbTerm.Value + "%"));
                    break;
                case DbSymbol.In:
                case DbSymbol.NotIn:
                case DbSymbol.Between:
                case DbSymbol.NotBetween:
                    if (dbTerm.Value is ICollection vs)
                    {
                        var ii = 0;
                        foreach (var v in vs)
                        {
                            var value = v.IsEnum() ? Convert.ToDecimal(v) : v;
                            dbParameter.Add(new SQLiteParameter(parameterName + ii, value));
                            ii++;
                        }
                    }
                    break;
/*                case DbSymbol.In:
                case DbSymbol.NotIn:
                    var vs = (object[])dbTerm.Value;
                    for (int ii = 0; ii < vs.Length; ii++)
                    {
                        if (vs[ii].IsEnum())
                            vs[ii] = Convert.ToDecimal(vs[ii]);
                        dbParameter.Add(new MySqlParameter(parameterName + ii, vs[ii]));
                    }
                    break;
                case DbSymbol.Between:
                case DbSymbol.NotBetween:
                    var bvs = (object[])dbTerm.Value;
                    for (var ii = 0; ii < bvs.Length; ii++)
                    {
                        if (bvs[ii].IsEnum())
                            bvs[ii] = Convert.ToDecimal(bvs[ii]);
                        dbParameter.Add(new MySqlParameter(parameterName + ii, bvs[ii]));
                    }
                    break;*/
                default:
                    if (dbTerm.Value.IsEnum())
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    dbParameter.Add(new SQLiteParameter(parameterName, dbTerm.Value));
                    break;
            }
            return sqlWhere;
        }

        #endregion

        #region 创建带符号的Where条件语句 private static string CreateSymbolWhere2(IObParameter iObParameter, ref List<DbParameter> dbParameter)

        /// <summary>
        /// 创建带符号的Where条件语句
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <param name="separator"></param>
        /// <param name="dbParameter">回带数据库参数</param>
        /// <returns></returns>
        private static string CreateSymbolWhere2(IObParameter iObParameter, char separator, ref IList<DbParameter> dbParameter)
        {
            string sqlWhere = string.Empty;
            var dbTerm = (DbTerm3)iObParameter.Value;
            string parameterName = "$" + dbTerm.SrcValue.ColumnName;

            if (!(dbTerm.DstValue is IObProperty))
            {
                #region 防止重复参数名

                int i = 0;
                foreach (var parameter in dbParameter)
                {
                    var pn = parameter.ParameterName;
                    if (pn.Length > parameterName.Length && pn.Substring(0, parameterName.Length).Equals(parameterName))
                        i++;
                    else if (pn.Length == parameterName.Length && pn.Equals(parameterName))
                        i++;
                }
                parameterName += i == 0 ? "" : i.ToString(CultureInfo.InvariantCulture);

                #endregion

                switch (dbTerm.DbSymbol)
                {
                    case DbSymbol.Like:
                    case DbSymbol.NotLike:
                        if (dbTerm.DstValue.IsEnum())
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        dbParameter.Add(new SQLiteParameter(parameterName, "%" + dbTerm.DstValue + "%"));
                        break;
                    case DbSymbol.LikeLeft:
                    case DbSymbol.NotLikeLeft:
                        if (dbTerm.DstValue.IsEnum())
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        dbParameter.Add(new SQLiteParameter(parameterName, "%" + dbTerm.DstValue));
                        break;
                    case DbSymbol.LikeRight:
                    case DbSymbol.NotLikeRight:
                        if (dbTerm.DstValue.IsEnum())
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        dbParameter.Add(new SQLiteParameter(parameterName, dbTerm.DstValue + "%"));
                        break;
                    case DbSymbol.In:
                    case DbSymbol.NotIn:
                    case DbSymbol.Between:
                    case DbSymbol.NotBetween:
                        if (dbTerm.DstValue is ICollection vs)
                        {
                            var ii = 0;
                            foreach (var v in vs)
                            {
                                var value = v.IsEnum() ? Convert.ToDecimal(v) : v;
                                dbParameter.Add(new SQLiteParameter(parameterName + ii, value));
                                ii++;
                            }
                        }
                        break;
/*                    case DbSymbol.In:
                    case DbSymbol.NotIn:
                        var vs = (object[])dbTerm.DstValue;
                        for (int ii = 0; ii < vs.Length; ii++)
                        {
                            if (vs[ii].IsEnum())
                                vs[ii] = Convert.ToDecimal(vs[ii]);
                            dbParameter.Add(new MySqlParameter(parameterName + ii, vs[ii]));
                        }
                        break;
                    case DbSymbol.Between:
                    case DbSymbol.NotBetween:
                        var bvs = (object[])dbTerm.DstValue;
                        for (var ii = 0; ii < bvs.Length; ii++)
                        {
                            if (bvs[ii].IsEnum())
                                bvs[ii] = Convert.ToDecimal(bvs[ii]);
                            dbParameter.Add(new MySqlParameter(parameterName + ii, bvs[ii]));
                        }
                        break;*/
                    default:
                        if (dbTerm.DstValue.IsEnum())
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        dbParameter.Add(new SQLiteParameter(parameterName, dbTerm.DstValue));
                        break;
                }
            }
            else if (dbTerm.DstValue != null)
            {
                dbTerm.DstValue = new ObProperty((IObProperty)dbTerm.DstValue);
                parameterName = ((IObProperty)dbTerm.DstValue).ToString(ref dbParameter);
            }
            switch (dbTerm.DbSymbol)
            {
                case DbSymbol.Equal:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)}={parameterName}";
                    break;
                case DbSymbol.NotEqual:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)}<>{parameterName}";
                    break;
                case DbSymbol.LessEqual:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)}<={parameterName}";
                    break;
                case DbSymbol.ThanEqual:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)}>={parameterName}";
                    break;
                case DbSymbol.Less:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)}<{parameterName}";
                    break;
                case DbSymbol.Than:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)}>{parameterName}";
                    break;
                case DbSymbol.Like:
                case DbSymbol.LikeLeft:
                case DbSymbol.LikeRight:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} LIKE {parameterName}";
                    break;
                case DbSymbol.NotLike:
                case DbSymbol.NotLikeLeft:
                case DbSymbol.NotLikeRight:
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} NOT LIKE {parameterName}";
                    break;
                case DbSymbol.In:
                    if (dbTerm.DstValue is ICollection vs)
                    {
                        var pns = string.Empty;
                        for (var ii = 0; ii < vs.Count; ii++)
                        {
                            if (ii > 0)
                                pns += ",";
                            pns += parameterName + ii;
                        }
                        sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} IN ({pns})";
                    }
                    /*var vs = (object[])dbTerm.DstValue;
                    var pns = string.Empty;
                    for (int ii = 0; ii < vs.Length; ii++)
                    {
                        if (ii > 0)
                            pns += ",";
                        pns += parameterName + ii;
                    }
                    sqlWhere += string.Format("{0} IN ({1})", dbTerm.SrcValue.ToString(ref dbParameter), pns);*/
                    break;
                case DbSymbol.NotIn:
                    if (dbTerm.DstValue is ICollection nvs)
                    {
                        var npns = string.Empty;
                        for (var ii = 0; ii < nvs.Count; ii++)
                        {
                            if (ii > 0)
                                npns += ",";
                            npns += parameterName + ii;
                        }
                        sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} NOT IN ({npns})";
                    }
                    /*var nvs = (object[])dbTerm.DstValue;
                    var npns = string.Empty;
                    for (int ii = 0; ii < nvs.Length; ii++)
                    {
                        if (ii > 0)
                            npns += ",";
                        npns += parameterName + ii;
                    }
                    sqlWhere += string.Format("{0} NOT IN ({1})", dbTerm.SrcValue.ToString(ref dbParameter), npns);*/
                    break;
                case DbSymbol.Between:
                    var bvs = dbTerm.DstValue as ICollection;
                    if (bvs == null || bvs.Count != 2)
                        throw new Exception("当条件运算符为Between时，值必需是长度为2的数组。");
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} BETWEEN {parameterName + 0} AND {parameterName + 1}";
                    break;
                case DbSymbol.NotBetween:
                    var nbvs = dbTerm.DstValue as ICollection;
                    if (nbvs == null || nbvs.Count != 2)
                        throw new Exception("当条件运算符为Not Between时，值必需是长度为2的数组。");
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} NOT BETWEEN {parameterName + 0} AND {parameterName + 1}";
                    break;
            }
            return sqlWhere;
        }

        #endregion

        #region 创建Where语句 private static string CreateSymbolWhere(IObParameter iObParameter)

        /// <summary>
        /// 创建Where语句
        /// </summary>
        /// <param name="iObParameter"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private static string CreateSymbolWhere(IObParameter iObParameter, char separator)
        {
            string sqlWhere = string.Empty;
            var dbTerm = (DbTerm2)iObParameter.Value;
            switch (dbTerm.DbSymbol)
            {
                case DbSymbol.Equal:
                    sqlWhere += string.Format("{0}{4}{1}={2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.NotEqual:
                    sqlWhere += string.Format("{0}{4}{1}<>{2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.LessEqual:
                    sqlWhere += string.Format("{0}{4}{1}<={2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.ThanEqual:
                    sqlWhere += string.Format("{0}{4}{1}>={2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.Less:
                    sqlWhere += string.Format("{0}{4}{1}<{2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.Than:
                    sqlWhere += string.Format("{0}{4}{1}>{2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.Like:
                case DbSymbol.LikeLeft:
                case DbSymbol.LikeRight:
                    sqlWhere += string.Format("{0}{4}{1} LIKE {2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.NotLike:
                case DbSymbol.NotLikeLeft:
                case DbSymbol.NotLikeRight:
                    sqlWhere += string.Format("{0}{4}{1} NOT LIKE {2}{4}{3}", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.In:
                    sqlWhere += string.Format("{0}{4}{1} IN ({2}{4}{3})", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
                case DbSymbol.NotIn:
                    sqlWhere += string.Format("{0}{4}{1} NOT IN ({2}{4}{3})", dbTerm.TableName, dbTerm.ColumnName, dbTerm.TableName2, dbTerm.ColumnName2, separator);
                    break;
            }
            return sqlWhere;
        }

        #endregion
    }
}