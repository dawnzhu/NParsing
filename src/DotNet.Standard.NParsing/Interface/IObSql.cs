using System.Collections.Generic;
using System.Data;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObSql<TModel>
    {
        /// <summary>
        /// 获得一个对像
        /// </summary>
        /// <returns></returns>
        TModel ToModel();

        /// <summary>
        /// 获得一个对象列表
        /// </summary>
        /// <returns></returns>
        IList<TModel> ToList();

        /// <summary>
        /// 获得一个数据表
        /// </summary>
        /// <returns></returns>
        DataTable ToTable();

        /// <summary>
        /// 获得一个值
        /// </summary>
        /// <param name="iObProperty"></param>
        /// <returns></returns>
        object Scalar();

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        int Exec();
    }
}
