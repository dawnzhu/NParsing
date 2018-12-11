using System.Data.Common;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IDbHelper
    {
        DbConnection DbConnection();
        DbCommand DbCommand();
        DbDataAdapter DbDataAdapter();
    }
}