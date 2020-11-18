using System;
using System.Diagnostics;
using System.Reflection;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;
using DotNet.Standard.Common.Utilities;

namespace DotNet.Standard.NParsing.Factory
{
    public class ObProperty : ObPropertyBase
    {
        private const string ASSEMBLY_STRING = "DotNet.Standard.NParsing.DbUtilities";
        private const string CLASS_NAME = ASSEMBLY_STRING + ".ObParameter";
        /*public string TableName { get; private set; }
        public string ColumnName { get; private set; }
        public Type ParentType { get; private set; }*/

        private ObProperty(Type modelType, string tableName, string propertyName, DbFunc dbFunc = DbFunc.Null)
        {
            TableName = tableName;
            PropertyInfo propertyInfo;
            if (!modelType.Exists(propertyName, out propertyInfo))
            {
                throw new Exception(string.Format("{0}.{1}属性名未找到", TableName, propertyName));
            }
            ModelType = modelType;
            //TODO ToColumnName
            ColumnName = propertyInfo.ToColumnName();
            PropertyName = propertyName;
            DbFunc = dbFunc;
            AriSymbol = DbAriSymbol.Null;
            AsProperty = this;
        }

        internal ObProperty(IObProperty iObProperty)
        {
            TableName = iObProperty.TableName;
            ModelType = iObProperty.ModelType;
            ColumnName = iObProperty.ColumnName;
            PropertyName = iObProperty.PropertyName;
            DbFunc = iObProperty.DbFunc;
            AriSymbol = DbAriSymbol.Null;
            AsProperty = this;
        }

        public static ObProperty Create2<TModel>()
        {
            var method = new StackFrame(1, false).GetMethod();
            var propertyInfo = typeof(TModel).ToPropertyInfo(method);
            return new ObProperty(typeof(TModel), typeof(TModel).ToTableName(), propertyInfo.Name);
        }

        public static ObProperty Create2<TModel>(string modelName)
        {
            var method = new StackFrame(1, false).GetMethod();
            var propertyInfo = typeof(TModel).ToPropertyInfo(method);
            return new ObProperty(typeof(TModel), modelName, propertyInfo.Name);
        }

        public static ObProperty Create2<TModel>(IObRedefine iObRedefine)
        {
            var method = new StackFrame(1, false).GetMethod();
            var propertyInfo = typeof(TModel).ToPropertyInfo(method);
            return iObRedefine == null
                ? new ObProperty(typeof (TModel), typeof (TModel).ToTableName(), propertyInfo.Name)
                : new ObProperty(typeof (TModel), typeof (TModel).ToTableName(iObRedefine.Models), propertyInfo.Name);
        }

        public static ObProperty Create<TModel>(MethodBase currentMethod)
        {
            var propertyInfo = typeof(TModel).ToPropertyInfo(currentMethod);
            return new ObProperty(typeof(TModel), typeof(TModel).ToTableName(), propertyInfo.Name);
        }

        public static ObProperty Create<TModel>(string modelName, MethodBase currentMethod)
        {
            var propertyInfo = typeof(TModel).ToPropertyInfo(currentMethod);
            return new ObProperty(typeof(TModel), modelName, propertyInfo.Name);
        }

        public static ObProperty Create<TModel>(IObRedefine iObRedefine, MethodBase currentMethod)
        {
            var propertyInfo = typeof (TModel).ToPropertyInfo(currentMethod);
            return iObRedefine == null 
                ? new ObProperty(typeof(TModel), typeof(TModel).ToTableName(), propertyInfo.Name) 
                : new ObProperty(typeof(TModel), typeof(TModel).ToTableName(iObRedefine.Models), propertyInfo.Name);
        }

        internal static ObProperty Create(Type mt, IObRedefine iObRedefine, MethodBase currentMethod)
        {
            var propertyInfo = mt.ToPropertyInfo(currentMethod);
            return iObRedefine == null
                ? new ObProperty(mt, mt.ToTableName(), propertyInfo.Name)
                : new ObProperty(mt, mt.ToTableName(iObRedefine.Models), propertyInfo.Name);
        }

        /*public static ObProperty Create<TModel>(PropertyInfo propertyInfo)
        {
            return new ObProperty(typeof(TModel), typeof(TModel).ToTableName(), propertyInfo.Name);
        }

        public static ObProperty Create<TModel>(string modelName, PropertyInfo propertyInfo)
        {
            return new ObProperty(typeof(TModel), modelName, propertyInfo.Name);
        }

        public static ObProperty Create<TModel>(IObRedefine iObRedefine, PropertyInfo propertyInfo)
        {
            return new ObProperty(typeof(TModel), typeof(TModel).ToTableName(iObRedefine.Models), propertyInfo.Name);
        }*/

