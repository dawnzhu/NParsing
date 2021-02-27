using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObQueryable<TModel> : ObQuery<TModel>, IObQueryable<TModel>
        where TModel : class, new()
    {
        public ObQueryable(IDbHelper iDbHelper, ISqlBuilder<TModel> iSqlBuilder, string providerName, IObJoin iJoin) : base(iDbHelper, iSqlBuilder, providerName, iJoin)
        {
        }

        public IObQueryable<TModel> Where(Expression<Func<TModel, bool>> keySelector)
        {
            if (ObGroup == null)
            {
                ObParameter = CreateWhere(keySelector.Body);
            }
            else
            {
                ObGroupParameter = CreateWhere(keySelector.Body);
            }
            return this;
        }

        public IObQueryable<TModel> And(Expression<Func<TModel, bool>> keySelector)
        {
            if (ObGroup == null)
            {
                ObParameter = (ObParameterBase)ObParameter && CreateWhere(keySelector.Body);
            }
            else
            {
                ObGroupParameter = (ObParameterBase)ObGroupParameter && CreateWhere(keySelector.Body);
            }
            return this;
        }

        public IObQueryable<TModel> Or(Expression<Func<TModel, bool>> keySelector)
        {
            if (ObGroup == null)
            {
                ObParameter = Factory.ObParameter.Create((ObParameterBase)ObParameter || CreateWhere(keySelector.Body));
            }
            else
            {
                ObGroupParameter = Factory.ObParameter.Create((ObParameterBase)ObGroupParameter || CreateWhere(keySelector.Body));
            }
            return this;
        }

        public IObQueryable<TModel> OrderBy<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            var body = keySelector.Body;
            var value = CreateValue(body);
            if (value is ObProperty obProperty)
            {
                ObSort.AddOrderBy(obProperty);
            }
            if (value is object[] objs)
            {
                ObSort.AddOrderBy(objs.Select(o => (ObProperty)o).ToArray());
            }
            return this;
        }

        public IObQueryable<TModel> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            var body = keySelector.Body;
            var value = CreateValue(body);
            if (value is ObProperty obProperty)
            {
                ObSort.AddOrderBy(obProperty);
            }
            if (value is object[] objs)
            {
                ObSort.AddOrderByDescending(objs.Select(o => (ObProperty)o).ToArray());
            }
            return this;
        }

        public IObQueryable<TModel> GroupBy<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            var body = keySelector.Body;
            var value = CreateValue(body);
            if (value is ObProperty obProperty)
            {
                ObGroup.AddGroupBy(obProperty);
            }
            if (value is object[] objs)
            {
                ObGroup.AddGroupBy(objs.Select(o => (ObProperty)o).ToArray());
            }
            return this;
        }

        public IObQueryable<TModel> DistinctBy<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            return GroupBy(keySelector);
        }

        public IObQueryable<TModel> Join()
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            return this;
        }

        public IObQueryable<TModel> Join<TKey>(Expression<Func<TModel, TKey>> keySelector)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            var body = keySelector.Body;
            var value = CreateJoin(body);
            if (value is object[] objs)
            {
                foreach (string obj in objs)
                {
                    ObJoin.JoinModels.Add(obj);
                }
            }
            else
            {
                ObJoin.JoinModels.Add((string)value);
            }
            return this;
        }

        public IObQueryable<TModel> Select<TKey>(Expression<Func<TModel[], TKey>> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            var body = keySelector.Body;
            var value = CreateValue(body);
            if (value is ObProperty obProperty)
            {
                ObGroup.ObProperties.Add(obProperty);
            }
            if (value is object[] objs)
            {
                foreach (ObProperty property in objs)
                {
                    ObGroup.ObProperties.Add(property);
                }
            }
            return this;
        }

        public IObQueryable<TModel> Where(IObQueryable<TModel> queryable)
        {
            ObParameter = queryable.ObParameter;
            ObGroupParameter = queryable.ObGroupParameter;
            ObGroup = queryable.ObGroup;
            ObSort = queryable.ObSort;
            ObJoin = queryable.ObJoin;
            return this;
        }

        public IObQueryable<TModel> And(IObQueryable<TModel> queryable)
        {
            ObParameter = (ObParameterBase)ObParameter && (ObParameterBase)queryable.ObParameter;
            if (ObGroup != null && queryable.ObGroupParameter != null)
            {
                ObGroupParameter = (ObParameterBase)ObGroupParameter && (ObParameterBase)queryable.ObGroupParameter;
            }
            return this;
        }

        public IObQueryable<TModel> Or(IObQueryable<TModel> queryable)
        {
            ObParameter = Factory.ObParameter.Create((ObParameterBase)ObParameter || (ObParameterBase)queryable.ObParameter);
            if (ObGroup != null && queryable.ObGroupParameter != null)
            {
                ObGroupParameter = Factory.ObParameter.Create((ObParameterBase)ObGroupParameter || (ObParameterBase)queryable.ObGroupParameter);
            }
            return this;
        }

        public IObQueryable<TModel> OrderBy(IObQueryable<TModel> queryable)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.Add(queryable.ObSort);
            return this;
        }

        /*public IObQueryable<TModel> OrderBy(IObQueryable<TModel> queryable)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.AddOrderBy(queryable.ObSort.List.Where(o => o.IsAsc).Select(o => o.ObProperty).ToArray());
            return this;
        }

        public IObQueryable<TModel> OrderByDescending(IObQueryable<TModel> queryable)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.AddOrderByDescending(queryable.ObSort.List.Where(o => !o.IsAsc).Select(o => o.ObProperty).ToArray());
            return this;
        }*/

        public IObQueryable<TModel> GroupBy(IObQueryable<TModel> queryable)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.AddGroupBy(queryable.ObGroup.ObGroupProperties.Select(o => (ObProperty)o).ToArray());
            return this;
        }

        public IObQueryable<TModel> DistinctBy(IObQueryable<TModel> queryable)
        {
            return GroupBy(queryable);
        }

        public IObQueryable<TModel> Join(IObQueryable<TModel> queryable)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            foreach (var obJoinJoinModel in queryable.ObJoin.JoinModels)
            {
                ObJoin.JoinModels.Add(obJoinJoinModel);
            }
            return this;
        }

        public IObQueryable<TModel> Select(IObQueryable<TModel> queryable)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.AddGroupBy(queryable.ObGroup.ObProperties.Select(o => (ObProperty)o).ToArray());
            return this;
        }

        private object CreateJoin(Expression exp)
        {
            if (exp is NewExpression newExp)
            {
                return newExp.Arguments.Select(CreateJoin).ToArray();
            }

            if (exp is NewArrayExpression arrExp)
            {
                return arrExp.Expressions.Select(CreateJoin).ToArray();
            }

            if (exp is ParameterExpression parExp)
            {
                return parExp.Type.ToTableName(SqlBuilder.ObRedefine.Models);
            }

            if (exp is MemberExpression memExp)
            {
                var tableName = typeof(TModel).ToTableName(SqlBuilder.ObRedefine.Models);
                var ts = memExp.ToString().Split('.').ToList();
                ts[0] = tableName;
                var propertyType = ((PropertyInfo) memExp.Member).PropertyType;
                var obRedefine = Factory.ObRedefine.Create(propertyType, string.Join("_", ts));
                return propertyType.ToTableName(obRedefine.Models);
            }

            return null;
        }

        private ObParameterBase CreateWhere(Expression exp)
        {
            ObParameterBase p = null;
            var type = NodeType(exp.NodeType);
            if (type == 1)
            {
                if (exp is BinaryExpression biExp)
                {
                    p = CreateWhere(biExp.Left);
                    var pp = CreateWhere(biExp.Right);
                    switch (biExp.NodeType)
                    {
                        case ExpressionType.AndAlso:
                            p = p && pp;
                            break;
                        case ExpressionType.OrElse:
                            p = Factory.ObParameter.Create(p || pp);
                            break;
                    }
                }
            }
            else //if ((14 & type) == type)
            {
                p = CreateParameter(exp);
            }
            return p;
        }

        /*private ObParameterBase DealExpress(Expression exp)
        {
            ObParameterBase p = null;
            if (exp is LambdaExpression laExp)
            {
                p = DealExpress(laExp.Body);
            }
            else if (exp is BinaryExpression biExp)
            {
                //表达式
                p = CreateParameter(biExp, biExp.Left);
                var pp = CreateParameter(biExp, biExp.Right);
                switch (biExp.NodeType)
                {
                    case ExpressionType.AndAlso:
                        p = p && pp;
                        break;
                    case ExpressionType.OrElse:
                        p = Factory.ObParameter.Create(p || pp);
                        break;
                }
            }
            else if(exp is MethodCallExpression mcallExp)
            {
                p = CreateParameter(mcallExp);
            }
            else if (exp is UnaryExpression unExp)
            {
                p = CreateParameter(unExp.Operand, unExp.NodeType == ExpressionType.Not);
            }
            else if (exp is ConstantExpression coExp)
            {
                p = Factory.ObParameter.Create((bool)coExp.Value);
            }
            return p;
        }

        private ObParameterBase CreateParameter(Expression exp, Expression sonExp)
        {
            ObParameterBase p;
            if (sonExp is UnaryExpression unExp && (unExp.Operand is BinaryExpression || unExp.Operand is MethodCallExpression meExp && (meExp.Method.Name == "Contains" || meExp.Method.Name == "Equals"))
                || sonExp is ConstantExpression && NodeType(exp.NodeType) == 1
                || sonExp is BinaryExpression && (3 & NodeType(sonExp.NodeType)) == NodeType(sonExp.NodeType))
            {
                p = DealExpress(sonExp);
            }
            else
            {
                p = CreateParameter(exp);
            }
            /*if (sonExp is BinaryExpression)
            {
                p = IsArithmetic(sonExp.NodeType)
                    ? CreateParameter(exp)
                    : DealExpress(sonExp);
            }
            else if (sonExp is UnaryExpression)
            {
                p = DealExpress(sonExp);
            }
            else
            {
                p = CreateParameter(exp);
            }#1#
            return p;
        }*/

        private ObParameterBase CreateParameter(Expression exp, bool not = false)
        {
            if (exp is MethodCallExpression mcallExp)
            {
                ObProperty obProperty;
                switch (mcallExp.Method.Name)
                {
                    case "Contains":
                        //var exps = ((NewArrayExpression) mcallExp.Arguments[0]).Expressions;
                        obProperty = CreateProperty(mcallExp.Object ?? mcallExp.Arguments[1]);
                        if (mcallExp.Arguments[0] is NewArrayExpression arrExp)
                        {
                            var value = arrExp.Expressions.Select(o => ((ConstantExpression) o).Value).ToArray();
                            return not ? obProperty.NotIn(value) : obProperty.In(value);
                        }
                        else if (mcallExp.Arguments[0] is MemberExpression meExp)
                        {
                            var value = CreateValue(meExp);
                            if (value is ICollection vs)
                            {
                                value = vs.Cast<object>().ToArray();
                            }
                            return not ? obProperty.NotIn(value) : obProperty.In(value);
                        }
                        //TODO
                        else //if(mcallExp.Arguments[0] is ConstantExpression coExp)
                        {
                            return not ? obProperty.NotLike(CreateValue(mcallExp.Arguments[0])) : obProperty.Like(CreateValue(mcallExp.Arguments[0]));
                        }
                    case "Equals":
                        obProperty = CreateProperty(mcallExp.Object ?? mcallExp.Arguments[1]);
                        return not ? obProperty != CreateValue(mcallExp.Arguments[0]) : obProperty == CreateValue(mcallExp.Arguments[0]);
                }
            }
            else if (exp is BinaryExpression biExp)
            {
                var obProperty = CreateProperty(biExp.Left);
                var value = CreateValue(biExp.Right);
                return CreateParameter(obProperty, exp.NodeType, value, not);
            }
            else if (exp is UnaryExpression unExp)
            {
                return CreateParameter(unExp.Operand, not ? unExp.NodeType != ExpressionType.Not : unExp.NodeType == ExpressionType.Not);
            }
            else if (exp is ConstantExpression coExp)
            {
                return Factory.ObParameter.Create(not ? !(bool)coExp.Value : (bool)coExp.Value);
            }
            return null;
        }

        private static ObParameterBase CreateParameter(ObProperty obProperty, ExpressionType nodeType, object value, bool not)
        {
            switch (nodeType)
            {
                case ExpressionType.Equal:
                    return not? obProperty != value : obProperty == value;
                case ExpressionType.GreaterThan:
                    return not ? obProperty <= value : obProperty > value;
                case ExpressionType.GreaterThanOrEqual:
                    return not ? obProperty > value : obProperty >= value;
                case ExpressionType.LessThan:
                    return not ? obProperty >= value : obProperty < value;
                case ExpressionType.LessThanOrEqual:
                    return not ? obProperty > value : obProperty <= value;
                case ExpressionType.NotEqual:
                    return not ? obProperty == value : obProperty != value;
            }
            return null;
        }

        private ObProperty CreateProperty(Expression exp)
        {
            return (ObProperty)CreateValue(exp);
        }

        private object CreateValue(Expression exp, string path = "")
        {
            if (exp is BinaryExpression biExp)
            {
                var left = CreateProperty(biExp.Left);
                var right = CreateValue(biExp.Right);
                return CreateProperty(left, biExp.NodeType, right);
            }

            if (exp is NewExpression newExp)
            {
                //return arrExp.Expressions.Select(o => ((ConstantExpression)o).Value).ToArray();
                var list = new List<object>();
                for (var i = 0; i < newExp.Arguments.Count; i++)
                {
                    var a = newExp.Arguments[i];
                    var m = newExp.Members[i];
                    var obj = CreateValue(a, path + "." + m.Name);
                    if (obj is object[] objs)
                    {
                        list.AddRange(objs);
                    }
                    else
                    {
                        list.Add(obj);
                    }
                }
                return list.ToArray();
            }

            if (exp is NewArrayExpression arrExp)
            {
                //return arrExp.Expressions.Select(o => ((ConstantExpression)o).Value).ToArray();
                return arrExp.Expressions.Select(o => CreateValue(o)).ToArray();
            }

            if (exp is ConstantExpression coExp)
            {
                //常量
                return coExp.Value;
            }

            if (exp is MemberExpression meExp)
            {
                if (meExp.Expression is ConstantExpression 
                    || meExp.Expression is MemberExpression)
                {
                    return Expression.Lambda(meExp).Compile().DynamicInvoke();
                }
                var tableName = typeof(TModel).ToTableName(SqlBuilder.ObRedefine.Models);
                var ts = meExp.ToString()
                    .Replace(".FirstOrDefault()", "").Replace(".First()", "")
                    .Replace(".LastOrDefault()", "").Replace(".Last()", "")
                    .Split('.').ToList();
                ts[0] = tableName;
                ts.RemoveAt(ts.Count - 1);
                var obRedefine = Factory.ObRedefine.Create(meExp.Expression.Type, string.Join("_", ts));
                return ObProperty.Create(meExp.Expression.Type, obRedefine, meExp.Member.Name);
            }

            if (exp is MethodCallExpression mcallExp)
            {
                switch (mcallExp.Method.Name)
                {
                    /*case "Contains":
                    {
                        if (mcallExp.Object != null)
                        {
                            return CreateProperty(mcallExp.Object);
                        }
                        return CreateProperty(mcallExp.Arguments[1]);
                    }*/
                    case "Substring":
                    {
                        var obProperty = CreateProperty(mcallExp.Object);
                        var value = mcallExp.Arguments.Select(o => (int)((ConstantExpression)o).Value).ToArray();
                            obProperty = ObFunc.SubString(obProperty, value[0], value[1]);
                        return obProperty;
                    }
                    case "Replace":
                    {
                        var obProperty = CreateProperty(mcallExp.Object);
                        var value = mcallExp.Arguments.Select(o => (string) ((ConstantExpression) o).Value).ToArray();
                        obProperty = ObFunc.Replace(obProperty, value[0], value[1]);
                        return obProperty;
                    }
                    case "IndexOf":
                    {
                        var obProperty = CreateProperty(mcallExp.Object);
                        var value = mcallExp.Arguments.Select(o => (string)((ConstantExpression)o).Value).ToArray();
                        obProperty = ObFunc.IndexOf(obProperty, value[0]);
                        return obProperty;
                    }
                    case "ToString":
                    {
                        var obProperty = CreateProperty(mcallExp.Object);
                        obProperty = mcallExp.Arguments.Count > 0 
                            ? ObFunc.ToString(obProperty, CreateValue(mcallExp.Arguments[0]).ToString()) 
                            : ObFunc.ToString(obProperty);
                        return obProperty;
                    }
                    case "ToInt16":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[0]);
                        obProperty = ObFunc.ToInt16(obProperty);
                        return obProperty;
                    }
                    case "ToInt32":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[0]);
                        obProperty = ObFunc.ToInt32(obProperty);
                        return obProperty;
                    }
                    case "ToInt64":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[0]);
                        obProperty = ObFunc.ToInt64(obProperty);
                        return obProperty;
                    }
                    case "ToSingle":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[0]);
                        obProperty = ObFunc.ToSingle(obProperty);
                        return obProperty;
                    }
                    case "ToDouble":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[0]);
                        obProperty = ObFunc.ToDouble(obProperty);
                        return obProperty;
                    }
                    /*case "ToDecimal":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[0]);
                        obProperty = ObFunc.ToDecimal(obProperty);
                        return obProperty;
                    }*/
                    case "ToDateTime":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[0]);
                        obProperty = ObFunc.ToDateTime(obProperty);
                        return obProperty;
                    }
                    /*case "Format":
                    {
                        var obProperty = CreateProperty(mcallExp.Arguments[1]);
                        obProperty = ObFunc.ToString(obProperty, mcallExp.Arguments[0].ToString());
                        return obProperty;
                    }*/
                    case "Average":
                    {
                        var obProperty = CreateProperty(((LambdaExpression) mcallExp.Arguments[1]).Body);
                        obProperty = ObFunc.Avg(obProperty);
                        return obProperty;
                    }
                    case "Count":
                    {
                        var type = typeof(TModel); 
                        var tableName = type.ToTableName(SqlBuilder.ObRedefine.Models);
                        var ts = (tableName + path).Split('.').ToList();
                        var memberName = ts[ts.Count - 1];
                        ts.RemoveAt(ts.Count - 1);
                        foreach (var t in ts)
                        {
                            foreach (var propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                            {
                                if (propertyInfo.Name != t) continue;
                                type = propertyInfo.PropertyType;
                                break;
                            }
                        }
                        var obRedefine = Factory.ObRedefine.Create(type, string.Join("_", ts));
                        var obProperty = ObProperty.Create(type, obRedefine, memberName);
                        obProperty = ObFunc.Count(obProperty);
                        return obProperty;
                    }
                    case "Max":
                    {
                        var obProperty = CreateProperty(((LambdaExpression)mcallExp.Arguments[1]).Body);
                        obProperty = ObFunc.Max(obProperty);
                        return obProperty;
                    }
                    case "Min":
                    {
                        var obProperty = CreateProperty(((LambdaExpression)mcallExp.Arguments[1]).Body);
                        obProperty = ObFunc.Min(obProperty);
                        return obProperty;
                    }
                    case "Sum":
                    {
                        var obProperty = CreateProperty(((LambdaExpression)mcallExp.Arguments[1]).Body);
                        obProperty = ObFunc.Sum(obProperty);
                        return obProperty;
                    }
                }
            }

            if (exp is UnaryExpression unExp)
            {
                var obProperty = CreateProperty(unExp.Operand);
                switch (unExp.NodeType)
                {
                    case ExpressionType.Convert:
                        if (unExp.Type == typeof(string))
                        {
                            obProperty = ObFunc.ToString(obProperty);
                        }
                        if (unExp.Type == typeof(short))
                        {
                            obProperty = ObFunc.ToInt16(obProperty);
                        }
                        if (unExp.Type == typeof(int))
                        {
                            obProperty = ObFunc.ToInt32(obProperty);
                        }
                        if (unExp.Type == typeof(long))
                        {
                            obProperty = ObFunc.ToInt64(obProperty);
                        }
                        if (unExp.Type == typeof(float))
                        {
                            obProperty = ObFunc.ToSingle(obProperty);
                        }
                        if (unExp.Type == typeof(double))
                        {
                            obProperty = ObFunc.ToDouble(obProperty);
                        }
                        if (unExp.Type == typeof(DateTime))
                        {
                            obProperty = ObFunc.ToDateTime(obProperty);
                        }
                        return obProperty;
                    case ExpressionType.Not:
                        obProperty = ~obProperty;
                        return obProperty;
                    case ExpressionType.Negate:
                        obProperty = -obProperty;
                        return obProperty;
                }
            }
            return null;
        }

        private static ObProperty CreateProperty(ObProperty left, ExpressionType nodeType, object right)
        {
            switch (nodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return left + right;
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return left - right;
                case ExpressionType.Divide:
                    return left / right;
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return left * right;
                case ExpressionType.Modulo:
                    return left % right;
                case ExpressionType.And:
                    return left & right;
                case ExpressionType.Or:
                    return left | right;
                case ExpressionType.ExclusiveOr:
                    return left ^ right;
                case ExpressionType.LeftShift:
                    return left << Convert.ToInt32(right);
                case ExpressionType.RightShift:
                    return left >> Convert.ToInt32(right);
            }
            return null;
        }

        private static int NodeType(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.AndAlso: // &&
                case ExpressionType.OrElse: // ||
                    return 1;
                case ExpressionType.Equal: // ==
                case ExpressionType.GreaterThan: // >
                case ExpressionType.GreaterThanOrEqual: // >=
                case ExpressionType.LessThan: // <
                case ExpressionType.LessThanOrEqual: // <=
                case ExpressionType.NotEqual: // !=
                    return 2;
                case ExpressionType.Add: // +
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract: // -
                case ExpressionType.SubtractChecked:
                case ExpressionType.Divide: // /
                case ExpressionType.Multiply: // *
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Modulo: // %
                case ExpressionType.And: // &
                case ExpressionType.Or: // |
                case ExpressionType.ExclusiveOr: // ^
                case ExpressionType.Not: // ~
                case ExpressionType.LeftShift: // <<
                case ExpressionType.RightShift: // >>
                case ExpressionType.Negate: // -
                    return 4;
                case ExpressionType.Constant:
                case ExpressionType.Call:
                    return 8;
            }
            return 0;
        }
    }

    public class ObQueryable<TModel, TTerm> : ObQueryable<TModel>, IObQueryable<TModel, TTerm>
        where TModel : ObModelBase, new()
        where TTerm : ObTermBase
    {
        public TTerm Term { get; }
        /*public IObParameter ObParameter { get; private set; }
        public IObSort ObSort { get; private set; }
        public IObGroup ObGroup { get; private set; }
        public IObJoin ObJoin { get; private set; }*/

        public ObQueryable(IDbHelper iDbHelper, ISqlBuilder<TModel> iSqlBuilder, string providerName, TTerm term, IObJoin iJoin) : base(iDbHelper, iSqlBuilder, providerName, iJoin)
        {
            Term = term;
        }

        /*public ObQueryable(TTerm term)
        {
            Term = term;
        }*/

        public IObQueryable<TModel, TTerm> Where(Func<TTerm, IObParameter> keySelector)
        {
            if (ObGroup == null)
            {
                ObParameter = keySelector(Term);
            }
            else
            {
                ObGroupParameter = keySelector(Term);
            }
            return this;
        }

        public IObQueryable<TModel, TTerm> And(Func<TTerm, IObParameter> keySelector)
        {
            if (ObGroup == null)
            {
                ObParameter = (ObParameterBase)ObParameter && (ObParameterBase)keySelector(Term);
            }
            else
            {
                ObGroupParameter = (ObParameterBase)ObGroupParameter && (ObParameterBase)keySelector(Term);
            }
            return this;
        }

        public IObQueryable<TModel, TTerm> Or(Func<TTerm, IObParameter> keySelector)
        {
            if (ObGroup == null)
            {
                ObParameter = Factory.ObParameter.Create((ObParameterBase)ObParameter || (ObParameterBase)keySelector(Term));
            }
            else
            {
                ObGroupParameter = Factory.ObParameter.Create((ObParameterBase)ObGroupParameter || (ObParameterBase)keySelector(Term));
            }
            return this;
        }

        public IObQueryable<TModel, TTerm> GroupBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            var list = new List<ObProperty>();
            var key = keySelector(Term);
            if (key is ObProperty value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObProperty value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            ObGroup.AddGroupBy(list.ToArray());
            return this;
        }

        public IObQueryable<TModel, TTerm> GroupBy(Func<TTerm, ObProperty> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.AddGroupBy(keySelector(Term));
            return this;
        }

        public IObQueryable<TModel, TTerm> DistinctBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            return GroupBy(keySelector);
        }

        public IObQueryable<TModel, TTerm> DistinctBy(Func<TTerm, ObProperty> keySelector)
        {
            return GroupBy(keySelector);
        }

        public IObQueryable<TModel, TTerm> Select<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            var key = keySelector(Term);
            if (key is IObProperty value)
            {
                ObGroup.ObProperties.Add(value);
            }
            else
            {
                /*foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is IObProperty value2)
                    {
                        if (value2.AsProperty.PropertyName != propertyInfo.Name)
                        {
                            var asProperty = typeof(TTerm).GetProperty(propertyInfo.Name);
                            if (asProperty != null)
                            {
                                value2.AsProperty = (IObProperty)asProperty.GetValue(Term);
                            }
                        }
                        ObGroup.ObProperties.Add(value2);
                    }
                }*/
                Select(ObGroup, Term, key);
            }
            return this;
        }

        private static void Select(IObGroup obGroup, object term, object value)
        {
            foreach (var valueProperty in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var v = valueProperty.GetValue(value);
                if (v is IObProperty value2)
                {
                    var asProperty = term.GetType().GetProperty(valueProperty.Name);
                    if (asProperty != null)
                    {
                        value2.AsProperty = (IObProperty)asProperty.GetValue(term);
                    }
                    obGroup.ObProperties.Add(value2);
                }
                else
                {
                    var termProperty = term.GetType().GetProperty(valueProperty.Name);
                    if (termProperty != null)
                    {
                        var t = termProperty.GetValue(term);
                        Select(obGroup, t, v);
                    }
                }
            }
        }

        public IObQueryable<TModel, TTerm> Select(Func<TTerm, IObProperty> keySelector)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.ObProperties.Add(keySelector(Term));
            return this;
        }

        public IObQueryable<TModel, TTerm> OrderBy<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            var list = new List<ObProperty>();
            var key = keySelector(Term);
            if (key is ObProperty value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObProperty value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            ObSort.AddOrderBy(list.ToArray());
            return this;
        }

        public IObQueryable<TModel, TTerm> OrderBy(Func<TTerm, ObProperty> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.AddOrderBy(keySelector(Term));
            return this;
        }

        public IObQueryable<TModel, TTerm> OrderByDescending<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            var list = new List<ObProperty>();
            var key = keySelector(Term);
            if (key is ObProperty value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObProperty value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            ObSort.AddOrderByDescending(list.ToArray());
            return this;
        }

        public IObQueryable<TModel, TTerm> OrderByDescending(Func<TTerm, ObProperty> keySelector)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.AddOrderByDescending(keySelector(Term));
            return this;
        }

        public IObQueryable<TModel, TTerm> Join<TKey>(Func<TTerm, TKey> keySelector)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            var list = new List<ObTermBase>();
            var key = keySelector(Term);
            if (key is ObTermBase value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObTermBase value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            ObJoin.AddJoin(list.ToArray());
            return this;
        }

        public IObQueryable<TModel, TTerm> Join(Func<TTerm, ObTermBase> keySelector)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            ObJoin.AddJoin(keySelector(Term));
            return this;
        }

        public IObQueryable<TModel, TTerm> Where(IObQueryable<TModel, TTerm> queryable)
        {
            ObParameter = queryable.ObParameter;
            ObGroupParameter = queryable.ObGroupParameter;
            ObGroup = queryable.ObGroup;
            ObSort = queryable.ObSort;
            ObJoin = queryable.ObJoin;
            return this;
        }

        public IObQueryable<TModel, TTerm> And(IObQueryable<TModel, TTerm> queryable)
        {
            ObParameter = (ObParameterBase)ObParameter && (ObParameterBase)queryable.ObParameter;
            if (ObGroup != null && queryable.ObGroupParameter != null)
            {
                ObGroupParameter = (ObParameterBase)ObGroupParameter && (ObParameterBase)queryable.ObGroupParameter;
            }
            return this;
        }

        public IObQueryable<TModel, TTerm> Or(IObQueryable<TModel, TTerm> queryable)
        {
            ObParameter = Factory.ObParameter.Create((ObParameterBase)ObParameter || (ObParameterBase)queryable.ObParameter);
            if (ObGroup != null && queryable.ObGroupParameter != null)
            {
                ObGroupParameter = Factory.ObParameter.Create((ObParameterBase)ObGroupParameter || (ObParameterBase)queryable.ObGroupParameter);
            }
            return this;
        }

        public IObQueryable<TModel, TTerm> OrderBy(IObQueryable<TModel, TTerm> queryable)
        {
            if (ObSort == null)
            {
                ObSort = new ObSort();
            }
            ObSort.Add(queryable.ObSort);
            return this;
        }

        public IObQueryable<TModel, TTerm> GroupBy(IObQueryable<TModel, TTerm> queryable)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.AddGroupBy(queryable.ObGroup.ObGroupProperties.Select(o => (ObProperty)o).ToArray());
            return this;
        }

        public IObQueryable<TModel, TTerm> DistinctBy(IObQueryable<TModel, TTerm> queryable)
        {
            return GroupBy(queryable);
        }

        public IObQueryable<TModel, TTerm> Select(IObQueryable<TModel, TTerm> queryable)
        {
            if (ObJoin == null)
            {
                ObJoin = new ObJoin();
            }
            foreach (var obJoinJoinModel in queryable.ObJoin.JoinModels)
            {
                ObJoin.JoinModels.Add(obJoinJoinModel);
            }
            return this;
        }

        public IObQueryable<TModel, TTerm> Join(IObQueryable<TModel, TTerm> queryable)
        {
            if (ObGroup == null)
            {
                ObGroup = new ObGroup();
            }
            ObGroup.AddGroupBy(queryable.ObGroup.ObProperties.Select(o => (ObProperty)o).ToArray());
            return this;
        }
    }
}
