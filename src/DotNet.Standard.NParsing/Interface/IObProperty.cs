using System;
using System.Collections.Generic;
using System.Data.Common;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObProperty
    {
        /// <summary>
        /// 平级兄弟列表
        /// </summary>
        IList<object> Brothers { get; }

        /// <summary>
        /// 算术运算符
        /// </summary>
        DbAriSymbol AriSymbol { get; }

        Type ModelType { get; }
        string TableName { get; }
        string ColumnName { get; }
        string PropertyName { get; }
        DbFunc DbFunc { get; set; }
        int FuncBrotherCount { get; set; }
        IObProperty AsProperty { get; set; }
        IObSort Sort { get; set; }
        IObGroup Group { get; set; }
        string FuncName { get; set; }
        object[] CustomParams { get; set; }

        ObProperty As(IObProperty iOProperty);

        string ToString();

        string ToString(bool renaming);

        string ToString(char separator);

        string ToString(ref IList<DbParameter> dbParameters);

        string ToString(bool renaming, ref IList<DbParameter> dbParameters);

        string ToString(char separator, ref IList<DbParameter> dbParameters);

        string Key { get; }
    }

    public interface IObProperty<out TTerm> : IObProperty
    {
        ObProperty As(Func<TTerm, IObProperty> keySelector);
    }
}