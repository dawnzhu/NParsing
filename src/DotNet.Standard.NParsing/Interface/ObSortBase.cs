using System;
using System.Collections.Generic;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public abstract class ObSortBase : IObSort
    {
        private IList<DbSort> _list;

        public IList<DbSort> List
        {
            get { return _list ?? (_list = new List<DbSort>()); }
            protected set
            {
                _list = value;
            }
        }

        public virtual IObSort AddOrderBy(ObProperty obProperty)
        {
            throw new NotImplementedException();
        }

        public virtual IObSort AddOrderByDescending(ObProperty obProperty)
        {
            throw new NotImplementedException();
        }

        public virtual IObSort AddOrderBy(ObProperty[] obPropertys)
        {
            throw new NotImplementedException();
        }

        public virtual IObSort AddOrderByDescending(ObProperty[] obPropertys)
        {
            throw new NotImplementedException();
        }

        public virtual IObSort Add(IObSort obSort)
        {
            throw new NotImplementedException();
        }

        public virtual IObSort Add(IObSort[] obSorts)
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

        public virtual string ToString(IList<string> columnNames)
        {
            throw new NotImplementedException();
        }

        public virtual string ToString(char separator, IList<string> columnNames)
        {
            throw new NotImplementedException();
        }

        public string Key { get; protected set; }
    }
}
