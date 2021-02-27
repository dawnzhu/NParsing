using System;
using System.Linq.Expressions;
using DotNet.Standard.NParsing.Factory;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObQueryable<TModel> : IObQuery<TModel>
    {
        IObQueryable<TModel> Where(Expression<Func<TModel, bool>> keySelector);
        IObQueryable<TModel> And(Expression<Func<TModel, bool>> keySelector);
        IObQueryable<TModel> Or(Expression<Func<TModel, bool>> keySelector);
        IObQueryable<TModel> GroupBy<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObQueryable<TModel> DistinctBy<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObQueryable<TModel> Select<TKey>(Expression<Func<TModel[], TKey>> keySelector);
        IObQueryable<TModel> OrderBy<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObQueryable<TModel> OrderByDescending<TKey>(Expression<Func<TModel, TKey>> keySelector);
        IObQueryable<TModel> Join();
        IObQueryable<TModel> Join<TKey>(Expression<Func<TModel, TKey>> keySelector);

        IObQueryable<TModel> Where(IObQueryable<TModel> queryable);
        IObQueryable<TModel> And(IObQueryable<TModel> queryable);
        IObQueryable<TModel> Or(IObQueryable<TModel> queryable);
        IObQueryable<TModel> OrderBy(IObQueryable<TModel> queryable);
        //IObQueryable<TModel> OrderBy(IObQueryable<TModel> queryable);
        //IObQueryable<TModel> OrderByDescending(IObQueryable<TModel> queryable);
        IObQueryable<TModel> GroupBy(IObQueryable<TModel> queryable);
        IObQueryable<TModel> DistinctBy(IObQueryable<TModel> queryable);
        IObQueryable<TModel> Select(IObQueryable<TModel> queryable);
        IObQueryable<TModel> Join(IObQueryable<TModel> queryable);
    }

    public interface IObQueryable<TModel, TTerm> : IObQueryable<TModel>
        where TModel : ObModelBase
        where TTerm : ObTermBase
    {
        TTerm Term { get; }
        /*IObParameter ObParameter { get; }
        IObSort ObSort { get; }
        IObGroup ObGroup { get; }*/
        IObQueryable<TModel, TTerm> Where(Func<TTerm, IObParameter> keySelector);
        IObQueryable<TModel, TTerm> And(Func<TTerm, IObParameter> keySelector);
        IObQueryable<TModel, TTerm> Or(Func<TTerm, IObParameter> keySelector);
        IObQueryable<TModel, TTerm> GroupBy<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> GroupBy(Func<TTerm, ObProperty> keySelector);
        IObQueryable<TModel, TTerm> DistinctBy<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> DistinctBy(Func<TTerm, ObProperty> keySelector);
        IObQueryable<TModel, TTerm> Select<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> Select(Func<TTerm, IObProperty> keySelector);
        IObQueryable<TModel, TTerm> OrderBy<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> OrderBy(Func<TTerm, ObProperty> keySelector);
        IObQueryable<TModel, TTerm> OrderByDescending<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> OrderByDescending(Func<TTerm, ObProperty> keySelector);
        IObQueryable<TModel, TTerm> Join<TKey>(Func<TTerm, TKey> keySelector);
        IObQueryable<TModel, TTerm> Join(Func<TTerm, ObTermBase> keySelector);

        IObQueryable<TModel, TTerm> Where(IObQueryable<TModel, TTerm> queryable);
        IObQueryable<TModel, TTerm> And(IObQueryable<TModel, TTerm> queryable);
        IObQueryable<TModel, TTerm> Or(IObQueryable<TModel, TTerm> queryable);
        IObQueryable<TModel, TTerm> OrderBy(IObQueryable<TModel, TTerm> queryable);
        //IObQueryable<TModel> OrderBy(IObQueryable<TModel> queryable);
        //IObQueryable<TModel> OrderByDescending(IObQueryable<TModel> queryable);
        IObQueryable<TModel, TTerm> GroupBy(IObQueryable<TModel, TTerm> queryable);
        IObQueryable<TModel, TTerm> DistinctBy(IObQueryable<TModel, TTerm> queryable);
        IObQueryable<TModel, TTerm> Select(IObQueryable<TModel, TTerm> queryable);
        IObQueryable<TModel, TTerm> Join(IObQueryable<TModel, TTerm> queryable);
    }
}
