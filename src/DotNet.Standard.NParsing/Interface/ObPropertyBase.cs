using System;
using System.Collections.Generic;
using System.Data.Common;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public abstract class ObPropertyBase : IObProperty
    {
        private IList<object> _brothers;

        /// <summary>
        /// 平级兄弟列表
        /// </summary>
        public IList<object> Brothers
        {
            get
            {
                return _brothers ?? (_brothers = new List<object>());
            }
            protected set
            {
                _brothers = value;
            }
        }

        public DbAriSymbol AriSymbol
        {
            get; protected set;
        }

        public Type ModelType
        {
            get; protected set;
        }

        public string TableName
        {
            get; protected set;
        }

        public string ColumnName
        {
            get; protected set;
        }

        public string PropertyName
        {
            get; protected set;
        }

        public DbFunc DbFunc
        {
            get; set;
        }

        public int FuncBrotherCount { get; set; }

        public IObProperty AsProperty { get; set; }
        public IObSort Sort { get; set; }
        public IObGroup Group { get; set; }

        public string FuncName { get; set; }

        public object[] CustomParams { get; set; }

        public virtual ObProperty As(IObProperty iOProperty)
        {
            throw new NotImplementedException();
        }

        public new virtual string ToString()
        {
            throw new NotImplementedException();
        }

        public virtual string ToString(char separator)
        {
            throw new NotImplementedException();
        }

        public virtual string ToString(ref IList<DbParameter> dbParameters)
        {
            throw new NotImplementedException();
        }

        public virtual string ToString(char separator, ref IList<DbParameter> dbParameters)
        {
            throw new NotImplementedException();
        }

        public string Key
        {
            get
            {
                return TableName + "_" + ColumnName + (DbFunc == DbFunc.Null ? "" : "_" + DbFunc);
            }
        }

        public virtual string ToString(bool renaming)
        {
            throw new NotImplementedException();
        }

        public virtual string ToString(bool renaming, ref IList<DbParameter> dbParameters)
        {
            throw new NotImplementedException();
        }
    }
}
