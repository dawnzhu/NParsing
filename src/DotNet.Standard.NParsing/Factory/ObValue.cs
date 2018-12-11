using System.Collections.Generic;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Factory
{
    internal class ObValue : IObValue
    {
        public ObValue(DbAriSymbol ariSymbol, object value)
        {
            AriSymbol = ariSymbol;
            Value = value;
            Brothers = new List<object>();
        }

        /// <summary>
        /// 平级兄弟列表
        /// </summary>
        public IList<object> Brothers
        {
            get; protected set;
        }

        public DbAriSymbol AriSymbol
        {
            get; protected set;
        }

        public object Value { get; private set; }
    }
}
