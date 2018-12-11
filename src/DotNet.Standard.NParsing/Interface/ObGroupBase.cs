using System;
using System.Collections.Generic;
using System.Data.Common;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public abstract class ObGroupBase : IObGroup
    {
        protected ObGroupBase()
        {
            ObProperties = new List<IObProperty>();
            ObGroupProperties = new List<IObProperty>();
            DbGroups = new List<DbGroup>();
        }

        /// <summary>
        /// 分组字段列表
        /// </summary>
        public IList<DbGroup> DbGroups { get; protected set; }

        /// <summary>
        /// 分组字段列表
        /// </summary>
        public IList<IObProperty> ObGroupProperties { get; protected set; }

        public virtual IObGroup AddGroupBy(ObProperty obProperty)
        {
            throw new NotImplementedException();
        }

        public virtual IObGroup AddGroupBy(params ObProperty[] obPropertys)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 显示属性列表
        /// </summary>
        public IList<IObProperty> ObProperties { get; protected set; }

        public virtual string ToString(ref IList<DbParameter> dbParameter, out string columns, out IList<string> columnNames)
        {
            throw new NotImplementedException();
        }

        protected string _Key;
        public string Key {
            get
            {
                var key = _Key;
                foreach (var obProperty in ObProperties)
                {
                    if(key.Length > 0)
                        key += "|";
                    key += obProperty.Key;
                }
                return key;
            }
        }
    }
}
