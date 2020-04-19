/**
* 作    者：朱晓春(zhi_dian@163.com)
* 修改时间：2012-06-01 16:10:00
* 版 本 号：2.4.0
* 功能说明：增加条件与(&&)或(||)符号支持
* ----------------------------------
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public abstract class ObParameterBase : IObParameter
    {
        private IList<IObParameter> _brothers;
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// 平级兄弟列表
        /// </summary>
        public IList<IObParameter> Brothers
        {
            get
            {
                return _brothers ?? (_brothers = new List<IObParameter>());
            }
            protected set
            {
                _brothers = value;
            }
        }

        /// <summary>
        /// 0 无兄弟 1 AND 2 OR
        /// </summary>
        public int BrotherType { get; protected set; }

        /// <summary>
        /// SQL条件语句
        /// </summary>
        /// <returns></returns>
        public virtual string ToString(ref IList<DbParameter> dbParameters)
        {
            throw new NotImplementedException();
        }

        public virtual string ToString(char separator, ref IList<DbParameter> dbParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// SQL条件语句
        /// </summary>
        /// <param name="dbParameters">回带数据库参数</param>
        /// <param name="tables"></param>
        /// <returns></returns>
        public virtual string ToString(ref IList<DbParameter> dbParameters, out IList<string> tables)
        {
            throw new NotImplementedException();
        }

        public virtual string ToString(char separator, ref IList<DbParameter> dbParameters, out IList<string> tables)
        {
            throw new NotImplementedException();
        }

        public static ObParameterBase operator &(ObParameterBase obParameter, ObParameterBase obParameter2)
        {
            if (obParameter == null && obParameter2 == null)
                return null;
            if (obParameter == null)
                return obParameter2;
            if (obParameter2 == null)
                return obParameter;
            return obParameter.And(obParameter2);
        }

        /// <summary>
        /// 平级AND条件
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <returns></returns>
        public virtual ObParameterBase And(ObParameterBase iObParameter)
        {
            throw new NotImplementedException();
        }

        public static ObParameterBase operator |(ObParameterBase obParameter, ObParameterBase obParameter2)
        {
            if (obParameter == null && obParameter2 == null)
                return null;
            if (obParameter == null)
                return obParameter2;
            if (obParameter2 == null)
                return obParameter;
            return obParameter.Or(obParameter2);
        }

        public static ObParameterBase operator !(ObParameterBase obParameter)
        {
            var dbsValue = (DbTerm) obParameter.Value;
            switch (dbsValue.DbSymbol)
            {
                case DbSymbol.Equal://=
                    dbsValue.DbSymbol = DbSymbol.NotEqual;
                    break;
                case DbSymbol.NotEqual://<>
                    dbsValue.DbSymbol = DbSymbol.Equal;
                    break;
                case DbSymbol.LessEqual://<=
                    dbsValue.DbSymbol = DbSymbol.Than;
                    break;
                case DbSymbol.ThanEqual://>=
                    dbsValue.DbSymbol = DbSymbol.Less;
                    break;
                case DbSymbol.Than://>
                    dbsValue.DbSymbol = DbSymbol.LessEqual;
                    break;
                case DbSymbol.Less://<
                    dbsValue.DbSymbol = DbSymbol.ThanEqual;
                    break;
                case DbSymbol.Like:
                    dbsValue.DbSymbol = DbSymbol.NotLike;
                    break;
                case DbSymbol.LikeLeft:
                    dbsValue.DbSymbol = DbSymbol.NotLikeLeft;
                    break;
                case DbSymbol.LikeRight:
                    dbsValue.DbSymbol = DbSymbol.NotLikeRight;
                    break;
                case DbSymbol.NotLike:
                    dbsValue.DbSymbol = DbSymbol.Like;
                    break;
                case DbSymbol.NotLikeLeft:
                    dbsValue.DbSymbol = DbSymbol.LikeLeft;
                    break;
                case DbSymbol.NotLikeRight:
                    dbsValue.DbSymbol = DbSymbol.LikeRight;
                    break;
                case DbSymbol.In:
                    dbsValue.DbSymbol = DbSymbol.NotIn;
                    break;
                case DbSymbol.NotIn:
                    dbsValue.DbSymbol = DbSymbol.In;
                    break;
                case DbSymbol.Between:
                    dbsValue.DbSymbol = DbSymbol.NotBetween;
                    break;
                case DbSymbol.NotBetween:
                    dbsValue.DbSymbol = DbSymbol.Between;
                    break;
            }
            return obParameter;
        }
        
        /// <summary>
        /// 平级OR条件
        /// </summary>
        /// <param name="iObParameter">参数</param>
        /// <returns></returns>
        public virtual ObParameterBase Or(ObParameterBase iObParameter)
        {
            throw new NotImplementedException();
        }

        public string Key
        {
            get { return "Where_" + GetKey(this); }
        }

        private static string GetKey(IObParameter obParameter)
        {
            var retKey = "";
            if (obParameter.Value is DbTerm dbTerm1)
            {
                var strIn = "";
                if(dbTerm1.DbSymbol == DbSymbol.In ||
                    dbTerm1.DbSymbol == DbSymbol.NotIn)
                {
                    if (dbTerm1.Value is ICollection nvs)
                        strIn = "_" + nvs.Count;
                    //strIn = "_" + ((object[]) dbTerm.Value).Length;
                }
                retKey += dbTerm1.TableName + "_" + dbTerm1.ColumnName + "_" + dbTerm1.DbSymbol + strIn;
            }
            else if (obParameter.Value is DbTerm2 dbTerm2)
            {
                retKey += dbTerm2.TableName + "_" + dbTerm2.ColumnName + "_" + dbTerm2.DbSymbol;
            }
            else if (obParameter.Value is DbTerm3 dbTerm3)
            {
                retKey += GetKey(dbTerm3.SrcValue) + "_" + dbTerm3.DbSymbol;//.TableName + "_" + dbTerm.SrcValue.ColumnName + "_" + dbTerm.DbSymbol;
            }
            else if (obParameter.Value is DbNTerm dbTerm4)
            {
                retKey += dbTerm4.TableName + "_" + dbTerm4.ColumnName + "_" + dbTerm4.DbValue;
            }
            else if (obParameter.Value is DbNTerm2 dbTerm)
            {
                retKey += GetKey(dbTerm.Value) + "_" + dbTerm.DbValue;//.TableName + "_" + dbTerm.Value.ColumnName + "_" + dbTerm.DbValue;
            }
            foreach (var brother in obParameter.Brothers)
            {
                switch (brother.BrotherType)
                {
                    case 1:
                        retKey += "_AND";
                        break;
                    case 2:
                        retKey += "_OR";
                        break;
                }
                retKey += "_" + GetKey(brother);
            }
            return retKey;
        }

        private static string GetKey(IObProperty obProperty)
        {
            var retKey = obProperty.TableName + "_" + obProperty.ColumnName + (obProperty.AriSymbol != DbAriSymbol.Null ? "_" + obProperty.AriSymbol : "");
            foreach (var brother in obProperty.Brothers)
            {
                if (brother is IObProperty property)
                {
                    retKey += "_" + GetKey(property);
                }
            }
            return retKey;
        }

        public static bool operator true(ObParameterBase obParameter)
        {
            return false;
        }

        public static bool operator false(ObParameterBase obParameter)
        {
            return false;
        }
    }
}
