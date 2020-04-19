using System;
using System.Collections.Generic;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObJoin
    {
        IList<string> JoinModels { get; }

        IObJoin AddJoin(ObTermBase obTermBase);

        IObJoin AddJoin(params ObTermBase[] obTermBases);
    }

    public interface IObJoin<out TTerm> : IObJoin
        where TTerm : ObTermBase
    {
        IObJoin<TTerm> AddJoin(Func<TTerm, ObTermBase> keySelector);

        IObJoin<TTerm> AddJoin(Func<TTerm, ObTermBase[]> keySelector);

        IObJoin<TTerm> AddJoin<TKey>(Func<TTerm, TKey> keySelector);
    }
}
