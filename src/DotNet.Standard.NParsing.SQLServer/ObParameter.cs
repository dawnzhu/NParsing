/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-26 10:13:53
* 版 本 号：1.0.0
* 功能说明：SQL WHERE 条件成生实现类
* ----------------------------------
* 修改标识：修改+增加功能
* 修 改 人：朱晓春
* 日    期：2009-09-01 15:01:00
* 版 本 号：1.0.0
* 修改内容：修改string SqlWhere {get;}属性string GetSqlWhere();为方法,并增加string GetSqlWhere(ref List<DbParameter> dbParameters);
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 08:59:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Standard.Hibernate.SQLServer->DotNet.Standard.Transport.SQLServer)和类名(O2rParameter->ObParameter)
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-18 17：22:00
* 版 本 号：1.0.1
* 修改内容：修正参数LIKE条件生成方法
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
* 修改内容：增加创建子参数功能 public ObParameter(IObParameter iObParameter)
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2012-04-23 16:20:00
* 版 本 号：2.3.0
* 修改内容：遵循SQL执行计划规则，当参数是字符串时，必需指定长度，默认指定为nvarchar(max)
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2012-06-04 15:41:00
* 版 本 号：2.4.0
* 修改内容：增加Between和Not Between运算
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.Utilities;

namespace DotNet.Standard.NParsing.SQLServer
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
        /// <param name="separator">分隔符</param>
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
        /// <param name="separator">分隔符</param>
        /// <param name="dbParameter">回带数据库参数</param>
        /// <returns></returns>
        private static string CreateSymbolWhere(IObParameter iObParameter, char separator, ref IList<DbParameter> dbParameter)
        {
            var sqlWhere = string.Empty;
            var dbTerm = (DbTerm)iObParameter.Value;
            var parameterName = "@" + dbTerm.ColumnName;

            #region 防止重复参数名

            var i = dbParameter.Count(obj => obj.ParameterName.StartsWith(parameterName));
            parameterName += i == 0 ? "" : i.ToString(CultureInfo.InvariantCulture);

            #endregion

            switch (dbTerm.DbSymbol)
            {
                case DbSymbol.Equal:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName}={parameterName}";
                    break;
                case DbSymbol.NotEqual:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName}<>{parameterName}";
                    break;
                case DbSymbol.LessEqual:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName}<={parameterName}";
                    break;
                case DbSymbol.ThanEqual:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName}>={parameterName}";
                    break;
                case DbSymbol.Less:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName}<{parameterName}";
                    break;
                case DbSymbol.Than:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName}>{parameterName}";
                    break;
                case DbSymbol.Like:
                case DbSymbol.LikeLeft:
                case DbSymbol.LikeRight:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName} LIKE {parameterName}";
                    break;
                case DbSymbol.NotLike:
                case DbSymbol.NotLikeLeft:
                case DbSymbol.NotLikeRight:
                    sqlWhere += $"{dbTerm.TableName}{separator}{dbTerm.ColumnName} NOT LIKE {parameterName}";
                    break;
                case DbSymbol.In:
                    if (dbTerm.Value is ICollection vs)
                    {
                        if (vs.Count == 0)
                        {
                            sqlWhere += "1=2";
                        }
                        else
                        {
                            var pns = string.Empty;
                            for (var ii = 0; ii < vs.Count; ii++)
                            {
                                if (ii > 0)
                                    pns += ",";
                                pns += parameterName + "_" + ii;
                            }
                            sqlWhere += string.Format("{0}{3}{1} IN ({2})", dbTerm.TableName, dbTerm.ColumnName, pns, separator);
                        }
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
                    if (dbTerm.Value is ICollection nvs && nvs.Count > 0)
                    {
                        var npns = string.Empty;
                        for (var ii = 0; ii < nvs.Count; ii++)
                        {
                            if (ii > 0)
                                npns += ",";
                            npns += parameterName + "_" + ii;
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
                    sqlWhere += string.Format("{0}{4}{1} BETWEEN {2} AND {3}", dbTerm.TableName, dbTerm.ColumnName, parameterName + "_0", parameterName + "_1", separator);
                    break;
                case DbSymbol.NotBetween:
                    var nbvs = dbTerm.Value as ICollection;
                    if (nbvs == null || nbvs.Count != 2)
                        throw new Exception("当条件运算符为Not Between时，值必需是长度为2的数组。");
                    sqlWhere += string.Format("{0}{4}{1} NOT BETWEEN {2} AND {3}", dbTerm.TableName, dbTerm.ColumnName, parameterName + "_0", parameterName + "_1", separator);
                    break;
            }
            var sqlParameter = new SqlParameter { ParameterName = parameterName };
            switch (dbTerm.DbSymbol)
            {
                case DbSymbol.Like:
                case DbSymbol.NotLike:
                    if (dbTerm.Value.IsString())
                    {
                        var value = dbTerm.Value.ToString();
                        sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                        sqlParameter.Size = value.Length + 2;
                    }
                    else if (dbTerm.Value.IsEnum())
                    {
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    }
                    sqlParameter.Value = "%" + dbTerm.Value + "%";
                    dbParameter.Add(sqlParameter);
                    break;
                case DbSymbol.LikeLeft:
                case DbSymbol.NotLikeLeft:
                    if (dbTerm.Value.IsString())
                    {
                        var value = dbTerm.Value.ToString();
                        sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                        sqlParameter.Size = value.Length + 1;
                    }
                    else if (dbTerm.Value.IsEnum())
                    {
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    }
                    sqlParameter.Value = "%" + dbTerm.Value;
                    dbParameter.Add(sqlParameter);
                    break;
                case DbSymbol.LikeRight:
                case DbSymbol.NotLikeRight:
                    if (dbTerm.Value.IsString())
                    {
                        var value = dbTerm.Value.ToString();
                        sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                        sqlParameter.Size = value.Length + 1;
                    }
                    else if (dbTerm.Value.IsEnum())
                    {
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    }
                    sqlParameter.Value = dbTerm.Value + "%";
                    dbParameter.Add(sqlParameter);
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
                            object value;
                            sqlParameter = new SqlParameter {ParameterName = parameterName + "_" + ii};
                            if (v.IsString())
                            {
                                var vv = v.ToString();
                                sqlParameter.SqlDbType = vv.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                                sqlParameter.Size = vv.Length == 0 ? 1 : vv.Length;
                                value = vv;
                            }
                            else if (v.IsEnum())
                            {
                                value = Convert.ToDecimal(v);
                            }
                            else
                            {
                                value = v;
                            }
                            sqlParameter.Value = value;
                            dbParameter.Add(sqlParameter);
                            ii++;
                        }
                    }
                    break;
/*                case DbSymbol.In:
                case DbSymbol.NotIn:
                    var vs = (object[])dbTerm.Value;
                    for (var ii = 0; ii < vs.Length; ii++)
                    {
                        sqlParameter = new SqlParameter { ParameterName = parameterName + ii };
                        if (vs[ii].IsString())
                        {
                            sqlParameter.SqlDbType = SqlDbType.NVarChar;
                            sqlParameter.Size = -1;
                        }
                        else if (vs[ii].IsEnum())
                        {
                            vs[ii] = Convert.ToDecimal(vs[ii]);
                        }
                        sqlParameter.Value = vs[ii];
                        dbParameter.Add(sqlParameter);
                    }
                    break;
                case DbSymbol.Between:
                case DbSymbol.NotBetween:
                    var bvs = (object[]) dbTerm.Value;
                    for (var ii = 0; ii < bvs.Length; ii++)
                    {
                        sqlParameter = new SqlParameter { ParameterName = parameterName + ii };
                        if (bvs[ii].IsString())
                        {
                            sqlParameter.SqlDbType = SqlDbType.NVarChar;
                            sqlParameter.Size = -1;
                        }
                        else if (bvs[ii].IsEnum())
                        {
                            bvs[ii] = Convert.ToDecimal(bvs[ii]);
                        }
                        sqlParameter.Value = bvs[ii];
                        dbParameter.Add(sqlParameter);
                    }
                    break;*/
                default:
                    if (dbTerm.Value.IsString())
                    {
                        var value = dbTerm.Value.ToString();
                        sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                        sqlParameter.Size = value.Length == 0 ? 1 : value.Length;
                    }
                    else if (dbTerm.Value.IsEnum())
                    {
                        dbTerm.Value = Convert.ToDecimal(dbTerm.Value);
                    }
                    sqlParameter.Value = dbTerm.Value;
                    dbParameter.Add(sqlParameter);
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
        /// <param name="separator">分隔符</param>
        /// <param name="dbParameter">回带数据库参数</param>
        /// <returns></returns>
        private static string CreateSymbolWhere2(IObParameter iObParameter, char separator, ref IList<DbParameter> dbParameter)
        {
            string sqlWhere = string.Empty;
            var dbTerm = (DbTerm3)iObParameter.Value;
            string parameterName = "@" + dbTerm.SrcValue.ColumnName;

            if (!(dbTerm.DstValue is IObProperty))
            {
                #region 防止重复参数名

                var i = dbParameter.Count(obj => obj.ParameterName.StartsWith(parameterName));
                parameterName += i == 0 ? "" : i.ToString(CultureInfo.InvariantCulture);

                #endregion

                var sqlParameter = new SqlParameter { ParameterName = parameterName };
                switch (dbTerm.DbSymbol)
                {
                    case DbSymbol.Like:
                    case DbSymbol.NotLike:
                        if (dbTerm.DstValue.IsString())
                        {
                            var value = dbTerm.DstValue.ToString();
                            sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                            sqlParameter.Size = value.Length + 2;
                        }
                        else if (dbTerm.DstValue.IsEnum())
                        {
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        }
                        sqlParameter.Value = "%" + dbTerm.DstValue + "%";
                        dbParameter.Add(sqlParameter);
                        break;
                    case DbSymbol.LikeLeft:
                    case DbSymbol.NotLikeLeft:
                        if (dbTerm.DstValue.IsString())
                        {
                            var value = dbTerm.DstValue.ToString();
                            sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                            sqlParameter.Size = value.Length + 1;
                        }
                        else if (dbTerm.DstValue.IsEnum())
                        {
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        }
                        sqlParameter.Value = "%" + dbTerm.DstValue;
                        dbParameter.Add(sqlParameter);
                        break;
                    case DbSymbol.LikeRight:
                    case DbSymbol.NotLikeRight:
                        if (dbTerm.DstValue.IsString())
                        {
                            var value = dbTerm.DstValue.ToString();
                            sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                            sqlParameter.Size = value.Length + 1;
                        }
                        else if (dbTerm.DstValue.IsEnum())
                        {
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        }
                        sqlParameter.Value = dbTerm.DstValue + "%";
                        dbParameter.Add(sqlParameter);
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
                                object value;
                                sqlParameter = new SqlParameter {ParameterName = parameterName + "_" + ii};
                                if (v.IsString())
                                {
                                    var vv = v.ToString();
                                    sqlParameter.SqlDbType = vv.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                                    sqlParameter.Size = vv.Length == 0 ? 1 : vv.Length;
                                    value = vv;
                                }
                                else if (v.IsEnum())
                                {
                                    value = Convert.ToDecimal(v);
                                }
                                else
                                {
                                    value = v;
                                }
                                sqlParameter.Value = value;
                                dbParameter.Add(sqlParameter);
                                ii++;
                            }
                        }
                        /*var vs = (object[])dbTerm.DstValue;
                            for (var ii = 0; ii < vs.Length; ii++)
                            {
                                sqlParameter = new SqlParameter {ParameterName = parameterName + ii};
                                if (vs[ii].IsString())
                                {
                                    sqlParameter.SqlDbType = SqlDbType.NVarChar;
                                    sqlParameter.Size = -1;
                                }
                                else if (vs[ii].IsEnum())
                                {
                                    vs[ii] = Convert.ToDecimal(vs[ii]);
                                }
                                sqlParameter.Value = vs[ii];
                                dbParameter.Add(sqlParameter);
                            }*/
                        break;
/*                    case DbSymbol.Between:
                    case DbSymbol.NotBetween:
                        var bvs = (object[])dbTerm.DstValue;
                        for (var ii = 0; ii < bvs.Length; ii++)
                        {
                            sqlParameter = new SqlParameter { ParameterName = parameterName + ii };
                            if (bvs[ii].IsString())
                            {
                                sqlParameter.SqlDbType = SqlDbType.NVarChar;
                                sqlParameter.Size = -1;
                            }
                            else if (bvs[ii].IsEnum())
                            {
                                bvs[ii] = Convert.ToDecimal(bvs[ii]);
                            }
                            sqlParameter.Value = bvs[ii];
                            dbParameter.Add(sqlParameter);
                        }
                        break;*/
                    default:
                        if (dbTerm.DstValue.IsString())
                        {
                            var value = dbTerm.DstValue.ToString();
                            sqlParameter.SqlDbType = value.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                            sqlParameter.Size = value.Length == 0 ? 1 : value.Length;
                        }
                        else if (dbTerm.DstValue.IsEnum())
                        {
                            dbTerm.DstValue = Convert.ToDecimal(dbTerm.DstValue);
                        }
                        sqlParameter.Value = dbTerm.DstValue;
                        dbParameter.Add(sqlParameter);
                        break;
                }
            }
            else if(dbTerm.DstValue != null)
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
                        if (vs.Count == 0)
                        {
                            sqlWhere += "1=2";
                        }
                        else
                        {
                            var pns = string.Empty;
                            for (var ii = 0; ii < vs.Count; ii++)
                            {
                                if (ii > 0)
                                    pns += ",";
                                pns += parameterName + "_" + ii;
                            }
                            sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} IN ({pns})";
                        }
                    }
/*                    var vs = (object[])dbTerm.DstValue;
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
                    if (dbTerm.DstValue is ICollection nvs && nvs.Count > 0)
                    {
                        var npns = string.Empty;
                        for (var ii = 0; ii < nvs.Count; ii++)
                        {
                            if (ii > 0)
                                npns += ",";
                            npns += parameterName + "_" + ii;
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
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} BETWEEN {parameterName + "_0"} AND {parameterName + "_1"}";
                    break;
                case DbSymbol.NotBetween:
                    var nbvs = dbTerm.DstValue as ICollection;
                    if (nbvs == null || nbvs.Count != 2)
                        throw new Exception("当条件运算符为Not Between时，值必需是长度为2的数组。");
                    sqlWhere += $"{dbTerm.SrcValue.ToString(separator, ref dbParameter)} NOT BETWEEN {parameterName + "_0"} AND {parameterName + "_1"}";
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