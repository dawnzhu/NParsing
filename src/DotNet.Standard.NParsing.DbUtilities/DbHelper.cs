/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-03-31 09:51:34
* 版 本 号：1.0.0
* 功能说明：创建 公用数据库操作类
* ----------------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class DbHelper : IDisposable
    {
        private readonly DbConnection _dbConn;
        private readonly IDbHelper _iDbHelper;

        public DbHelper(IDbHelper iDbHelper)
        {
            _iDbHelper = iDbHelper;
            _dbConn = iDbHelper.DbConnection();
            _dbConn.Open();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_dbConn != null)
            {
                if (_dbConn.State == ConnectionState.Open)
                {
                    _dbConn.Close();
                }
                _dbConn.Dispose();
            }
        }

        #endregion

        #region AttachParameters

        private static void AttachParameters(DbCommand command, IEnumerable<DbParameter> commandParameters)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (commandParameters != null)
            {
                foreach (DbParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        #endregion

        #region Exists

        /*public bool Exists(string commandText)
        {
            return Exists(commandText, new DbParameter[0]);
        }*/

        public bool Exists(string commandText, params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = _iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = _dbConn;
                    dbCom.CommandType = CommandType.Text;
                    dbCom.CommandText = commandText;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    object obj = dbCom.ExecuteScalar();

                    int cmdresult;
                    if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                    {
                        cmdresult = 0;
                    }
                    else
                    {
                        cmdresult = int.Parse(obj.ToString());
                    }
                    if (cmdresult == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception er)
                {
                    throw new ObException(er, CommandType.Text, commandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        public static bool Exists(IDbHelper iDbHelper, DbTransaction transaction, string commandText, params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = transaction.Connection;
                    dbCom.Transaction = transaction;
                    dbCom.CommandType = CommandType.Text;
                    dbCom.CommandText = commandText;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    object obj = dbCom.ExecuteScalar();

                    int cmdresult;
                    if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
                    {
                        cmdresult = 0;
                    }
                    else
                    {
                        cmdresult = int.Parse(obj.ToString());
                    }
                    if (cmdresult == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception er)
                {
                    throw new ObException(er, CommandType.Text, commandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        #endregion

        #region ExecuteNonQuery

        /*public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, new DbParameter[0]);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(commandType, commandText, new DbParameter[0]);
        }*/

        public int ExecuteNonQuery(string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, commandParameters);
        }

        public int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = _iDbHelper.DbCommand())
            {
                int iRet;
                try
                {
                    dbCom.Connection = _dbConn;
                    dbCom.CommandType = commandType;
                    dbCom.CommandText = commandText;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    iRet = dbCom.ExecuteNonQuery();
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, commandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
                return iRet;
            }
        }

        /*public int ExecuteNonQuery(string[] commandTexts)
        {
            return ExecuteNonQuery(CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public int ExecuteNonQuery(CommandType commandType, string[] commandTexts)
        {
            return ExecuteNonQuery(commandType, commandTexts, new DbParameter[0]);
        }*/

        public int ExecuteNonQuery(string[] commandTexts, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandTexts, commandParameters);
        }

        public int ExecuteNonQuery(CommandType commandType, string[] commandTexts, params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = _iDbHelper.DbCommand())
            {
                int iRet = 0;
                try
                {
                    dbCom.Connection = _dbConn;
                    dbCom.CommandType = commandType;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    foreach (var commandText in commandTexts)
                    {
                        dbCom.CommandText = commandText;
                        iRet += dbCom.ExecuteNonQuery();
                    }
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, dbCom.CommandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
                return iRet;
            }
        }

        /*public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, string commandText)
        {
            return ExecuteNonQuery(iDbHelper, transaction, CommandType.Text, commandText, new DbParameter[0]);
        }

        public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(iDbHelper, transaction, commandType, commandText, new DbParameter[0]);
        }*/

        public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(iDbHelper, transaction, CommandType.Text, commandText, commandParameters);
        }

        public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string commandText,
                                   params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = iDbHelper.DbCommand())
            {
                int iRet;
                try
                {
                    dbCom.Connection = transaction.Connection;
                    dbCom.Transaction = transaction;
                    dbCom.CommandType = commandType;
                    dbCom.CommandText = commandText;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    iRet = dbCom.ExecuteNonQuery();
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, commandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
                return iRet;
            }
        }

        /*public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts)
        {
            return ExecuteNonQuery(iDbHelper, transaction, CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string[] commandTexts)
        {
            return ExecuteNonQuery(iDbHelper, transaction, commandType, commandTexts, new DbParameter[0]);
        }*/

        public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(iDbHelper, transaction, CommandType.Text, commandTexts, commandParameters);
        }

        public static int ExecuteNonQuery(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string[] commandTexts,
                                   params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = iDbHelper.DbCommand())
            {
                int iRet = 0;
                try
                {
                    dbCom.Connection = transaction.Connection;
                    dbCom.Transaction = transaction;
                    dbCom.CommandType = commandType;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    foreach (var commandText in commandTexts)
                    {
                        dbCom.CommandText = commandText;
                        iRet += dbCom.ExecuteNonQuery();
                    }
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, dbCom.CommandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
                return iRet;
            }
        }

        #endregion

        #region ExecuteDataset

        /*public DataSet ExecuteDataset(string commandText)
        {
            return ExecuteDataset(CommandType.Text, commandText, new DbParameter[0]);
        }

        public DataSet ExecuteDataset(CommandType commandType, string commandText)
        {
            return ExecuteDataset(commandType, commandText, new DbParameter[0]);
        }*/

        public DataSet ExecuteDataset(string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteDataset(CommandType.Text, commandText, commandParameters);
        }

        public DataSet ExecuteDataset(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            using (DbDataAdapter da = _iDbHelper.DbDataAdapter())
            {
                try
                {
                    da.SelectCommand = _iDbHelper.DbCommand();
                    da.SelectCommand.Connection = _dbConn;
                    da.SelectCommand.CommandType = commandType;
                    da.SelectCommand.CommandText = commandText;
                    da.SelectCommand.CommandTimeout = 240;
                    AttachParameters(da.SelectCommand, commandParameters);
                    var dataSet = new DataSet();
                    da.Fill(dataSet);
                    return dataSet;
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, commandText, commandParameters);
                }
                finally
                {
                    da.SelectCommand.Parameters.Clear();
                }
            }
        }

        /*public DataSet ExecuteDataset(string[] commandTexts)
        {
            return ExecuteDataset(CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public DataSet ExecuteDataset(CommandType commandType, string[] commandTexts)
        {
            return ExecuteDataset(commandType, commandTexts, new DbParameter[0]);
        }*/

        public DataSet ExecuteDataset(string[] commandTexts, params DbParameter[] commandParameters)
        {
            return ExecuteDataset(CommandType.Text, commandTexts, commandParameters);
        }

        public DataSet ExecuteDataset(CommandType commandType, string[] commandTexts, params DbParameter[] commandParameters)
        {
            using (DbDataAdapter da = _iDbHelper.DbDataAdapter())
            {
                try
                {
                    da.SelectCommand = _iDbHelper.DbCommand();
                    da.SelectCommand.Connection = _dbConn;
                    da.SelectCommand.CommandType = commandType;
                    da.SelectCommand.CommandTimeout = 240;
                    AttachParameters(da.SelectCommand, commandParameters);
                    for (int i = 0; i < commandTexts.Length - 1; i++)
                    {
                        da.SelectCommand.CommandText = commandTexts[i];
                        da.SelectCommand.ExecuteNonQuery();
                    }
                    da.SelectCommand.CommandText = commandTexts[commandTexts.Length - 1];
                    var dataSet = new DataSet();
                    da.Fill(dataSet);
                    return dataSet;
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, da.SelectCommand.CommandText, commandParameters);
                }
                finally
                {
                    da.SelectCommand.Parameters.Clear();
                }
            }
        }

        /*public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, string commandText)
        {
            return ExecuteDataset(iDbHelper, transaction, CommandType.Text, commandText, new DbParameter[0]);
        }

        public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(iDbHelper, transaction, commandType, commandText, new DbParameter[0]);
        }*/

        public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteDataset(iDbHelper, transaction, CommandType.Text, commandText, commandParameters);
        }

        public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            using (DbDataAdapter da = iDbHelper.DbDataAdapter())
            {
                try
                {
                    da.SelectCommand = iDbHelper.DbCommand();
                    da.SelectCommand.Connection = transaction.Connection;
                    da.SelectCommand.Transaction = transaction;
                    da.SelectCommand.CommandType = commandType;
                    da.SelectCommand.CommandText = commandText;
                    da.SelectCommand.CommandTimeout = 240;
                    AttachParameters(da.SelectCommand, commandParameters);
                    var dataSet = new DataSet();
                    da.Fill(dataSet);
                    return dataSet;
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, commandText, commandParameters);
                }
                finally
                {
                    da.SelectCommand.Parameters.Clear();
                }
            }
        }

        /*public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts)
        {
            return ExecuteDataset(iDbHelper, transaction, CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string[] commandTexts)
        {
            return ExecuteDataset(iDbHelper, transaction, commandType, commandTexts, new DbParameter[0]);
        }*/

        public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts, params DbParameter[] commandParameters)
        {
            return ExecuteDataset(iDbHelper, transaction, CommandType.Text, commandTexts, commandParameters);
        }

        public static DataSet ExecuteDataset(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string[] commandTexts, params DbParameter[] commandParameters)
        {
            using (DbDataAdapter da = iDbHelper.DbDataAdapter())
            {
                try
                {
                    da.SelectCommand = iDbHelper.DbCommand();
                    da.SelectCommand.Connection = transaction.Connection;
                    da.SelectCommand.Transaction = transaction;
                    da.SelectCommand.CommandType = commandType;
                    da.SelectCommand.CommandTimeout = 240;
                    AttachParameters(da.SelectCommand, commandParameters);
                    for (int i = 0; i < commandTexts.Length - 1; i++)
                    {
                        da.SelectCommand.CommandText = commandTexts[i];
                        da.SelectCommand.ExecuteNonQuery();
                    }
                    da.SelectCommand.CommandText = commandTexts[commandTexts.Length - 1];
                    var dataSet = new DataSet();
                    da.Fill(dataSet);
                    return dataSet;
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, da.SelectCommand.CommandText, commandParameters);
                }
                finally
                {
                    da.SelectCommand.Parameters.Clear();
                }
            }
        }

        #endregion

        #region ExecuteReader

        /*public DbDataReader ExecuteReader(string commandText)
        {
            return ExecuteReader(CommandType.Text, commandText, new DbParameter[0]);
        }

        public DbDataReader ExecuteReader(CommandType commandType, string commandText)
        {
            return ExecuteReader(commandType, commandText, new DbParameter[0]);
        }*/

        public DbDataReader ExecuteReader(string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, commandText, commandParameters);
        }

        public DbDataReader ExecuteReader(CommandType commandType, string commandText,
                                          params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = _iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = _dbConn;
                    dbCom.CommandType = commandType;
                    dbCom.CommandText = commandText;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    return dbCom.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, commandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        /*public DbDataReader ExecuteReader(string[] commandTexts)
        {
            return ExecuteReader(CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public DbDataReader ExecuteReader(CommandType commandType, string[] commandTexts)
        {
            return ExecuteReader(commandType, commandTexts, new DbParameter[0]);
        }*/

        public DbDataReader ExecuteReader(string[] commandTexts, params DbParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, commandTexts, commandParameters);
        }

        public DbDataReader ExecuteReader(CommandType commandType, string[] commandTexts,
                                          params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = _iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = _dbConn;
                    dbCom.CommandType = commandType;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    for (int i = 0; i < commandTexts.Length - 1; i++)
                    {
                        dbCom.CommandText = commandTexts[i];
                        dbCom.ExecuteNonQuery();
                    }
                    dbCom.CommandText = commandTexts[commandTexts.Length - 1];
                    return dbCom.ExecuteReader(CommandBehavior.CloseConnection);
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, dbCom.CommandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        /*public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, string commandText)
        {
            return ExecuteReader(iDbHelper, transaction, CommandType.Text, commandText, new DbParameter[0]);
        }

        public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType,
                                          string commandText)
        {
            return ExecuteReader(iDbHelper, transaction, commandType, commandText, new DbParameter[0]);
        }*/

        public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, string commandText,
                                          params DbParameter[] commandParameters)
        {
            return ExecuteReader(iDbHelper, transaction, CommandType.Text, commandText, commandParameters);
        }

        public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType,
                                          string commandText,
                                          params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = transaction.Connection;
                    dbCom.Transaction = transaction;
                    dbCom.CommandType = commandType;
                    dbCom.CommandText = commandText;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    return dbCom.ExecuteReader();
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, commandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        /*public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts)
        {
            return ExecuteReader(iDbHelper, transaction, CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType,
                                          string[] commandTexts)
        {
            return ExecuteReader(iDbHelper, transaction, commandType, commandTexts, new DbParameter[0]);
        }*/

        public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts,
                                          params DbParameter[] commandParameters)
        {
            return ExecuteReader(iDbHelper, transaction, CommandType.Text, commandTexts, commandParameters);
        }

        public static DbDataReader ExecuteReader(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType,
                                          string[] commandTexts,
                                          params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = transaction.Connection;
                    dbCom.Transaction = transaction;
                    dbCom.CommandType = commandType;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    for (int i = 0; i < commandTexts.Length - 1; i++)
                    {
                        dbCom.CommandText = commandTexts[i];
                        dbCom.ExecuteNonQuery();
                    }
                    dbCom.CommandText = commandTexts[commandTexts.Length - 1];
                    return dbCom.ExecuteReader();
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, dbCom.CommandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        #endregion

        #region ExecuteScalar

        /*public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(CommandType.Text, commandText, new DbParameter[0]);
        }

        public object ExecuteScalar(CommandType commandType, string commandText)
        {
            return ExecuteScalar(commandType, commandText, new DbParameter[0]);
        }*/

        public object ExecuteScalar(string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, commandText, commandParameters);
        }

        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = _iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = _dbConn;
                    dbCom.CommandType = commandType;
                    dbCom.CommandText = commandText;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    return dbCom.ExecuteScalar();
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, commandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        /*public object ExecuteScalar(string[] commandTexts)
        {
            return ExecuteScalar(CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public object ExecuteScalar(CommandType commandType, string[] commandTexts)
        {
            return ExecuteScalar(commandType, commandTexts, new DbParameter[0]);
        }*/

        public object ExecuteScalar(string[] commandTexts, params DbParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, commandTexts, commandParameters);
        }

        public object ExecuteScalar(CommandType commandType, string[] commandTexts, params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = _iDbHelper.DbCommand())
            {
                try
                {
                    dbCom.Connection = _dbConn;
                    dbCom.CommandType = commandType;
                    dbCom.CommandTimeout = 240;
                    AttachParameters(dbCom, commandParameters);
                    for (int i = 0; i < commandTexts.Length - 1; i++)
                    {
                        dbCom.CommandText = commandTexts[i];
                        dbCom.ExecuteNonQuery();
                    }
                    dbCom.CommandText = commandTexts[commandTexts.Length - 1];
                    return dbCom.ExecuteScalar();
                }
                catch (Exception er)
                {
                    throw new ObException(er, commandType, dbCom.CommandText, commandParameters);
                }
                finally
                {
                    dbCom.Parameters.Clear();
                }
            }
        }

        /*public static object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, string commandText)
        {
            return ExecuteScalar(iDbHelper, transaction, CommandType.Text, commandText, new DbParameter[0]);
        }

        public object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteScalar(iDbHelper, transaction, commandType, commandText, new DbParameter[0]);
        }*/

        public static object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, string commandText,
                                    params DbParameter[] commandParameters)
        {
            return ExecuteScalar(iDbHelper, transaction, CommandType.Text, commandText, commandParameters);
        }

        public static object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string commandText,
                                    params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = iDbHelper.DbCommand())
            {
                dbCom.Connection = transaction.Connection;
                dbCom.Transaction = transaction;
                dbCom.CommandType = commandType;
                dbCom.CommandText = commandText;
                dbCom.CommandTimeout = 240;
                AttachParameters(dbCom, commandParameters);
                return dbCom.ExecuteScalar();
            }
        }

        /*public static object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts)
        {
            return ExecuteScalar(iDbHelper, transaction, CommandType.Text, commandTexts, new DbParameter[0]);
        }

        public object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string[] commandTexts)
        {
            return ExecuteScalar(iDbHelper, transaction, commandType, commandTexts, new DbParameter[0]);
        }*/

        public static object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, string[] commandTexts,
                                    params DbParameter[] commandParameters)
        {
            return ExecuteScalar(iDbHelper, transaction, CommandType.Text, commandTexts, commandParameters);
        }

        public static object ExecuteScalar(IDbHelper iDbHelper, DbTransaction transaction, CommandType commandType, string[] commandTexts,
                                    params DbParameter[] commandParameters)
        {
            using (DbCommand dbCom = iDbHelper.DbCommand())
            {
                dbCom.Connection = transaction.Connection;
                dbCom.Transaction = transaction;
                dbCom.CommandType = commandType;
                dbCom.CommandTimeout = 240;
                AttachParameters(dbCom, commandParameters);
                for (int i = 0; i < commandTexts.Length - 1; i++)
                {
                    dbCom.CommandText = commandTexts[i];
                    dbCom.ExecuteNonQuery();
                }
                dbCom.CommandText = commandTexts[commandTexts.Length - 1];
                return dbCom.ExecuteScalar();
            }
        }

        #endregion
    }
}