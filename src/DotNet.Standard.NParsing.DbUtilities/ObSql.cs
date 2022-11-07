using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObSql<TModel> : IObSql<TModel> where TModel : class, new()
    {
        private readonly IDbHelper _iDbHelper;
        //private readonly IObRedefine _iObRedefine;
        private readonly IObTransaction _iObTransaction;
        private readonly CommandType _commandType;
        private readonly string _commandText;
        private readonly DbParameter[] _commandParameters;
        public bool CreateEmptyObject { get; set; }

        public ObSql(IDbHelper iDbHelper/*, IObRedefine iObRedefine*/, IObTransaction iObTransaction, CommandType commandType, string commandText,
            params DbParameter[] commandParameters) : this(iDbHelper, true, iObTransaction, commandType, commandText, commandParameters)
        { 
        }

        public ObSql(IDbHelper iDbHelper/*, IObRedefine iObRedefine*/, bool createEmptyObject, IObTransaction iObTransaction, CommandType commandType, string commandText,
            params DbParameter[] commandParameters)
        {
            _iDbHelper = iDbHelper;
            //_iObRedefine = iObRedefine;
            _iObTransaction = iObTransaction;
            _commandType = commandType;
            _commandText = commandText;
            _commandParameters = commandParameters;
            CreateEmptyObject = createEmptyObject;
        }

        public TModel ToModel()
        {
            TModel model;
            DbDataReader dr;

            #region 读取数据

            if (_iObTransaction != null)
            {
                dr = DbHelper.ExecuteReader(_iDbHelper, _iObTransaction.DbTransaction, _commandType, _commandText, _commandParameters);
                model = dr.ToModel<TModel>(CreateEmptyObject);
                if (!dr.IsClosed)
                {
                    dr.Close();
                }
                dr.Dispose();
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    dr = dbHelper.ExecuteReader(_commandType, _commandText, _commandParameters);
                    model = dr.ToModel<TModel>(CreateEmptyObject);
                }
            }

            #endregion

            return model;
        }

        public IList<TModel> ToList()
        {
            IList<TModel> models;
            DbDataReader dr;

            #region 读取数据

            if (_iObTransaction != null)
            {
                dr = DbHelper.ExecuteReader(_iDbHelper, _iObTransaction.DbTransaction, _commandType, _commandText, _commandParameters);
                models = dr.ToList<TModel>(CreateEmptyObject);
                if (!dr.IsClosed)
                {
                    dr.Close();
                }
                dr.Dispose();
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    dr = dbHelper.ExecuteReader(_commandType, _commandText, _commandParameters);
                    models = dr.ToList<TModel>(CreateEmptyObject);
                }
            }

            #endregion

            return models;
        }

        public DataTable ToTable()
        {
            DataSet ds;

            #region 读取数据

            if (_iObTransaction != null)
            {
                ds = DbHelper.ExecuteDataset(_iDbHelper, _iObTransaction.DbTransaction, _commandType, _commandText, _commandParameters);
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    ds = dbHelper.ExecuteDataset(_commandType, _commandText, _commandParameters);
                }
            }

            #endregion

            return ds.Tables[0];
        }

        public DataTableCollection ToTables()
        {
            DataSet ds;

            #region 读取数据

            if (_iObTransaction != null)
            {
                ds = DbHelper.ExecuteDataset(_iDbHelper, _iObTransaction.DbTransaction, _commandType, _commandText, _commandParameters);
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    ds = dbHelper.ExecuteDataset(_commandType, _commandText, _commandParameters);
                }
            }

            #endregion

            return ds.Tables;
        }

        public object Scalar()
        {
            object obj;

            #region 读取数据

            if (_iObTransaction != null)
            {
                obj = DbHelper.ExecuteScalar(_iDbHelper, _iObTransaction.DbTransaction, _commandType, _commandText, _commandParameters);
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    obj = dbHelper.ExecuteScalar(_commandType, _commandText, _commandParameters);
                }
            }
            obj = obj != DBNull.Value && obj != null ? obj : null;

            #endregion

            return obj;
        }

        public int Exec()
        {
            int ret;

            #region 读取数据

            if (_iObTransaction != null)
            {
                ret = DbHelper.ExecuteNonQuery(_iDbHelper, _iObTransaction.DbTransaction, _commandType, _commandText, _commandParameters);
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    ret = dbHelper.ExecuteNonQuery(_commandType, _commandText, _commandParameters);
                }
            }

            #endregion

            return ret;
        }
    }
}
