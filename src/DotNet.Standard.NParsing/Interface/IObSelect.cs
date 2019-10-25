using System;
using DotNet.Standard.NParsing.Factory;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObSelect<TModel, out TTerm> : IObQuery<TModel>
        where TModel : ObModelBase
        where TTerm : ObTermBase
    {
        TTerm Term { get; }
        /*IObParameter ObParameter { get; }
        IObSort ObSort { get; }
        IObGroup ObGroup { get; }*/
        IObSelect<TModel, TTerm> Where(Func<TTerm, IObParameter> keySelector);
        IObSelect<TModel, TTerm> GroupBy<TKey>(Func<TTerm, TKey> keySelector);
        IObSelect<TModel, TTerm> GroupBy(Func<TTerm, ObProperty> keySelector);
        IObSelect<TModel, TTerm> DistinctBy<TKey>(Func<TTerm, TKey> keySelector);
        IObSelect<TModel, TTerm> DistinctBy(Func<TTerm, ObProperty> keySelector);
        IObSelect<TModel, TTerm> Select<TKey>(Func<TTerm, TKey> keySelector);
        IObSelect<TModel, TTerm> Select(Func<TTerm, IObProperty> keySelector);
        IObSelect<TModel, TTerm> OrderBy<TKey>(Func<TTerm, TKey> keySelector);
        IObSelect<TModel, TTerm> OrderBy(Func<TTerm, ObProperty> keySelector);
        IObSelect<TModel, TTerm> OrderByDescending<TKey>(Func<TTerm, TKey> keySelector);
        IObSelect<TModel, TTerm> OrderByDescending(Func<TTerm, ObProperty> keySelector);
        IObSelect<TModel, TTerm> Join<TKey>(Func<TTerm, TKey> keySelector);
        IObSelect<TModel, TTerm> Join(Func<TTerm, ObTermBase> keySelector);
    }
}
