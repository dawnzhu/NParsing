﻿/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2010-09-27 12:47:56
* 版 本 号：2.2.0
* 功能说明：数据库查询接口实现
* ----------------------------------
* 修改标识：
* 修 改 人：
* 日    期：
* 版 本 号：
* 修改内容：
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObQuery<TModel> : IObQuery<TModel> where TModel : class, new()
    {
        private readonly IDbHelper _iDbHelper;
        protected readonly ISqlBuilder<TModel> SqlBuilder;
        private readonly string _providerName;
        private readonly IObTransaction _iObTransaction;
        private  IObJoin _iObJoin;
        private readonly string _key;
        private readonly string _sqlExtra;
        private readonly string _sqlVersion;
        public bool CreateEmptyObject { get; set; }
        public IObParameter ObParameter { get; set; }
        public IObParameter ObGroupParameter { get; set; }
        public IObGroup ObGroup { get; set; }
        public IObSort ObSort { get; set; }
        public IObJoin ObJoin
        {
            get => _iObJoin;
            set
            {
                _iObJoin = value;
                if (value != null)
                {
                    SqlBuilder.JoinModels = value.JoinModels;
                }
            }
        }

        public ObQuery(IDbHelper iDbHelper, ISqlBuilder<TModel> iSqlBuilder, string providerName, IObJoin iJoin)
            : this(iDbHelper, iSqlBuilder, providerName, null, iJoin, null, null, null, null, true)
        {
        }

        public ObQuery(IDbHelper iDbHelper, ISqlBuilder<TModel> iSqlBuilder, string providerName,
            IObTransaction iObTransaction, IObJoin iJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObGroupParameter, IObSort iObSort)
            : this(iDbHelper, iSqlBuilder, providerName, iObTransaction, iJoin, iObParameter, iObGroup, iObGroupParameter, iObSort, true)
        { 
        }

        public ObQuery(IDbHelper iDbHelper, ISqlBuilder<TModel> iSqlBuilder, string providerName,
            IObTransaction iObTransaction, IObJoin iJoin, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObGroupParameter, IObSort iObSort, bool createEmptyObject)
        {
            PageSize = 0;
            _iDbHelper = iDbHelper;
            SqlBuilder = iSqlBuilder;
            ObJoin = iJoin;
            _providerName = providerName;
            _iObTransaction = iObTransaction;
            ObParameter = iObParameter;
            ObGroupParameter = iObGroupParameter;
            ObGroup = iObGroup;
            ObSort = iObSort;
            CreateEmptyObject = createEmptyObject;
            _key = typeof(TModel).ToTableName(iSqlBuilder.ObRedefine.Models, out _sqlExtra, out _sqlVersion) + "|";
            if(iObParameter != null)
                _key += iObParameter.Key + "|";
            if(iObGroup != null)
                _key += iObGroup.Key + "|";
        }

        public TModel ToModel()
        {
            var models = ToList((int?) 1);
            if (models != null && models.Count == 1)
                return models[0];
            return null;
        }

        public IList<TModel> ToList()
        {
            return ToList(null);
        }

        public IList<TModel> ToList(int topSize)
        {
            return ToList((int?)topSize);
        }

        private IList<TModel> ToList(int? topSize)
        {
            #region 并行获取分页并组合

            if ((!topSize.HasValue || topSize.Value > PageSize) && PageSize > 0)
            {
                var count = Count();
                var list = new List<TModel>();
                var len = count / PageSize;
                if (count % PageSize > 0)
                {
                    len++;
                }
                var po = new ParallelOptions();
                if (MaxDegreeOfParallelism > 0)
                {
                    po.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
                }
                System.Threading.Tasks.Parallel.For(0, len, po, i =>
                {
                    var tl = ToList(PageSize, i + 1, out _);
                    while (list.Count < i * PageSize)
                    {
                        Thread.Sleep(100);
                    }
                    lock (list)
                    {
                        list.AddRange(tl);
                    }
                });
                return list;
                /*int count;
                var list = ToList(PageSize, 1, out count);
                if (topSize.HasValue)
                {
                    count = topSize.Value;
                }
                if (count > PageSize)
                {
                    var pageCount = count / PageSize;
                    var lastPageSize = count % PageSize;
                    var indexs = new List<int>();
                    for (var i = 2; i < pageCount + 1; i++)
                    {
                        indexs.Add(i);
                    }
                    var alist = indexs.Select(i => new Action(() =>
                    {
                        int c;
                        var tl = ToList(PageSize, i, out c);
                        while (list.Count < (i - 1) * PageSize)
                        {
                            Thread.Sleep(100);
                        }
                        lock (list)
                        {
                            ((List<TModel>)list).AddRange(tl);
                        }
                    })).ToList();
                    if (lastPageSize > 0)
                    {
                        alist.Add(() =>
                        {
                            int c;
                            var tl = ToList(lastPageSize, pageCount + 1, out c);
                            while (list.Count < pageCount * PageSize)
                            {
                                Thread.Sleep(100);
                            }
                            lock (list)
                            {
                                ((List<TModel>)list).AddRange(tl);
                            }
                        });
                    }
                    if (MaxDegreeOfParallelism > 0)
                    {
                        System.Threading.Tasks.Parallel.Invoke(new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism }, alist.ToArray());
                    }
                    else
                    {
                        System.Threading.Tasks.Parallel.Invoke(alist.ToArray());
                    }
                }
                return list;*/
            }

            #endregion

            #region 生成Key

            var key = _key;
            if (ObSort != null)
                key += ObSort.Key + "|";
            key += "ToList";
            if (topSize.HasValue)
            {
                key += "_Top";
            }

            #endregion

            #region 初始化对象

            IList<TModel> models;
            IObParameter iObParameter = null;
            IObParameter iObParameter2 = null;
            IObGroup iObGroup = null;
            IObSort iObSort = null;
            if (ObParameter != null)
            {
                iObParameter = ObConvert.ToObParameter(_providerName, ObParameter);
            }
            if (ObGroup != null)
            {
                iObGroup = ObConvert.ToObGroup(_providerName, ObGroup);
            }
            if (ObGroupParameter != null)
            {
                iObParameter2 = ObConvert.ToObParameter(_providerName, ObGroupParameter);
            }
            if (ObSort != null)
            {
                iObSort = ObConvert.ToObSort(_providerName, ObSort);
            }
            DbDataReader dr;

            #endregion

            #region 生成SQL

            string sql;
            IList<DbParameter> dbParameters = new List<DbParameter>();
            var sc = ObCache.Value(key);
            IList<string> columnNames;
            if (sc != null && sc.Version == _sqlVersion)
            {
                sql = sc.SqlText;
                columnNames = sc.ColumnNames;
                iObParameter?.ToString(ref dbParameters);
                if (topSize.HasValue)
                {
                    SqlBuilder.TopParameters(topSize.Value, ref dbParameters);
                }
            }
            else
            {
                sql = SqlBuilder.Select(topSize, iObParameter, iObGroup, iObParameter2, iObSort, ref dbParameters, out columnNames);
                ObCache.Add(key, new ObSqlcache { Version = _sqlVersion, SqlText = sql, ColumnNames = columnNames});
            }
#if DEBUG
            CurrentExeSql = (key, sql, dbParameters);
#endif

            #endregion

            #region 读取数据

            if (_iObTransaction != null)
            {
                dr = DbHelper.ExecuteReader(_iDbHelper, _iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray());
                models = dr.ToList<TModel>(SqlBuilder.ObRedefine.Models, columnNames, CreateEmptyObject);
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
                    dr = dbHelper.ExecuteReader(sql, ((List<DbParameter>)dbParameters).ToArray());
                    models = dr.ToList<TModel>(SqlBuilder.ObRedefine.Models, columnNames, CreateEmptyObject);
                }
            }

            return models;

            #endregion
        }

        public IList<TModel> ToList(int pageSize, int pageIndex, out int count)
        {
            #region 生成Key

            var key = _key;
            if (ObSort == null)
                throw new Exception("分页时，请设置排序方式");
            key += ObSort.Key + "|";
            key += "ToList_" + (pageIndex == 1 ? "Home" : "Other");
            //var key = _key + "ToList_" + pageSize + "_" + pageIndex;
            //var key = _key + "ToList_" + (pageIndex == 1 ? "Home" : "Other");

            #endregion

            #region 初始化对象

            IList<TModel> models;
            IObParameter iObParameter = null;
            IObParameter iObParameter2 = null;
            IObGroup iObGroup = null;
            IObSort iObSort = null;
            if (ObParameter != null)
            {
                iObParameter = ObConvert.ToObParameter(_providerName, ObParameter);
            }
            if (ObGroup != null)
            {
                iObGroup = ObConvert.ToObGroup(_providerName, ObGroup);
            }
            if (ObGroupParameter != null)
            {
                iObParameter2 = ObConvert.ToObParameter(_providerName, ObGroupParameter);
            }
            if (ObSort != null)
            {
                iObSort = ObConvert.ToObSort(_providerName, ObSort);
            }
            DbDataReader dr;

            #endregion

            #region 生成SQL

            string countsql;
            string sql;
            IList<DbParameter> dbParameters = new List<DbParameter>();
            var sc = ObCache.Value(_key + "Count");
            IList<string> columnNames;
            if (sc != null && sc.Version == _sqlVersion)
            {
                countsql = sc.SqlText;
                iObParameter?.ToString(ref dbParameters);
            }
            else
            {
                countsql = SqlBuilder.CountSelect(iObParameter, iObGroup, iObParameter2, ref dbParameters);
                ObCache.Add(_key + "Count", new ObSqlcache { Version = _sqlVersion, SqlText = countsql });
            }
            dbParameters = new List<DbParameter>();
            sc = ObCache.Value(key);
            if (sc != null && sc.Version == _sqlVersion)
            {
                sql = sc.SqlText;
                columnNames = sc.ColumnNames;
                iObParameter?.ToString(ref dbParameters);
                SqlBuilder.PageParameters(pageSize, pageIndex, ref dbParameters);
            }
            else
            {
                sql = SqlBuilder.Select(pageSize, pageIndex, iObParameter, iObGroup, iObParameter2, iObSort, ref dbParameters, out columnNames);
                ObCache.Add(key, new ObSqlcache { Version = _sqlVersion, SqlText = sql, ColumnNames = columnNames });
            }
#if DEBUG
            CurrentExeSql = (key, sql, dbParameters);
#endif
            #endregion

            #region 读取数据

            if (_iObTransaction != null)
            {
                count = Convert.ToInt32(DbHelper.ExecuteScalar(_iDbHelper, _iObTransaction.DbTransaction, countsql, ((List<DbParameter>)dbParameters).ToArray()));
                dr = DbHelper.ExecuteReader(_iDbHelper, _iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray());
                models = dr.ToList<TModel>(SqlBuilder.ObRedefine.Models, columnNames, CreateEmptyObject);
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
                    count = Convert.ToInt32(dbHelper.ExecuteScalar(countsql, ((List<DbParameter>)dbParameters).ToArray()));
                    dr = dbHelper.ExecuteReader(sql, ((List<DbParameter>)dbParameters).ToArray());
                    models = dr.ToList<TModel>(SqlBuilder.ObRedefine.Models, columnNames, CreateEmptyObject);
                }
            }

            return models;

            #endregion
        }

        public DataTable ToTable()
        {
            return ToTable(null);
        }

        public DataTable ToTable(int topSize)
        {
            return ToTable((int?) topSize);
        }

        private DataTable ToTable(int? topSize)
        {
            #region 并行获取分页并组合

            if ((!topSize.HasValue || topSize.Value > PageSize) && PageSize > 0)
            {
                var table = ToTable(PageSize, 1, out var count);
                if (topSize.HasValue)
                {
                    count = topSize.Value;
                }
                if (count > PageSize)
                {
                    var pageCount = count / PageSize;
                    var lastPageSize = count % PageSize;
                    var indexs = new List<int>();
                    for (var i = 2; i < pageCount + 1; i++)
                    {
                        indexs.Add(i);
                    }
                    var alist = indexs.Select(i => new Action(() =>
                    {
                        var tt = ToTable(PageSize, i, out _);
                        while (table.Rows.Count < (i - 1) * PageSize)
                        {
                            Thread.Sleep(100);
                        }
                        lock (table)
                        {
                            table.Merge(tt);
                        }
                    })).ToList();
                    if (lastPageSize > 0)
                    {
                        alist.Add(() =>
                        {
                            var tt = ToTable(lastPageSize, pageCount + 1, out _);
                            while (table.Rows.Count < pageCount * PageSize)
                            {
                                Thread.Sleep(100);
                            }
                            lock (table)
                            {
                                table.Merge(tt);
                            }
                        });
                    }
                    System.Threading.Tasks.Parallel.Invoke(alist.ToArray());
                }
                return table;
            }

            #endregion

            #region 生成Key

            var key = _key;
            if (ObSort != null)
                key += ObSort.Key + "|";
            key += "ToList";
            if (topSize.HasValue)
            {
                key += "_Top";
            }

            #endregion

            #region 初始化对象

            DataTable dt;
            IObParameter iObParameter = null;
            IObParameter iObParameter2 = null;
            IObGroup iObGroup = null;
            IObSort iObSort = null;
            if (ObParameter != null)
            {
                iObParameter = ObConvert.ToObParameter(_providerName, ObParameter);
            }
            if (ObGroup != null)
            {
                iObGroup = ObConvert.ToObGroup(_providerName, ObGroup);
            }
            if (ObGroupParameter != null)
            {
                iObParameter2 = ObConvert.ToObParameter(_providerName, ObGroupParameter);
            }
            if (ObSort != null)
            {
                iObSort = ObConvert.ToObSort(_providerName, ObSort);
            }

            #endregion

            #region 生成SQL

            string sql;
            IList<DbParameter> dbParameters = new List<DbParameter>();
            var sc = ObCache.Value(key);
            if (sc != null && sc.Version == _sqlVersion)
            {
                sql = sc.SqlText;
                //columnNames = sc.ColumnNames;
                iObParameter?.ToString(ref dbParameters);
                if (topSize.HasValue)
                {
                    SqlBuilder.TopParameters(topSize.Value, ref dbParameters);
                }
            }
            else
            {
                sql = SqlBuilder.Select(topSize, iObParameter, iObGroup, iObParameter2, iObSort, ref dbParameters, out var columnNames);
                ObCache.Add(key, new ObSqlcache { Version = _sqlVersion, SqlText = sql, ColumnNames = columnNames });
            }
#if DEBUG
            CurrentExeSql = (key, sql, dbParameters);
#endif
            #endregion

            #region 读取数据

            if (_iObTransaction != null)
            {
                dt = DbHelper.ExecuteDataset(_iDbHelper, _iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray()).Tables[0];
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    dt = dbHelper.ExecuteDataset(sql, ((List<DbParameter>) dbParameters).ToArray()).Tables[0];
                }
            }

            return dt;

            #endregion
        }

        public DataTable ToTable(int pageSize, int pageIndex, out int count)
        {
            #region 生成Key

            var key = _key;
            if (ObSort == null)
                throw new Exception("分页时，请设置排序方式");
            key += ObSort.Key + "|";
            key += "ToList_" + (pageIndex == 1 ? "Home" : "Other");
            //var key = _key + "ToList_" + pageSize + "_" + pageIndex;
            //var key = _key + "ToList_" + (pageIndex == 1 ? "Home" : "Other");

            #endregion

            #region 初始化对象

            DataTable dt;
            IObParameter iObParameter = null;
            IObParameter iObParameter2 = null;
            IObGroup iObGroup = null;
            IObSort iObSort = null;
            if (ObParameter != null)
            {
                iObParameter = ObConvert.ToObParameter(_providerName, ObParameter);
            }
            if (ObGroup != null)
            {
                iObGroup = ObConvert.ToObGroup(_providerName, ObGroup);
            }
            if (ObGroupParameter != null)
            {
                iObParameter2 = ObConvert.ToObParameter(_providerName, ObGroupParameter);
            }
            if (ObSort != null)
            {
                iObSort = ObConvert.ToObSort(_providerName, ObSort);
            }

            #endregion

            #region 生成SQL

            string countsql;
            string sql;
            IList<DbParameter> dbParameters = new List<DbParameter>();
            var sc = ObCache.Value(_key + "Count");
            if (sc != null && sc.Version == _sqlVersion)
            {
                countsql = sc.SqlText;
                iObParameter?.ToString(ref dbParameters);
            }
            else
            {
                countsql = SqlBuilder.CountSelect(iObParameter, iObGroup, iObParameter2, ref dbParameters);
                ObCache.Add(_key + "Count", new ObSqlcache { Version = _sqlVersion, SqlText = countsql });
            }
            dbParameters = new List<DbParameter>();
            sc = ObCache.Value(key);
            if (sc != null && sc.Version == _sqlVersion)
            {
                sql = sc.SqlText;
                //columnNames = sc.ColumnNames;
                iObParameter?.ToString(ref dbParameters);
                SqlBuilder.PageParameters(pageSize, pageIndex, ref dbParameters);
            }
            else
            {
                sql = SqlBuilder.Select(pageSize, pageIndex, iObParameter, iObGroup, iObParameter2, iObSort, ref dbParameters, out var columnNames);
                ObCache.Add(key, new ObSqlcache { Version = _sqlVersion, SqlText = sql, ColumnNames = columnNames });
            }
#if DEBUG
            CurrentExeSql = (key, sql, dbParameters);
#endif
            #endregion

            #region 读取数据

            if (_iObTransaction != null)
            {
                count = Convert.ToInt32(DbHelper.ExecuteScalar(_iDbHelper, _iObTransaction.DbTransaction, countsql, ((List<DbParameter>)dbParameters).ToArray()));
                dt = DbHelper.ExecuteDataset(_iDbHelper, _iObTransaction.DbTransaction, sql, ((List<DbParameter>)dbParameters).ToArray()).Tables[0];
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    count = Convert.ToInt32(dbHelper.ExecuteScalar(countsql, ((List<DbParameter>)dbParameters).ToArray()));
                    dt = dbHelper.ExecuteDataset(sql, ((List<DbParameter>)dbParameters).ToArray()).Tables[0];
                }
            }

            return dt;

            #endregion
        }

        public object Scalar(IObProperty iObProperty)
        {
            #region 生成Key

            var key = _key;
            if (ObSort != null)
                key += ObSort.Key + "|";
            key += "Scalar_" + iObProperty.Key;

            #endregion

            #region 初始化对象

            object obj;
            iObProperty = ObConvert.ToObProperty(_providerName, iObProperty);
            IObParameter iObParameter = null;
            IObParameter iObParameter2 = null;
            IObGroup iObGroup = null;
            IObSort iObSort = null;
            if (ObParameter != null)
            {
                iObParameter = ObConvert.ToObParameter(_providerName, ObParameter);
            }
            if (ObGroup != null)
            {
                iObGroup = ObConvert.ToObGroup(_providerName, ObGroup);
            }
            if (ObGroupParameter != null)
            {
                iObParameter2 = ObConvert.ToObParameter(_providerName, ObGroupParameter);
            }
            if (ObSort != null)
            {
                iObSort = ObConvert.ToObSort(_providerName, ObSort);
            }
            IList<DbParameter> dbParameters = new List<DbParameter>();

            #endregion

            #region 生成SQL

            string sql;
            var sc = ObCache.Value(key);
            if (sc != null && sc.Version == _sqlVersion)
            {
                sql = sc.SqlText;
                iObParameter?.ToString(ref dbParameters);
            }
            else
            {
                sql = SqlBuilder.Select(iObProperty, iObParameter, iObGroup, iObParameter2, iObSort, ref dbParameters);
                ObCache.Add(key, new ObSqlcache { Version = _sqlVersion, SqlText = sql });
            }
#if DEBUG
            CurrentExeSql = (key, sql, dbParameters);
#endif
            #endregion

            #region 读取数据

            if (_iObTransaction != null)
            {
                obj = DbHelper.ExecuteScalar(_iDbHelper, _iObTransaction.DbTransaction, sql,
                    ((List<DbParameter>) dbParameters).ToArray());
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    obj = dbHelper.ExecuteScalar(sql, ((List<DbParameter>) dbParameters).ToArray());
                }
            }
            obj = obj != DBNull.Value && obj != null ? obj : null;

            return obj;

            #endregion
        }

        public bool Exists()
        {
            var key = _key + "Exists";

            #region 初始化对象

            bool b;
            IObParameter iObParameter = null;
            if (ObParameter != null)
            {
                iObParameter = ObConvert.ToObParameter(_providerName, ObParameter);
            }
            IList<DbParameter> dbParameters = new List<DbParameter>();

            #endregion

            #region 生成SQL

            string sql;
            var sc = ObCache.Value(key);
            if (sc != null && sc.Version == _sqlVersion)
            {
                sql = sc.SqlText;
                iObParameter?.ToString(ref dbParameters);
            }
            else
            {
                sql = SqlBuilder.ExistsSelect(iObParameter, ref dbParameters);
                ObCache.Add(key, new ObSqlcache { Version = _sqlVersion, SqlText = sql });
            }
#if DEBUG
            CurrentExeSql = (key, sql, dbParameters);
#endif
            #endregion

            #region 读取数据

            if (_iObTransaction != null)
            {
                b = DbHelper.Exists(_iDbHelper, _iObTransaction.DbTransaction, sql,
                    ((List<DbParameter>) dbParameters).ToArray());
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    b = dbHelper.Exists(sql, ((List<DbParameter>) dbParameters).ToArray());
                }
            }
            return b;

            #endregion
        }

        public int Count()
        {
            var key = _key + "Count";

            #region 初始化对象

            int count;
            IObParameter iObParameter = null;
            IObParameter iObParameter2 = null;
            IObGroup iObGroup = null;
            if (ObParameter != null)
            {
                iObParameter = ObConvert.ToObParameter(_providerName, ObParameter);
            }
            if (ObGroup != null)
            {
                iObGroup = ObConvert.ToObGroup(_providerName, ObGroup);
            }
            if (ObGroupParameter != null)
            {
                iObParameter2 = ObConvert.ToObParameter(_providerName, ObGroupParameter);
            }
            IList<DbParameter> dbParameters = new List<DbParameter>();

            #endregion

            #region 生成SQL

            string sql;
            var sc = ObCache.Value(key);
            if (sc != null && sc.Version == _sqlVersion)
            {
                sql = sc.SqlText;
                iObParameter?.ToString(ref dbParameters);
            }
            else
            {
                sql = SqlBuilder.CountSelect(iObParameter, iObGroup, iObParameter2, ref dbParameters);
                ObCache.Add(key, new ObSqlcache { Version = _sqlVersion, SqlText = sql });
            }
#if DEBUG
            CurrentExeSql = (key, sql, dbParameters);
#endif
            #endregion

            #region 读取数据

            if (_iObTransaction != null)
            {
                count = Convert.ToInt32(DbHelper.ExecuteScalar(_iDbHelper, _iObTransaction.DbTransaction, sql,
                    ((List<DbParameter>) dbParameters).ToArray()));
            }
            else
            {
                using (var dbHelper = new DbHelper(_iDbHelper))
                {
                    count = Convert.ToInt32(dbHelper.ExecuteScalar(sql, ((List<DbParameter>) dbParameters).ToArray()));
                }
            }
            return count;

            #endregion
        }

        public int PageSize { get; private set; }

        public int MaxDegreeOfParallelism { get; private set; }

        public (string Key, string Text, IList<DbParameter> Parameters) CurrentExeSql { get; private set; }

        public IObQuery<TModel> Parallel()
        {
            return Parallel(1000);
        }

        public IObQuery<TModel> Parallel(int pageSize)
        {
            return Parallel(pageSize, 0);
        }

        public IObQuery<TModel> Parallel(int pageSize, int maxDegreeOfParallelism)
        {
            PageSize = pageSize;
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            return this;
        }
    }
}
