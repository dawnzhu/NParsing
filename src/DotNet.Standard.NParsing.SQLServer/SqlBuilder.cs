/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2009-08-25 20:51:34
* 版 本 号：1.0.0
* 功能说明：通过对象生成各种SQL语句
* ----------------------------------
* 修改标识：增加功能
* 修 改 人：朱晓春
* 日    期：2009-08-28 14:30:00
* 版 本 号：1.0.0
* 修改内容：增加了生成获取记录数的功能 CountSelect()
* ----------------------------------
* 修改标识：增加功能
* 修 改 人：朱晓春
* 日    期：2009-09-01 15:30:00
* 版 本 号：1.0.0
* 修改内容：增加了以参数方式增删改查数据库
* ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2010-03-12 09:00:00
* 版 本 号：1.0.1
* 修改内容：修改了命名空间(Zhuxc.Standard.Hibernate.SQLServer->DotNet.Standard.Transport.SQLServer)
* ----------------------------------
* 修改标识：修改功能
* 修 改 人：朱晓春
* 日    期：2010-03-31 09:20:00
* 版 本 号：1.0.2
* 修改内容：类名CreateSQL -> SqlBuilder，去掉CreateDbConnection方法，改成非静态类
* ----------------------------------
* 修改标识：修改功能
* 修 改 人：朱晓春
* 日    期：2010-03-31 21:30:00
* 版 本 号：1.0.2
* 修改内容：Insert Delete Update语句
* ----------------------------------
* 修改标识：修改功能
* 修 改 人：朱晓春
* 日    期：2010-08-01 09:10:00
* 版 本 号：2.1.0
* 修改内容：增加自动序列功能
* ----------------------------------
* 修改标识：修改功能
* 修 改 人：朱晓春
* 日    期：2010-08-04 08:30:00
* 版 本 号：2.1.0
* 修改内容：纠正不同表中有相同字段名，内联查询时信息填充有误问题
* ----------------------------------
* 修改标识：修改功能
* 修 改 人：朱晓春
* 日    期：2010-08-05 17:21:00
* 版 本 号：2.1.0
* 修改内容：增加多表联合查询功能、可为对象模型增加固定属性功能、增加多字段排序分页功能
 * ----------------------------------
