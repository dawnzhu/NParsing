using System;
using System.Collections.Generic;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public abstract class SqlBuilderBase<TModel>
    {
        protected Type ModelType { get; }

        /// <summary>
        /// 不允许关联
        /// </summary>
        protected IList<string> NotJoinModels { get; }
        public string TableExtra { get; }
        protected string TableName { get; }
        public IDictionary<string, string> TableNames { get; }
        protected SqlBuilderBase(IObRedefine iObRedefine, IList<string> notJoinModels)
        {
            ModelType = typeof (TModel);
            TableName = ModelType.ToTableName(iObRedefine.Models, out var extra);
            TableExtra = extra;
            TableNames = iObRedefine.Models;
            NotJoinModels = notJoinModels;
        }

        protected virtual bool IsJoin(string tableName)
        {
            if (NotJoinModels == null) return true;
            return !NotJoinModels.Contains(tableName);
        }
    }
}