        public static ObProperty Create<TModel>(string propertyName)
        {
            return new ObProperty(typeof(TModel), typeof(TModel).ToTableName(), propertyName);
        }

        public static ObProperty Create<TModel>(string modelName, string propertyName)
        {
            return new ObProperty(typeof(TModel), modelName, propertyName);
        }

        public static ObProperty Create<TModel>(IObRedefine iObRedefine, string propertyName)
        {
            return iObRedefine == null
                ? new ObProperty(typeof(TModel), typeof(TModel).ToTableName(), propertyName)
                : new ObProperty(typeof(TModel), typeof(TModel).ToTableName(iObRedefine.Models), propertyName);
        }

        public static ObProperty Create(Type mt, IObRedefine iObRedefine, string propertyName)
        {
            return iObRedefine == null
                ? new ObProperty(mt, mt.ToTableName(), propertyName)
                : new ObProperty(mt, mt.ToTableName(iObRedefine.Models), propertyName);
        }

        #region 创建算术运算

        public static ObProperty operator +(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.Add;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.Add, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }

        public static ObProperty operator -(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.Subtract;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.Subtract, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }

        public static ObProperty operator *(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.Multiply;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.Multiply, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }

        public static ObProperty operator /(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.Divide;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.Divide, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }
        public static ObProperty operator %(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.Modulo;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.Modulo, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }

        public static ObProperty operator &(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.And;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.And, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }

        public static ObProperty operator |(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.Or;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.Or, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }

        public static ObProperty operator ^(ObProperty obProperty, object value)
        {
            if (value is ObProperty property)
            {
                obProperty.Brothers.Add(property);
                property.AriSymbol = DbAriSymbol.ExclusiveOr;
            }
            else
            {
                var obValue = new ObValue(DbAriSymbol.ExclusiveOr, value);
                obProperty.Brothers.Add(obValue);
            }
            return obProperty;
        }

        public static ObProperty operator ~(ObProperty obProperty)
        {
            var obValue = new ObValue(DbAriSymbol.Not, null);
            obProperty.Brothers.Add(obValue);
            return obProperty;
        }

        public static ObProperty operator -(ObProperty obProperty)
        {
            var obValue = new ObValue(DbAriSymbol.Negate, null);
            obProperty.Brothers.Add(obValue);
            return obProperty;
        }

        public static ObProperty operator <<(ObProperty obProperty, int value)
        {
            var obValue = new ObValue(DbAriSymbol.LeftShift, value);
            obProperty.Brothers.Add(obValue);
            return obProperty;
        }

        public static ObProperty operator >>(ObProperty obProperty, int value)
        {
            var obValue = new ObValue(DbAriSymbol.RightShift, value);
            obProperty.Brothers.Add(obValue);
            return obProperty;
        }

        #endregion

        #region 与值比较的条件创建

        public static ObParameterBase operator ==(ObProperty obProperty, object value)
        {
            var t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            if (value != DBNull.Value && value != null)
            {
                var parameters = new[]
                                     {
                                         obProperty,
                                         DbSymbol.Equal,
                                         value
                                     };
                return (ObParameterBase)Activator.CreateInstance(t, parameters);
            }
            else
            {
                var parameters = new object[]
                                     {
                                         obProperty,
                                         DbValue.IsNull
                                     };
                return (ObParameterBase)Activator.CreateInstance(t, parameters);
            }
        }

        //public static IObParameter operator ==(ObProperty obProperty, object[] value)
        //{
        //    Type tp = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
        //    var parameters = new[]
        //                             {
        //                                 obProperty.TableName,
        //                                 obProperty.ColumnName,
        //                                 DbSymbol.In,
        //                                 (object)value
        //                             };
        //    return (IObParameter)Activator.CreateInstance(tp, parameters);
        //}

        public static ObParameterBase operator !=(ObProperty obProperty, object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            if (value != DBNull.Value && value != null)
            {
                var parameters = new[]
                                     {
                                         obProperty,
                                         DbSymbol.NotEqual,
                                         value
                                     };
                return (ObParameterBase)Activator.CreateInstance(t, parameters);
            }
            else
            {
                var parameters = new object[]
                                     {
                                         obProperty,
                                         DbValue.IsNotNull
                                     };
                return (ObParameterBase)Activator.CreateInstance(t, parameters);
            }
        }

