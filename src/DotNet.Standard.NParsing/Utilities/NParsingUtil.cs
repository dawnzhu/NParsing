using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.Utilities;

namespace DotNet.Standard.NParsing.Utilities
{
    public static class NParsingUtil
    {
        #region 获取数据表名 public static string ToTableName(this string typeName)

        public static string ToTableName(this Type modelType)
        {
            return ToTableName(modelType, null, out _, out _);
        }

        public static string ToTableName(this Type modelType, out string extra)
        {
            return ToTableName(modelType, null, out extra, out _);
        }

        public static string ToTableName(this Type modelType, IDictionary<string, string> tableNames)
        {
            return ToTableName(modelType, tableNames, out _, out _);
        }

        public static string ToTableName(this Type modelType, IDictionary<string, string> tableNames, out string extra)
        {
            return ToTableName(modelType, tableNames, out extra, out _);
        }

        /// <summary>
        /// 获取数据表名
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="extra"></param>
        /// <param name="tableNames"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string ToTableName(this Type modelType, IDictionary<string, string> tableNames, out string extra, out string version)
        {
            extra = "";
            version = "";
            var tableName = "";
            var obModelAttribute = (ObModelAttribute)modelType.GetCustomAttributes(typeof(ObModelAttribute), false).FirstOrDefault();
            if(obModelAttribute != null)
            {
                tableName = obModelAttribute.Name;
                extra = !string.IsNullOrEmpty(obModelAttribute.Extra) ? " " + obModelAttribute.Extra : "";
                version = obModelAttribute.Version ?? "";
            }
            if (string.IsNullOrEmpty(tableName))
                tableName = modelType.Name;
            if (tableNames != null && tableNames.ContainsKey(tableName))
            {
                tableName = tableNames[tableName];
            }
            return tableName;
        }

        public static string ToTableName(this PropertyInfo property)
        {
            return ToTableName(property, null, out _, out _);
        }

        public static string ToTableName(this PropertyInfo property, out string extra)
        {
            return ToTableName(property, null, out extra, out _);
        }

        public static string ToTableName(this PropertyInfo property, IDictionary<string, string> tableNames)
        {
            return ToTableName(property, tableNames, out _, out _);
        }

        public static string ToTableName(this PropertyInfo property, IDictionary<string, string> tableNames, out string extra)
        {
            return ToTableName(property, tableNames, out extra, out _);
        }

        /// <summary>
        /// 获取数据表名
        /// </summary>
        /// <param name="property"></param>
        /// <param name="extra"></param>
        /// <param name="tableNames"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static string ToTableName(this PropertyInfo property, IDictionary<string, string> tableNames, out string extra, out string version)
        {
            var obModelAttribute = (ObModelAttribute)property.GetCustomAttributes(typeof(ObModelAttribute), false).FirstOrDefault();
            if (obModelAttribute != null)
            {
                var tableName = obModelAttribute.Name;
                extra = !string.IsNullOrEmpty(obModelAttribute.Extra) ? " " + obModelAttribute.Extra : "";
                version = obModelAttribute.Version ?? "";
                if (tableNames != null && tableNames.ContainsKey(tableName))
                {
                    tableName = tableNames[tableName];
                }
                return tableName;
            }
            return ToTableName(property.PropertyType, tableNames, out extra, out version);
        }

        #endregion

        #region 获取数据字段名 public static string ToColumnName(this PropertyInfo propertyInfo)

        public static string ToColumnName(this PropertyInfo propertyInfo)
        {
            return ToColumnName(propertyInfo, out _, out _, out _);
        }

        public static string ToColumnName(this PropertyInfo propertyInfo, out int length, out int precision, out bool nullable)
        {
            var columnName = propertyInfo.Name;
            length = 0;
            precision = 0;
            nullable = false;
            var obPropertyAttribute = (ObPropertyAttribute)propertyInfo.GetCustomAttributes(typeof(ObPropertyAttribute), false).FirstOrDefault();
            if(obPropertyAttribute != null)
            {
                length = obPropertyAttribute.Length;
                precision = obPropertyAttribute.Precision;
                nullable = obPropertyAttribute.Nullable;
                if (!string.IsNullOrEmpty(obPropertyAttribute.Name))
                    columnName = obPropertyAttribute.Name;
            }
            return columnName;
        }

