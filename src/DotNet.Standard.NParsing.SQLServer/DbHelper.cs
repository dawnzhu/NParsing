using System.Data.Common;
using System.Data.SqlClient;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.SQLServer
{
    public class DbHelper : IDbHelper
    {
        #region IDbHelper Members

        private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbConnection DbConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public DbCommand DbCommand()
        {
            return new SqlCommand();
        }

        public DbDataAdapter DbDataAdapter()
        {
            return new SqlDataAdapter();
        }

        #endregion
    }
}