        //public static IObParameter operator !=(ObProperty obProperty, object[] value)
        //{
        //    Type tp = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
        //    var parameters = new[]
        //                             {
        //                                 obProperty.TableName,
        //                                 obProperty.ColumnName,
        //                                 DbSymbol.NotIn,
        //                                 (object)value
        //                             };
        //    return (IObParameter)Activator.CreateInstance(tp, parameters);
        //}

        public static ObParameterBase operator >(ObProperty obProperty, object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     obProperty,
                                     DbSymbol.Than,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public static ObParameterBase operator <(ObProperty obProperty, object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     obProperty,
                                     DbSymbol.Less,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public static ObParameterBase operator >=(ObProperty obProperty, object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     obProperty,
                                     DbSymbol.ThanEqual,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public static ObParameterBase operator <=(ObProperty obProperty, object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     obProperty,
                                     DbSymbol.LessEqual,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase Like(object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.Like,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase LikeLeft(object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.LikeLeft,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase LikeRight(object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.LikeRight,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase In<T>(params T[] values)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.In,
                                     (object)values
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotIn<T>(params T[] values)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.NotIn,
                                     (object)values
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotLike(object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.NotLike,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotLikeLift(object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.NotLikeLeft,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotLikeRight(object value)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.NotLikeRight,
                                     value
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase Between(object fromValue, object toValue)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.Between,
                                     (object)new []{fromValue, toValue}
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotBetween(object fromValue, object toValue)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new[]
                                 {
                                     this,
                                     DbSymbol.NotBetween,
                                     (object)new []{fromValue, toValue}
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        #endregion

        #region 内部字段间比较的条件创建

        public static ObParameterBase operator ==(ObProperty obProperty, ObProperty obProperty2)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            if (obProperty2 != null)
            {
                var parameters = new object[]
                                 {
                                     obProperty,
                                     DbSymbol.Equal,
                                     obProperty2
                                 };
                return (ObParameterBase)Activator.CreateInstance(t, parameters);
            }
            else
            {
                var parameters = new object[]
                                 {
                                     obProperty,
                                     DbValue.IsNull
                                 };
                return (ObParameterBase)Activator.CreateInstance(t, parameters);
            }
        }

        public static ObParameterBase operator !=(ObProperty obProperty, ObProperty obProperty2)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     DbSymbol.NotEqual,
                                     obProperty2
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public static ObParameterBase operator >(ObProperty obProperty, ObProperty obProperty2)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     DbSymbol.Than,
                                     obProperty2
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public static ObParameterBase operator <(ObProperty obProperty, ObProperty obProperty2)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     DbSymbol.Less,
                                     obProperty2
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public static ObParameterBase operator >=(ObProperty obProperty, ObProperty obProperty2)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     DbSymbol.ThanEqual,
                                     obProperty2
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public static ObParameterBase operator <=(ObProperty obProperty, ObProperty obProperty2)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     obProperty,
                                     DbSymbol.LessEqual,
                                     obProperty2
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase Like(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.Like,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase LikeLeft(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.LikeLeft,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase LikeRight(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.LikeRight,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase In(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.In,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotIn(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.NotIn,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotLike(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.NotLike,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotLikeLift(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.NotLikeLeft,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        public ObParameterBase NotLikeRight(ObProperty obProperty)
        {
            Type t = Assembly.Load(ASSEMBLY_STRING).GetType(CLASS_NAME);
            var parameters = new object[]
                                 {
                                     this,
                                     DbSymbol.NotLikeRight,
                                     obProperty
                                 };
            return (ObParameterBase)Activator.CreateInstance(t, parameters);
        }

        #endregion

        #region 基类重载

        public bool Equals(ObProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.TableName, TableName) && Equals(other.ColumnName, ColumnName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ObProperty)) return false;
            return Equals((ObProperty) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((TableName != null ? TableName.GetHashCode() : 0)*397) ^ (ColumnName != null ? ColumnName.GetHashCode() : 0);
            }
        }

        #endregion

        public override ObProperty As(IObProperty iOProperty)
        {
            AsProperty = iOProperty;
            return this;
        }
    }

    public class ObProperty<TTerm> : ObProperty, IObProperty<TTerm>
        where TTerm : ObTermBase
    {
        private readonly TTerm _term;

        internal ObProperty(TTerm term, IObProperty iObProperty) : base(iObProperty)
        {
            _term = term;
        }

        public ObProperty As(Func<TTerm, IObProperty> keySelector)
        {
            AsProperty = keySelector(_term);
            return this;
        }
    }
}
