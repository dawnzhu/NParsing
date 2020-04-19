/**
* 作    者：朱晓春(zhi_dian@163.com)
* 创建时间：2018-12-22 14.27:00
* 版 本 号：1.0.0
* 功能说明：创建
* ----------------------------------
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.Common.Utilities;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.SQLite
{
    public class SqlBuilder : ISqlBuilder
    {
        #region 生成创建库语句

        public string CreateDatabase(string name)
        {
            return $"IF NOT EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name='{name}')\nTHEN\nCREATE DATABASE {name};\nSELECT 1;\nEND IF;";
        }

        #endregion

        #region 生成删除库语句

        public string DropDatabase(string name)
        {
            return $"IF EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name='{name}')\nTHEN\nDROP DATABASE {name};\nSELECT 1;\nEND IF;";
        }

        #endregion
    }

    public class SqlBuilder<TModel> : SqlBuilderBase<TModel>, ISqlBuilder<TModel>
    {
        public SqlBuilder(IObRedefine iObRedefine, IList<string> notJoinModels)
             : base(iObRedefine, notJoinModels)
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
            long seed = 0;
            var increment = 0;
            var indexs = new Dictionary<string, string>();
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
                                columnSql += "\tAUTO_INCREMENT";
                                seed = obIdentityAttribute.Seed;
                                increment = obIdentityAttribute.Increment;
                            }
                        }
                        else if (attribute is ObSettledAttribute)
                        {
                            settled = true;
                            break;
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
                                alterTable += $"ALTER TABLE {tableName}\nADD CONSTRAINT FK_{tableName}_{columnName}_{fTableName}_{fColumnName} FOREIGN KEY ({columnName})\nREFERENCES {fTableName} ({fColumnName}) ON DELETE RESTRICT ON UPDATE RESTRICT;\n";
                            }
                        }
                        else if (attribute is ObIndexAttribute obIndexAttribute)
                        {
                            if (indexs.ContainsKey(obIndexAttribute.Name))
                            {
                                indexs[obIndexAttribute.Name] += $",\n{columnName}";
                            }
                            else
                            {
                                indexs.Add(obIndexAttribute.Name, columnName);
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
            sqlReturn = $"IF((SHOW TABLES LIKE '%{tableName}%') != 1) THEN\nCREATE TABLE {tableName} (\n{sqlReturn}\n";
            if (primaryKeyColumns.Length > 0)
            {
                sqlReturn += $",\nPRIMARY KEY ({primaryKeyColumns})";
            }
            sqlReturn += "\n)";
            if (increment > 0)
                sqlReturn += $"AUTO_INCREMENT={increment}";
            sqlReturn += ";";
            if (seed > 0)
                alterTable += $"ALTER TABLE {tableName} AUTO_INCREMENT={seed};\n";
            sqlReturn += alterTable;
            foreach (var index in indexs)
            {
                sqlReturn += $"CREATE INDEX {index.Key} ON {tableName} (\n{index.Value}\n);\n";
            }
            sqlReturn += "SELECT 1;\nEND IF;";
            return sqlReturn;
        }

        #endregion

        #region 生成删除表语句

        public string DropTable()
        {
            return string.Format("IF((SHOW TABLES LIKE '%{0}%') != 1) THEN\nDROP TABLE {0};\nSELECT 1;\nEND IF;",
                                 TableName);
        }

        #endregion

        public string CreateIndex(string name, IObSort iObSort, string fileGroup)
        {
            throw new NotImplementedException();
        }

        public string DropIndex(string name)
        {
            throw new NotImplementedException();
        }

        #region 生成Insert

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
                var strParameterName = "$" + strColumnName;
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
                if (value != null)
                {
                    if (value.IsEnum())
                        value = Convert.ToDecimal(value);
                    dbParameters.Add(new SQLiteParameter(strParameterName, value));
                }
                else
                    dbParameters.Add(new SQLiteParameter(strParameterName, DBNull.Value));
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
                        sqlReturn.Add("SELECT last_insert_rowid()");
                    }
/*                    sqlReturn = string.Format("INSERT INTO {0}({1}) VALUES({2})", t.ToTableName(), sqlColumns, sqlParameters);
                    sqlReturn += "\nSELECT last_insert_rowid()";*/
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
            string strJoin = JoinString(ModelType, ref dbParameters, out _, out _);
            string sql = $"DELETE FROM {ModelType.ToUTableName(TableName)} FROM {ModelType.ToUTableName(TableName)} {strJoin}";
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
            string strJoin = JoinString(ModelType, ref dbParameters, out _, out _);
            var iObModel = model as IObModel;
            foreach (PropertyInfo property in ModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                //判断属性是否有效
                if (iObModel != null && !iObModel.IsPropertyValid(property.Name))
                    continue;
                object value = property.GetValue(model, null);
                if (property.PropertyType.IsSystem())
                {
                    //TODO ToColumnName
                    string strColumnName = property.ToColumnName();
                    string strParameterName = "$" + strColumnName;

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
                        else if (attribute is ObIdentityAttribute)
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
                    strSet += $"{strColumnName}={strParameterName}";
                    if (value != null)
                    {
                        if (value.IsEnum())
                            value = Convert.ToDecimal(value);
                        dbParameters.Add(new SQLiteParameter(strParameterName, value));
                    }
                    else
                        dbParameters.Add(new SQLiteParameter(strParameterName, DBNull.Value));
                }
            }
            string sql = $"UPDATE {ModelType.ToUTableName(TableName)} {strSet} FROM {ModelType.ToUTableName(TableName)} {strJoin}";
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
            string sql = ModelType.ToUTableName(TableName);
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
                var sqlGroup = " GROUP BY " + iObGroup.ToString(ref dbParameters, out _, out _);
                sql = "SELECT COUNT(1) FROM (SELECT 1 AS C FROM " + sql + sqlGroup + ") " + TableName;
            }
            else
            {
                sql = "SELECT COUNT(1) FROM " + sql;
            }
            return sql;
        }

        #endregion

        #region Select记录是否存在

        public string ExistsSelect(IObParameter iObParameter, ref IList<DbParameter> dbParameters)
        {
            string sql = "SELECT 1 FROM " + ModelType.ToUTableName(TableName);
            sql += JoinString(ModelType, ref dbParameters);
            if (iObParameter != null)
            {
                string strWhere = iObParameter.ToString(ref dbParameters);
                if (strWhere.Length > 0)
                {
                    sql += " WHERE " + strWhere;
                }
            }
            return sql + " LIMIT 0,1";
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

            var sql = $"SELECT {columns} FROM {ModelType.ToUTableName(TableName)} ";
            if (topSize.HasValue)
            {
                sql += "LIMIT 0,$TopRow ";
                TopParameters(topSize.Value, ref dbParameters);
            }
            sql += innerJoin;
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
            dbParameters.Add(new SQLiteParameter("$TopRow", DbType.Int32) { Value = topSize });
        }

        #endregion

        #region Select函数首行首列

        public string Select(IObProperty iObProperty, IObParameter iObParameter, IObGroup iObGroup, IObParameter iObParameter2, IObSort iObSort, ref IList<DbParameter> dbParameters)
        {
            string sql;
            if (iObGroup != null)
            {
                string innerJoin = JoinString(ModelType, ref dbParameters, out var columns, out _);
                var sqlGroup = iObGroup.ToString(ref dbParameters, out columns, out _);
                columns = Regex.Replace(columns, @"\sAS\s[^_]+_[^,]+", "");
                sql = $"SELECT {columns} FROM {ModelType.ToUTableName(TableName)} {innerJoin}";
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
                sql = $"SELECT {iObProperty.ToString()} FROM ({sql}) {TableName} LIMIT 0,1";
                if (iObSort != null)
                    sql += " ORDER BY " + iObSort.ToString();
            }
            else
            {
                sql = "SELECT " + iObProperty.ToString() + " FROM " + ModelType.ToUTableName(TableName) + " LIMIT 0,1";
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
                table = $"(SELECT {columns} FROM {table} {innerJoin} {strWhere}{sqlGroup}) {TableName} ORDER BY {iObSort.ToString(columnNames)}";
                sql = $"SELECT {cols} FROM {table} LIMIT $StartRow,$SizeRow";
            }
            else
            {
                sql = $"SELECT {columns} FROM {table} {innerJoin} {strWhere} ORDER BY {iObSort.ToString()} LIMIT $StartRow,$SizeRow";
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
            dbParameters.Add(new SQLiteParameter("$SizeRow", DbType.Int32) { Value = pageSize });
            dbParameters.Add(new SQLiteParameter("$StartRow", DbType.Int32) { Value = pageSize * (pageIndex - 1) });
        }

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
                            var ctname = foreignkeyName.ChildColumnName.StartsWith("$")
                                ? foreignkeyName.ChildColumnName
                                : $"{newTableName}.{foreignkeyName.ChildColumnName}";
                            var ptname = foreignkeyName.ParentColumnName.StartsWith("$")
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
                        columns += string.Format("{0}.{1} AS {0}_{2}", tableName, columnName, propertyName);
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