* 修改标识：修改
* 修 改 人：朱晓春
* 日    期：2012-04-23 16:20:00
* 版 本 号：2.3.0
* 修改内容：遵循SQL执行计划规则，当参数是字符串时，必需指定长度，默认指定为nvarchar(max)
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Standard.NParsing.SQLServer
{
    public class SqlBuilder : ISqlBuilder
    {
        #region 生成创建库语句

        public string CreateDatabase(string name)
        {
            return
                string.Format(
                    "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name=N'{0}')\nBEGIN\nCREATE DATABASE {0}\nSELECT 1\nEND",
                    name);
        }

        #endregion

        #region 生成删除库语句

        public string DropDatabase(string name)
        {
            return
                string.Format(
                    "IF EXISTS (SELECT name FROM sys.databases WHERE name=N'{0}')\nBEGIN\nDROP DATABASE {0}\nSELECT 1\nEND",
                    name);
        }

        #endregion
    }

    public class SqlBuilder<TModel> : SqlBuilderBase<TModel>, ISqlBuilder<TModel>
    {
        public SqlBuilder(IObRedefine iObRedefine, IList<string> notJoinModels) : base(iObRedefine, notJoinModels)
        {
            ObRedefine = iObRedefine;
            JoinModels = null;
        }

        /// <summary>
        /// 允许关联
        /// </summary>
        public IList<string> JoinModels { get; set; }

        public IObRedefine ObRedefine { get; }

        protected override bool IsJoin(string tableName)
        {
            if (JoinModels != null)
            {
                return JoinModels.Contains(tableName);
            }
            return base.IsJoin(tableName);
        }

        #region 生成创建表语句

        public string CreateTable()
        {
            return CreateTable(null);
        }

        public string CreateTable(string name)
        {
            var tableName = string.IsNullOrEmpty(name) ? TableName : name;
            var sqlReturn = "";
            var primaryKeyColumns = "";
            var alterTable = "";
            var indexs = new Dictionary<string, string[]>();
            foreach (PropertyInfo property in ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.PropertyType.IsSystem())
                {
                    var columnName = property.ToColumnName(out var length, out var precision, out var nullable);
                    var columnSql = columnName + "\t";
                    columnSql += string.Format(property.PropertyType.ToDbTypeString(),
                                               length <= 0 ? "MAX" : length.ToString(), precision);
                    var settled = false;
                    foreach (var attribute in property.GetCustomAttributes(true))
                    {
                        if (attribute is ObIdentityAttribute identityAttribute)
                        {
                            var obIdentityAttribute = identityAttribute;
                            if (obIdentityAttribute.ObIdentity == ObIdentity.Database)
                            {
                                columnSql += $"\tIDENTITY({obIdentityAttribute.Seed},{obIdentityAttribute.Increment})";
                            }
                        }
                        else if (attribute is ObSettledAttribute)
                        {
                            settled = true;
                        }
                        else if (attribute is ObConstraintAttribute obConstraintAttribute)
                        {
                            if (obConstraintAttribute.ObConstraint == ObConstraint.PrimaryKey ||
                                obConstraintAttribute.ObConstraint ==
                                (ObConstraint.PrimaryKey | ObConstraint.ForeignKey))
                            {
                                if (primaryKeyColumns.Length > 0)
                                    primaryKeyColumns += ",";
                                primaryKeyColumns += columnName;
                                nullable = false;
                            }
                            if (obConstraintAttribute.ObConstraint == ObConstraint.ForeignKey ||
                                obConstraintAttribute.ObConstraint ==
                                (ObConstraint.PrimaryKey | ObConstraint.ForeignKey))
                            {
                                var fTableName = obConstraintAttribute.Refclass.ToTableName(TableNames);
                                var fColumnName = "";
                                foreach (var propertyInfo in obConstraintAttribute.Refclass.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                                {
                                    if (property.PropertyType.IsSystem() &&
                                        propertyInfo.Name == obConstraintAttribute.Refproperty)
                                    {
                                        fColumnName = propertyInfo.ToColumnName();
                                        break;
                                    }
                                }
                                alterTable +=
                                    string.Format(
                                        "ALTER TABLE {0}\nADD CONSTRAINT FK_{0}_{1}_{2}_{3} FOREIGN KEY ({1})\nREFERENCES {2} ({3})\n",
                                        tableName, columnName, fTableName, fColumnName);
                            }
                        }
                        else if (attribute is ObIndexAttribute obIndexAttribute)
                        {
                            if (indexs.ContainsKey(obIndexAttribute.Name))
                            {
                                indexs[obIndexAttribute.Name][0] +=
                                    $",\n{columnName} {(obIndexAttribute.Sort == Sort.Ascending ? "ASC" : "DESC")}";
                            }
                            else
                            {
                                indexs.Add(obIndexAttribute.Name, new[]{
                                    $"{columnName} {(obIndexAttribute.Sort == Sort.Ascending ? "ASC" : "DESC")}", obIndexAttribute.FileGroup});
                            }
                        }
                    }
                    if (settled) continue;
                    columnSql += nullable ? "\tNULL" : "\tNOT NULL";
                    if (sqlReturn.Length > 0)
                        sqlReturn += ",\n";
                    sqlReturn += columnSql;
                }
            }
            sqlReturn =
                string.Format(
                    "IF NOT EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id('{0}') AND type='U')\nBEGIN\nCREATE TABLE {0} (\n{1}\n",
                    tableName, sqlReturn);
            if (primaryKeyColumns.Length > 0)
            {
                sqlReturn += $",\nCONSTRAINT PK_{tableName} PRIMARY KEY ({primaryKeyColumns})";
            }
            sqlReturn += "\n)\n";
            sqlReturn += alterTable;
            foreach (var index in indexs)
            {
                sqlReturn +=
                    $"CREATE INDEX {index.Key} ON {tableName} (\n{index.Value[0]}\n) WITH(ONLINE = ON, DATA_COMPRESSION = PAGE){(string.IsNullOrEmpty(index.Value[1]) ? "" : " ON [" + index.Value[1] + "]")}\n";
            }
            sqlReturn += "SELECT 1\nEND\n";
            return sqlReturn;
        }

        #endregion

        #region 生成删除表语句

        public string DropTable()
        {
            return string.Format(
                    "IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id('{0}') AND type='U')\nBEGIN\nDROP TABLE {0}\nSELECT 1\nEND",
                    TableName);
        }

        #endregion

        #region 生成创建表索引语句

        public string CreateIndex(string name, IObSort iObSort, string fileGroup)
        {
            return string.Format(
                    "IF EXISTS (SELECT 1 FROM sysobjects WHERE id = object_id('{0}') AND type='U') AND\n" +
                    " NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'{0}') AND name = N'{1}')\n" +
                    "BEGIN\n" +
                    "CREATE INDEX [{1}] ON {0}\n" +
                    "(\n" +
                    "{2}\n" +
                    ") WITH(ONLINE = ON, DATA_COMPRESSION = PAGE){3}\n" +
                    "SELECT 1\n" +
                    "END",
                    TableName, name, iObSort.ToString(),
                    string.IsNullOrEmpty(fileGroup) ? "" : " ON [" + fileGroup + "]");
        }

        #endregion

        #region 生成删除表索引语句

        public string DropIndex(string name)
        {
            return string.Format(
                    "IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'{0}') AND name = N'{1}')\n" +
                    "BEGIN\n" +
                    "DROP INDEX [{1}] ON {0} WITH (ONLINE = OFF)\n" +
                    "SELECT 1\n" +
                    "END",
                    TableName, name);
        }

        #endregion

        #region 生成Insert语句

        public string[] Insert(IList<TModel> models, ref IList<DbParameter> dbParameters)
        {
            var sqllist = new List<string>();
            foreach (var model in models)
            {
                var list = Insert(model, ref dbParameters, false);
                sqllist.AddRange(list);
            }
            return sqllist.ToArray();
        }

        public string[] Insert(TModel model, ref IList<DbParameter> dbParameters)
        {
            return Insert(model, ref dbParameters, true);
        }

        private string[] Insert(TModel model, ref IList<DbParameter> dbParameters, bool isSingle)
        {
            //Type t = model.GetType();
            //model的表名
            //string tableName = t.ToTableName();
            //返回sql字段串
            var sqlReturn = new List<string>();
            //标识列名
            string sqlIdentityColumn = string.Empty;
            //标识列值
            string sqlIdentityParameter = string.Empty;
            //普通列名
            string sqlColumns = string.Empty;
            //普通列参数
            string sqlParameters = string.Empty;
            //标识列个数
            int identityCount = 0;
            //取何种方式标识
            ObIdentity obIdentity = ObIdentity.Database;
            //增长种子
            long seed = 1;
            //增长量
            int increment = 1;
            var iObModel = model as IObModel;
            foreach (PropertyInfo property in ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = property.GetValue(model, null);
                if (!property.PropertyType.IsSystem())
                    continue;
                //TODO ToColumnName
                var strColumnName = property.ToColumnName();
                var strParameterName = "@" + strColumnName;
                if (!isSingle)
                {
                    var id = dbParameters.Count(obj => obj.ParameterName.Contains(strParameterName));
                    if (id > 0)
                    {
                        strParameterName += id;
                    }
                }

                #region 自动编号,查找固定列

                var identity = false;
                var settled = false;
                foreach (var attribute in property.GetCustomAttributes(true))
                {
                    if (attribute is ObIdentityAttribute identityAttribute)
                    {
                        identity = true;
                        var obIdentityAttribute = identityAttribute;
                        obIdentity = obIdentityAttribute.ObIdentity;
                        seed = obIdentityAttribute.Seed;
                        increment = obIdentityAttribute.Increment;
                    }
                    else if (attribute is ObSettledAttribute)
                    {
                        settled = true;
                    }
                }

                if (settled) continue;
                if (identity)
                {
                    identityCount++;
                    if (identityCount > 1)
                        throw new Exception($"为表 '{TableName}' 指定了多个标识列。只允许为每个表指定一个标识列。");

                    sqlIdentityColumn = strColumnName;
                    sqlIdentityParameter = $"ISNULL(MAX({strColumnName}), {seed - increment})+{increment}";
                    continue;
                }

                #endregion

                //判断属性是否有效
                if (iObModel != null && !iObModel.IsPropertyValid(property.Name))
                    continue;
                if (sqlColumns.Length != 0)
                {
                    sqlColumns += ",";
                    sqlParameters += ",";
                }
                sqlColumns += strColumnName;
                sqlParameters += strParameterName;
                var sqlParameter = new SqlParameter { ParameterName = strParameterName };
                if (property.PropertyType.IsString())
                {
                    var vv = value?.ToString();
                    sqlParameter.SqlDbType = vv.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                    sqlParameter.Size = vv?.Length ?? 1;
                    if (sqlParameter.Size == 0)
                        sqlParameter.Size = 1;
                }
                else if (property.PropertyType.IsEnum())
                {
                    value = Convert.ToDecimal(value);
                }
                sqlParameter.Value = value ?? DBNull.Value;
                dbParameters.Add(sqlParameter);
            }
            if (identityCount != 0)
            {
                if (obIdentity == ObIdentity.Program)
                {
                    sqlReturn.Add(string.Format("INSERT INTO {0}({3}, {1}) (SELECT {4}, {2} FROM {0})", TableName, sqlColumns, sqlParameters, sqlIdentityColumn, sqlIdentityParameter));
                    if (isSingle)
                    {
                        sqlReturn.Add(string.Format("SELECT MAX({1}) FROM {0}", TableName, sqlIdentityColumn));
                    }
/*                    sqlReturn = string.Format("INSERT INTO {0}({3}, {1}) (SELECT {4}, {2} FROM {0})\n", tableName, sqlColumns, sqlParameters, sqlIdentityColumn, sqlIdentityParameter);
                    sqlReturn += string.Format("SELECT MAX({1}) FROM {0}", tableName, sqlIdentityColumn);*/
                }
                else
                {
                    sqlReturn.Add($"INSERT INTO {TableName}({sqlColumns}) VALUES({sqlParameters})");
                    if (isSingle)
                    {
                        sqlReturn.Add("SELECT @@IDENTITY");
                    }
/*                    sqlReturn = string.Format("INSERT INTO {0}({1}) VALUES({2})", t.ToTableName(), sqlColumns, sqlParameters);
                    sqlReturn += "\nSELECT @@IDENTITY";*/
                }
            }
            else
            {
                sqlReturn.Add($"INSERT INTO {TableName}({sqlColumns}) VALUES({sqlParameters})");
                //sqlReturn = string.Format("INSERT INTO {0}({1}) VALUES({2})", t.ToTableName(), sqlColumns, sqlParameters);
            }
            return sqlReturn.ToArray();
        }

        #endregion

        #region 生成Delete语句

        public string Delete(ref IList<DbParameter> dbParameters)
        {
            return Delete(null, ref dbParameters);
        }

        public string Delete(IObParameter iObParameter, ref IList<DbParameter> dbParameters)
        {
            string sql = $"DELETE FROM {TableName}";
            if (iObParameter != null)
            {
                string strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    sql += " WHERE " + strWhere;
                }
            }
            return sql;
        }

        #endregion

        #region 生成Update语句

        public string Update(TModel model, ref IList<DbParameter> dbParameters)
        {
            return Update(model, null, ref dbParameters);
        }

        public string Update(TModel model, IObParameter iObParameter, ref IList<DbParameter> dbParameters)
        {
            //Type t = model.GetType();
            string strSet = string.Empty;
            var iObModel = model as IObModel;
            foreach (PropertyInfo property in ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //判断属性是否有效
                if(iObModel != null && !iObModel.IsPropertyValid(property.Name))
                    continue;
                object value = property.GetValue(model, null);
                if (property.PropertyType.IsSystem())
                {
                    //TODO ToColumnName
                    string strColumnName = property.ToColumnName();
                    string strParameterName = "@" + strColumnName;

                    #region 查找不可修改列,标识列,查找固定列

                    var identity = false;
                    var settled = false;
                    var modifiable = true;
                    foreach (var attribute in property.GetCustomAttributes(true))
                    {
                        if (attribute is ObPropertyAttribute pattr)
                        {
                            modifiable = pattr.Modifiable;
                        }
                        else if(attribute is ObIdentityAttribute)
                        {
                            identity = true;
                        }
                        else if (attribute is ObSettledAttribute)
                        {
                            settled = true;
                        }
                    }

                    if (!modifiable || settled || identity) continue;

                    #endregion

                    strSet += strSet.Length == 0 ? "SET " : ",";
                    strSet += $"{TableName}.{strColumnName}={strParameterName}";
                    var sqlParameter = new SqlParameter { ParameterName = strParameterName };
                    if (property.PropertyType.IsString())
                    {
                        var vv = value?.ToString();
                        sqlParameter.SqlDbType = vv.IsChinese() ? SqlDbType.NVarChar : SqlDbType.VarChar;
                        sqlParameter.Size = vv?.Length ?? 1;
                        if (sqlParameter.Size == 0)
                            sqlParameter.Size = 1;
                    }
                    else if (property.PropertyType.IsEnum())
                    {
                        value = Convert.ToDecimal(value);
                    }
                    sqlParameter.Value = value ?? DBNull.Value;
                    dbParameters.Add(sqlParameter);
                }
            }
            string sql = $"UPDATE {TableName} {strSet}";
            if (iObParameter != null)
            {
                string strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    sql += " WHERE " + strWhere;
                }
            }
            return sql;
        }

        #endregion

        #region 生成Select语句

        #region Select记录数

        public string CountSelect(IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, ref IList<DbParameter> dbParameters)
        {
            string sql = ModelType.ToUTableName(TableName) + TableExtra;
            sql += JoinString(ModelType, ref dbParameters);
            if (iObParameter != null)
            {
                string strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    sql += " WHERE " + strWhere;
                }
            }
            if (iObGroup != null)
            {
                var sqlGroup = " GROUP BY " + iObGroup.ToString(ref dbParameters, out var columns, out _);
                sql = "SELECT COUNT(*) FROM (SELECT " + columns + " FROM " + sql + sqlGroup + ") " + TableName;
                if (iObParameter2 != null)
                {
                    sql += " WHERE " + iObParameter2.ToString('_', ref dbParameters);
                }
            }
            else
            {
                sql = "SELECT COUNT(*) FROM " + sql;
            }
            return sql;
        }

        #endregion

        #region Select记录是否存在

        public string ExistsSelect(IObParameter iObParameter, ref IList<DbParameter> dbParameters)
        {
            string sql = "SELECT TOP 1 1 FROM " + ModelType.ToUTableName(TableName) + TableExtra;
            sql += JoinString(ModelType, ref dbParameters);
            if (iObParameter != null)
            {
                string strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    sql += " WHERE " + strWhere;
                }
            }
            return sql;
        }

        #endregion

        #region Select记录

        public string Select(int? topSize, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters,
            out IList<string> columnNames)
        {
            string sqlGroup = string.Empty;
            string innerJoin = JoinString(ModelType, ref dbParameters, out var columns, out columnNames);
            if (iObGroup != null)
                sqlGroup = iObGroup.ToString(ref dbParameters, out columns, out columnNames);

            var sql = "SELECT ";
            if (topSize.HasValue)
            {
                sql += "TOP (@TopRow) ";
                TopParameters(topSize.Value, ref dbParameters);
            }
            sql += $"{columns} FROM {ModelType.ToUTableName(TableName) + TableExtra} {innerJoin}";
            if (iObParameter != null)
            {
                string strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    sql += " WHERE " + strWhere;
                }
            }
            if (!string.IsNullOrEmpty(sqlGroup))
            {
                sql += " GROUP BY " + sqlGroup;
                if (iObParameter2 != null)
                {
                    sql = "SELECT * FROM (" + sql + ") " + TableName + " WHERE " +
                          iObParameter2.ToString('_', ref dbParameters);
                }
            }
            if (iObSort != null)
            {
                sql += " ORDER BY ";
                if (!string.IsNullOrEmpty(sqlGroup))
                    sql += iObSort.ToString('_');
                else
                    sql += iObSort.ToString();
            }
            return sql;
        }

        public void TopParameters(int topSize, ref IList<DbParameter> dbParameters)
        {
            dbParameters.Add(new SqlParameter("@TopRow", SqlDbType.Int) { Value = topSize });
        }

        #endregion

        #region Select函数首行首列

        public string Select(IObProperty iObProperty, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters)
        {
            string sql;
            if (iObGroup != null)
            {
                var innerJoin = JoinString(ModelType, ref dbParameters, out var columns, out _);
                var sqlGroup = iObGroup.ToString(ref dbParameters, out columns, out _);
                columns = Regex.Replace(columns, @"\sAS\s[^_]+_[^,]+", "");
                sql = $"SELECT {columns} FROM {ModelType.ToUTableName(TableName) + TableExtra} {innerJoin}";
                if (iObParameter != null)
                {
                    string strWhere = iObParameter.ToString(ref dbParameters);
                    if (strWhere.Length > 0)
                    {
                        sql += " WHERE " + strWhere;
                    }
                }
                if (!string.IsNullOrEmpty(sqlGroup))
                    sql += " GROUP BY " + sqlGroup;
                sql = $"SELECT TOP 1 {iObProperty.ToString()} FROM ({sql}) {TableName}";
                if (iObParameter2 != null)
                {
                    sql += " WHERE " + iObParameter2.ToString('_', ref dbParameters);
                }
                if (iObSort != null)
                    sql += " ORDER BY " + iObSort.ToString();
            }
            else
            {
                sql = "SELECT TOP 1 " + iObProperty.ToString() + " FROM " + ModelType.ToUTableName(TableName) + TableExtra;
                sql += JoinString(ModelType, ref dbParameters);
                if (iObParameter != null)
                {
                    string strWhere = iObParameter.ToString(ref dbParameters);
                    if (strWhere.Length > 0)
                    {
                        sql += " WHERE " + strWhere;
                    }
                }
                if (iObSort != null)
                    sql += " ORDER BY " + iObSort.ToString();
            }
            return sql;
        }

        #endregion

        #region Select分页记录

        /// <summary>
        /// 生成分页SELECT语句
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="iObParameter">条件参数</param>
        /// <param name="iObGroup">分组</param>
        /// <param name="iObParameter2"></param>
        /// <param name="iObSort">排序</param>
        /// <param name="dbParameters">转成数据库参数</param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public string Select(int pageSize, int pageIndex, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters, out IList<string> columnNames)
        {
            PageParameters(pageSize, pageIndex, ref dbParameters);
            string innerJoin = JoinString(ModelType, ref dbParameters, out var columns, out columnNames);
            string strWhere = string.Empty;
            if (iObParameter != null)
            {
                strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    strWhere = "WHERE " + strWhere;
                }
            }
            var table = ModelType.ToUTableName(TableName);
            string sql;
            if (iObGroup != null)
            {
                var sqlGroup = iObGroup.ToString(ref dbParameters, out columns, out columnNames);
                if (!string.IsNullOrEmpty(sqlGroup))
                    sqlGroup = " GROUP BY " + sqlGroup;
                var cols = string.Empty;
                foreach (var columnName in columnNames)
                {
                    if (cols.Length > 0)
                        cols += ",";
                    cols += columnName;
                }
                table = $"(SELECT {columns} FROM {table} {innerJoin} {strWhere}{sqlGroup}) {TableName} ORDER BY {iObSort.ToString('_')}";
                sql = $"SELECT {cols} FROM {table} OFFSET @StartRow ROWS FETCH NEXT @SizeRow ROWS ONLY";
            }
            else
            {
                sql = $"SELECT {columns} FROM {table} {innerJoin} {strWhere} ORDER BY {iObSort.ToString()} OFFSET @StartRow ROWS FETCH NEXT @SizeRow ROWS ONLY";
            }
            return sql;
        }

        /// <summary>
        /// 分页参数
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="dbParameters"></param>
        public void PageParameters(int pageSize, int pageIndex, ref IList<DbParameter> dbParameters)
        {
            dbParameters.Add(new SqlParameter("@SizeRow", SqlDbType.Int) { Value = pageSize });
            dbParameters.Add(new SqlParameter("@StartRow", SqlDbType.Int) { Value = pageSize * (pageIndex - 1) });
        }

        /* 改成OFFSET,FETCH方式分页（最低支持SQL Server 2012）
        /// <summary>
        /// 生成分页SELECT语句
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="iObParameter">条件参数</param>
        /// <param name="iObGroup">分组</param>
        /// <param name="iObParameter2"></param>
        /// <param name="iObSort">排序</param>
        /// <param name="dbParameters">转成数据库参数</param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public string Select(int pageSize, int pageIndex, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters, out IList<string> columnNames)
        {
            PageParameters(pageSize, pageIndex, ref dbParameters);
            var dbSort = iObSort.List[0];
            string innerJoin = JoinString(ModelType, ref dbParameters, out var columns, out columnNames);
            string strWhereAnd = " WHERE ";
            string strWhere = string.Empty;
            if (iObParameter != null)
            {
                strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    strWhere = "WHERE (" + strWhere + ")";
                    strWhereAnd = " AND ";
                }
            }
            string sql;
            var table = ModelType.ToUTableName(TableName) + TableExtra;

            if (iObSort.List.Count > 1 || iObGroup != null)
            {
                const string sortName = "RowOrder";
                if (iObGroup != null)
                {
                    var sqlGroup = iObGroup.ToString(ref dbParameters, out columns, out columnNames);
                    if (!string.IsNullOrEmpty(sqlGroup))
                        sqlGroup = " GROUP BY " + sqlGroup;
                    var strWhere2 = "";
                    if (iObParameter2 != null)
                    {
                        strWhere2 = " WHERE " + iObParameter2.ToString('_', ref dbParameters);
                    }
                    table = $"(SELECT *,ROW_NUMBER() OVER (ORDER BY {iObSort.ToString('_')}) AS {sortName} FROM (SELECT {columns} FROM {table} {innerJoin} {strWhere}{sqlGroup})T{strWhere2}) {TableName}";
                }
                else
                {
                    table = $"(SELECT {columns},ROW_NUMBER() OVER (ORDER BY {iObSort.ToString()}) AS {sortName} FROM {table} {innerJoin} {strWhere}) {TableName}";
                }
                sql = $"SELECT * FROM {table} WHERE {sortName} BETWEEN @StartRow AND @EndRow";
            }
            else
            {
                var sortName = $"{dbSort.TableName}.{dbSort.ColumnName}";
                var sql1 = $"SELECT TOP (@TopRow) {columns} FROM {table} {innerJoin} {strWhere}";
                var sql2 = $"SELECT TOP (@StartRow) {sortName} FROM {table} {innerJoin} {strWhere}";
                if (pageIndex == 1)
                {
                    sql = sql1 + " ORDER BY " + iObSort.ToString();
                }
                else
                {
                    if (dbSort.IsAsc)
                    {
                        sql = sql1 + strWhereAnd + sortName + ">=(SELECT MAX(" + sortName + ") FROM (" + sql2 +
                              " ORDER BY " + sortName + ") " + dbSort.TableName + ") ORDER BY " + sortName;
                    }
                    else
                    {
                        sql = sql1 + strWhereAnd + sortName + "<=(SELECT MIN(" + sortName + ") FROM (" + sql2 +
                              " ORDER BY " + sortName + " DESC) " + dbSort.TableName + ") ORDER BY " + sortName + " DESC";
                    }
                }
            }
            return sql;
        }

        /// <summary>
        /// 分页参数
        /// </summary>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">第几页</param>
        /// <param name="dbParameters"></param>
        public void PageParameters(int pageSize, int pageIndex, ref IList<DbParameter> dbParameters)
        {
            dbParameters.Add(new SqlParameter("@TopRow", SqlDbType.Int) { Value = pageSize });
            dbParameters.Add(new SqlParameter("@StartRow", SqlDbType.Int) { Value = pageSize * (pageIndex - 1) + 1 });
            dbParameters.Add(new SqlParameter("@EndRow", SqlDbType.Int) { Value = pageSize * pageIndex });
        }*/

        #endregion

        #endregion

        #region 生成多表JOIN语句

        private string JoinString(Type parent, ref IList<DbParameter> dbParameters)
        {
            return JoinString(parent, null, ref dbParameters, /*new Dictionary<string, int> { { parent.ToTableName(TableNames), 1 } }, */out _, out _);
        }

        private string JoinString(Type parent, ref IList<DbParameter> dbParameters, out string columns, out IList<string> columnNames)
        {
            return JoinString(parent, null, ref dbParameters, /*new Dictionary<string, int> { { parent.ToTableName(TableNames), 1 } }, */out columns, out columnNames);
        }

        private string JoinString(Type parent, string tableName, ref IList<DbParameter> dbParameters, /*IDictionary<string, int> tableNames, */out string columns, out IList<string> columnNames)
        {
            var sql = string.Empty;
            columns = string.Empty;
            var childColumns = string.Empty;
            columnNames = new List<string>();
            var childColumnNames = new List<string>();
            foreach (var property in parent.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var child = property.PropertyType;
                if (!child.IsSystem())
                {
                    var newTableName = property.Name;
                    var parentTableName = tableName ?? parent.ToTableName(TableNames);
                    newTableName = parentTableName + "_" + newTableName;
                    if (!IsJoin(newTableName))
                        continue;
                    var foreignkeyNames = ForeignkeyName(parent, property, ref dbParameters);
                    if (foreignkeyNames.Count > 0)
                    {
                        var innerJoin = string.Empty;
                        /*if (tableNames.ContainsKey(newTableName))
                        {
                            var index = tableNames[newTableName] + 1;
                            tableNames[newTableName] = index;
                            newTableName += index;
                        }
                        else
                        {
                            tableNames.Add(newTableName, 1);
                        }*/
                        var childTableName = property.ToTableName(TableNames, out var childTableExtra) + " " + newTableName;
                        foreach (var foreignkeyName in foreignkeyNames)
                        {
                            var ctname = foreignkeyName.ChildColumnName.StartsWith("@")
                                ? foreignkeyName.ChildColumnName
                                : $"{newTableName}.{foreignkeyName.ChildColumnName}";
                            var ptname = foreignkeyName.ParentColumnName.StartsWith("@")
                                ? foreignkeyName.ParentColumnName
                                : $"{parentTableName}.{foreignkeyName.ParentColumnName}";
                            if (innerJoin.Length == 0)
                            {
                                innerJoin += $" {foreignkeyName.JoinMode} JOIN \n{childTableName + childTableExtra} ON {ctname}={ptname}";
                            }
                            else
                            {
                                innerJoin += $" AND {ctname}={ptname}";
                            }
                            /*if (innerJoin.Length == 0)
                                innerJoin += string.Format(" {5} JOIN \n{0} ON {1}.{2}={3}.{4}",
                                                           childTableName + childTableExtra, newTableName, foreignkeyName.ChildColumnName, parentTableName, foreignkeyName.ParentColumnName, foreignkeyName.JoinMode);
                            else
                                innerJoin += string.Format(" AND {0}.{1}={2}.{3}",
                                                           newTableName, foreignkeyName.ChildColumnName, parentTableName, foreignkeyName.ParentColumnName);*/
                        }
                        sql += innerJoin;
                        sql += JoinString(child, newTableName, ref dbParameters, /*tableNames, */out var cols, out var colNames);
                        if (childColumns.Length > 0 && cols.Length > 0)
                            childColumns += ",";
                        childColumns += cols;
                        foreach (var colName in colNames)
                        {
                            childColumnNames.Add(colName);
                        }
                    }
                }
                else
                {
                    if (columns.Length > 0)
                        columns += ",";

                    var obSettledValue = property.GetSettledValue();
                    //var obSettledValue = property.GetCustomAttributes(typeof(ObSettledAttribute), true).Select(psAttribute => ((ObSettledAttribute)psAttribute).Value).FirstOrDefault();
                    //TODO ToColumnName
                    tableName = tableName ?? parent.ToTableName(TableNames);
                    var columnName = property.ToColumnName();
                    var propertyName = property.Name;
                    //var isForeignkey = fkeyNames.Any(fkeyName => fkeyName.Value == columnName);
                    if (obSettledValue == null)
                        columns += $"{tableName}.{columnName} AS {tableName}_{propertyName}";
                    else
                    {
                        columns += $"{obSettledValue} AS {tableName}_{propertyName}";
                        /*if (child.IsEnum())
                        {
                            columns += string.Format("{0} AS {1}_{2}", Convert.ToDecimal(obSettledValue), tableName, propertyName);
                        }
                        else
                        {
                            columns += string.Format("'{0}' AS {1}_{2}", obSettledValue, tableName, propertyName);
                        }*/
                    }
                    columnNames.Add($"{tableName}_{propertyName}");
                }
            }
            if (columns.Length > 0 && childColumns.Length > 0)
                columns += ",";
            columns += childColumns;
            foreach (var childColumnName in childColumnNames)
            {
                columnNames.Add(childColumnName);
            }
            return sql;
        }

        private static IList<DbJoin> ForeignkeyName(Type parent, PropertyInfo childProperty, ref IList<DbParameter> dbParameters)
        {
            var child = childProperty.PropertyType;
            var foreignkeyNames = new List<DbJoin>();//new Dictionary<string, string>();
            foreach (PropertyInfo pParent in parent.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var isForeignKey = true;
                var key = false;
                var pNullable = true;
                string keyColumnName = null;
                string property = null;
                ObSettledAttribute pSetValueAttribute = null;
                foreach (var attribute in pParent.GetCustomAttributes(true))
                {
                    pSetValueAttribute = attribute as ObSettledAttribute;
                    if (attribute is ObConstraintAttribute constraintAttribute)
                    {
                        isForeignKey = (constraintAttribute.ObConstraint & ObConstraint.ForeignKey) == ObConstraint.ForeignKey;
                        if (constraintAttribute.Refclass == null)
                            key = true;
                        if ((constraintAttribute.Refclass != null && constraintAttribute.Refproperty != null) &&
                            //支持多外建绑定
                            constraintAttribute.Refclass == childProperty.PropertyType &&
                            (constraintAttribute.Property == null ||
                                (constraintAttribute.Property != null && constraintAttribute.Property == childProperty.Name)))
                        {
                            key = true;
                            keyColumnName = constraintAttribute.Refclass.Name + "." + constraintAttribute.Refproperty;
                            property = constraintAttribute.Property;
                        }
                    }

                    if (attribute is ObPropertyAttribute propertyAttribute)
                    {
                        pNullable = propertyAttribute.Nullable;
                    }
                }
                if (!key) continue;
                if (property != null && property != childProperty.Name) continue;
                foreach (PropertyInfo pChild in child.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    //与子类是主键的字段对比
                    var parentColumnName = pParent.ToColumnName();
                    var childColumnName = pChild.ToColumnName(); 
                    if (pSetValueAttribute != null)
                    {
                        dbParameters = dbParameters.Add(ref parentColumnName, pSetValueAttribute.Value);
                    }
                    //如果当外键字段定义中的主键名称为空时，并且父类外键字段名称和子类主键字段名称相等时
                    //如果当外键字段定义中的主键名称不为空时，并且外键字段定义中的主键名称和子类主键字段名称相等时
                    //以上两个条件任何满足一条，表示为主外键关联字段
                    if ((keyColumnName == null && parentColumnName != childColumnName) ||
                        (keyColumnName != null && keyColumnName != (child.Name + "." + pChild.Name))) continue;
                    key = false;
                    var cNullable = false;
                    foreach (var attribute in pChild.GetCustomAttributes(true))
                    {
                        if (attribute is ObSettledAttribute setValueAttribute)
                        {
                            dbParameters = dbParameters.Add(ref childColumnName, setValueAttribute.Value);
                        }

                        if (attribute is ObConstraintAttribute constraintAttribute)
                        {
                            if (isForeignKey)
                            {
                                key = (constraintAttribute.ObConstraint & ObConstraint.PrimaryKey) == ObConstraint.PrimaryKey;
                            }
                            else
                            {
                                key = (constraintAttribute.ObConstraint & ObConstraint.ForeignKey) == ObConstraint.ForeignKey;
                            }
                        }

                        if (attribute is ObPropertyAttribute propertyAttribute)
                        {
                            cNullable = propertyAttribute.Nullable;
                        }
                    }
                    if (!key) continue;
                    var dbJoin = new DbJoin
                    {
                        ParentColumnName = parentColumnName,
                        ChildColumnName = childColumnName
                    };
                    if (isForeignKey)
                    {
                        if (pNullable && !cNullable)
                            dbJoin.JoinMode = "LEFT";
                        else if (!pNullable && cNullable)
                            dbJoin.JoinMode = "RIGHT";
                        else
                            dbJoin.JoinMode = "INNER";
                    }
                    else
                    {
                        if (pNullable && !cNullable)
                            dbJoin.JoinMode = "RIGHT";
                        else if (!pNullable && cNullable)
                            dbJoin.JoinMode = "LEFT";
                        else
                            dbJoin.JoinMode = "INNER";
                    }
                    foreignkeyNames.Add(dbJoin);
                    break;
                }
            }
            return foreignkeyNames;
        }

        #endregion
    }
}