        #endregion

        #region 取联合查询时的表 public static string ToTableName(this Type modelType)

        /// <summary>
        /// 取联合查询时的表
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="baseName"> </param>
        /// <returns></returns>
        public static string ToUTableName(this Type modelType, string baseName)
        {
            foreach (var attribute in modelType.GetCustomAttributes(typeof(ObUnionAttribute), false))
            {
                var sql = string.Empty;
                var mps = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var obUnionAttribute = (ObUnionAttribute)attribute;
                foreach (var unionModelType in obUnionAttribute.ModelTypes)
                {
                    var columns = string.Empty;
                    var ps = unionModelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    for (var i = 0; i < ps.Length; i++)
                    {
                        if (!ps[i].PropertyType.IsSystem()) continue;
                        if (columns.Length > 0)
                            columns += ",";
                        var obSettledValue = ps[i].GetSettledValue();
                        //var obSettledValue = ps[i].GetCustomAttributes(typeof (ObSettledAttribute), false).Select(psAttribute => ((ObSettledAttribute) psAttribute).Value).FirstOrDefault();
                        //TODO ToColumnName()
                        if (obSettledValue == null)
                            columns += $"{unionModelType.ToTableName()}.{ps[i].ToColumnName()} AS {mps[i].ToColumnName()}";
                        else
                        {
                            columns += $"{obSettledValue} AS {mps[i].ToColumnName()}";
                            /*if (mps[i].PropertyType.IsEnum())
                            {
                                obSettledValue = Convert.ToDecimal(obSettledValue);
                                columns += string.Format("{0} AS {1}", obSettledValue, mps[i].ToColumnName());
                            }
                            else
                            {
                                columns += string.Format("'{0}' AS {1}", obSettledValue, mps[i].ToColumnName());
                            }*/
                        }
                    }
                    if (sql.Length > 0)
                        sql += "\n" + "UNION ALL" + "\n";
                    sql += $"SELECT {columns} FROM {unionModelType.ToTableName()}";
                }
                return "(" + sql + ") " + baseName;
/*                if(attribute is ObModelAttribute)
                {
                    var obModelAttribute = (ObModelAttribute) attribute;
                    if(!string.IsNullOrEmpty(obModelAttribute.Name))
                        return baseName ?? obModelAttribute.Name;
                    break;
                }*/
            }
            return baseName /*?? modelType.Name.ToTableName()*/;
        }

        #endregion

        #region 获取两个对象关联属性 public static IList<string> ForeignkeyName(this Type parent, Type child)

        /*public static IList<DbJoin> ForeignkeyName(this Type parent, PropertyInfo childProperty)
        {
            IList<DbParameter> dbParameters = new List<DbParameter>();
            return ForeignkeyName(parent, childProperty, ref dbParameters);
        }*/

