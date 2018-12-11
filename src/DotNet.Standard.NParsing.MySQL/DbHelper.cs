using System.Data.Common;
using DotNet.Standard.NParsing.Interface;
using MySql.Data.MySqlClient;

namespace DotNet.Standard.NParsing.MySQL
{
    public class DbHelper : IDbHelper
    {
        private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region IDbHelper Members

        public DbConnection DbConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public DbCommand DbCommand()
        {
            return new MySqlCommand();
        }

        public DbDataAdapter DbDataAdapter()
        {
            return new MySqlDataAdapter();
        }

        #endregion
    }
}