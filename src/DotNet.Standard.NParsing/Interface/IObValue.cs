using System.Collections.Generic;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObValue
    {
        /// <summary>
        /// 平级兄弟列表
        /// </summary>
        IList<object> Brothers { get; }

        /// <summary>
        /// 算术运算符
        /// </summary>
        DbAriSymbol AriSymbol { get; }

        /// <summary>
        /// 值
        /// </summary>
        object Value { get; }
    }
}