        /// <summary>
        /// 获取两个对象关联属性
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childProperty"></param>
        /// <returns></returns>
        public static IList<DbJoin> ForeignkeyName(this Type parent, PropertyInfo childProperty)
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
                foreach (var attribute in pParent.GetCustomAttributes(true))
                {
                    if (attribute is ObConstraintAttribute constraintAttribute)
                    {
                        isForeignKey = (constraintAttribute.ObConstraint & ObConstraint.ForeignKey) == ObConstraint.ForeignKey;
                        if(constraintAttribute.Refclass == null)
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
/*                    if (!(attribute is ObConstraintAttribute) || ((ObConstraintAttribute)attribute).ObConstraint == ObConstraint.PrimaryKey) continue;
                    foreignKey = true;
                    primaryKeyColumnName = ((ObConstraintAttribute)attribute).PrimaryKey;
                    fkIsNull = ((ObConstraintAttribute)attribute).IsNull;
                    break;*/
                }
                if (!key) continue;
                if (property != null && property != childProperty.Name) continue;
                foreach (PropertyInfo pChild in child.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    //与子类是主键的字段对比
                    var parentColumnName = pParent.ToColumnName();
                    var childColumnName = pChild.ToColumnName();
                    //如果当外键字段定义中的主键名称为空时，并且父类外键字段名称和子类主键字段名称相等时
                    //如果当外键字段定义中的主键名称不为空时，并且外键字段定义中的主键名称和子类主键字段名称相等时
                    //以上两个条件任何满足一条，表示为主外键关联字段
                    if ((keyColumnName == null && parentColumnName != childColumnName) ||
                        (keyColumnName != null && keyColumnName != (child.Name + "." + pChild.Name))) continue;
                    key = false;
                    var cNullable = false;
                    foreach (var attribute in pChild.GetCustomAttributes(true))
                    {
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
/*                        if (!(attribute is ObConstraintAttribute) || ((ObConstraintAttribute) attribute).ObConstraint == ObConstraint.ForeignKey) continue;
                        primaryKey = true;
                        pkIsNull = ((ObConstraintAttribute)attribute).IsNull;
                        break;*/
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

        #region 填充数据对象 public static M ToModel<M>(this IDataReader di, IList<string> columnNames) where M : new()

        /// <summary>
        /// 填充数据对象集
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dr"></param>
        /// <param name="tableNames"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public static IList<TModel> ToList<TModel>(this IDataReader dr, IDictionary<string, string> tableNames, IList<string> columnNames) where TModel : new()
        {
            var list = new List<TModel>();
            var propertyInfos = typeof(TModel).GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var tableName = typeof(TModel).ToTableName(tableNames);
            var propertyNames = new Dictionary<string, string>();
            while (dr.Read())
            {
                var model = new TModel();
                if (!dr.IsClosed)
                {
                    DataFill(dr/*, tableNames*/, columnNames, model, propertyInfos, tableName, propertyNames/*, new Dictionary<string, int> { { tableName, 1 } }*/);
                }
                list.Add(model);
            }
            return list;
        }

        /// <summary>
        /// 填充数据对象
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="dr"></param>
        /// <param name="tableNames"></param>
        /// <param name="columnNames"></param>
        public static TModel ToModel<TModel>(this IDataReader dr, IDictionary<string, string> tableNames, IList<string> columnNames) where TModel : new()
        {
            var model = new TModel();
            if (!dr.IsClosed)
            {
                var propertyInfos = typeof(TModel).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var tableName = typeof(TModel).ToTableName(tableNames);
                var propertyNames = new Dictionary<string, string>();
                DataFill(dr/*, tableNames*/, columnNames, model, propertyInfos, tableName, propertyNames/*, new Dictionary<string, int> { { tableName, 1 } }*/);
            }
            return model;
        }

        /// <summary>
        /// 将数据流中的记录存储到对像中
        /// </summary>
        /// <param name="dr">数据</param>
        /// <param name="columnNames">字段集</param>
        /// <param name="model">对象</param>
        /// <param name="propertyInfos">对象属性集</param>
        /// <param name="tableName">当前表名</param>
        /// <param name="propertyNames"></param>
        private static void DataFill(IDataRecord dr/*, IDictionary<string, string> tableNames*/, IList<string> columnNames, object model, IEnumerable<PropertyInfo> propertyInfos, string tableName, IDictionary<string, string> propertyNames/*, IDictionary<string, int> sqlTableNames*/)
        {
            foreach (var property in propertyInfos)
            {
                try
                {
                    var oValue = property.GetValue(model, null);
                    var t = property.PropertyType.ToBasic();
                    if (t.IsSystem())
                    {
                       /* string propertyName;
                       var k = tableName + "_" + property.Name;
                        if (propertyNames.ContainsKey(k))
                        {
                            propertyName = propertyNames[k];
                        }
                        else
                        {
                            //TODO ToColumnName
                            propertyName = tableName + "_" + property.ToColumnName();
                            propertyNames.Add(k, propertyName);
                        }*/
                        var propertyName = tableName + "_" + property.Name;
                        object value = null;
                        var indexOf = columnNames.IndexOf(propertyName);
                        if (indexOf > -1 && dr[indexOf] != DBNull.Value)
                        {
                            value = t.IsEnum()
                                ? Enum.ToObject(t, dr[indexOf])
                                : Convert.ChangeType(dr[indexOf], t);
                        }
                        property.SetValue(model, value, null);
                    }
                    else if (!t.IsGenericType)
                    {
                        if (oValue == null)
                        {
                            property.SetValue(model, Activator.CreateInstance(t), null);
                            oValue = property.GetValue(model, null);
                        }
                        /*var tn = t.ToTableName(tableNames);
                        if (sqlTableNames.ContainsKey(tn))
                        {
                            var index = sqlTableNames[tn] + 1;
                            tn += index;
                            sqlTableNames[tn] = index;
                        }
                        else
                        {
                            sqlTableNames.Add(tn, 1);
                        }*/
                        var newTableName = tableName + "_" + property.Name;
                        DataFill(dr/*, tableNames*/, columnNames, oValue, t.GetProperties(BindingFlags.Instance | BindingFlags.Public), newTableName,
                            propertyNames/*, sqlTableNames*/);
                    }
                }
                catch (Exception er)
                {
                    throw new Exception(property.ToColumnName() + er.Message);
                }
            }
        }

        /// <summary>
        /// 填充数据对象
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="di"></param>
        /// <returns></returns>
        public static TModel ToModel<TModel>(this IDictionary di) where TModel : new()
        {
            var model = new TModel();
            DataFill(di, model);
            return model;
        }

        /// <summary>
        /// 将数据流中的记录存储到对像中
        /// </summary>
        /// <param name="di">数据</param>
        /// <param name="model">对象</param>
        private static void DataFill(IDictionary di, object model)
        {
            var modelType = model.GetType();
            foreach (PropertyInfo property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                try
                {
                    var oValue = property.GetValue(model, null);
                    var t = property.PropertyType.ToBasic();
                    if (t.IsSystem())
                    {
                        object value = null;
                        if (di?[property.Name] != null)
                        {
                            value = t.IsEnum()
                                ? Enum.ToObject(t, di[property.Name])
                                : Convert.ChangeType(di[property.Name], t);
                        }
                        property.SetValue(model, value, null);
                    }
                    else if(!t.IsGenericType)
                    {
                        if (oValue == null)
                        {
                            property.SetValue(model, Activator.CreateInstance(t), null);
                            oValue = property.GetValue(model, null);
                        }
                        DataFill((IDictionary) di[property.Name], oValue);
                    }
                }
                catch (Exception er)
                {
                    throw new Exception(property.Name + er.Message);
                }
            }
        }

        #endregion

        public static PropertyInfo ToPropertyInfo(this Type t, MethodBase method)
        {
            var propertyInfo = (
                from p in t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where p.GetGetMethod(true).Name == method.Name || p.GetSetMethod(true).Name == method.Name
                select p).FirstOrDefault();
            return propertyInfo;
        }

        public static string GetSettledValue(this PropertyInfo property)
        {
            string retValue = null;
            var obSettledValue = property.GetCustomAttributes(typeof(ObSettledAttribute), true).Select(psAttribute => ((ObSettledAttribute)psAttribute).Value).FirstOrDefault();
            if(obSettledValue != null)
            {
                if (property.PropertyType.IsEnum())
                {
                    retValue = Convert.ToDecimal(obSettledValue).ToString(CultureInfo.InvariantCulture);
                }
                else if(property.PropertyType == typeof(bool))
                {
                    retValue = Convert.ToBoolean(obSettledValue) ? "1" : "0";
                }
                else if (obSettledValue is string ||
                         obSettledValue is char ||
                         obSettledValue is DateTime)
                {
                    retValue = $"'{obSettledValue}'";
                }
                else
                {
                    retValue = obSettledValue.ToString();
                }
            }
            return retValue;
        }
    }
}