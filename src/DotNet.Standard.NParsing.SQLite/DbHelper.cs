using System.Data.Common;
using System.Data.SQLite;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.SQLite
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
            return new SQLiteConnection(_connectionString);
        }

        public DbCommand DbCommand()
        {
            return new SQLiteCommand();
        }

        public DbDataAdapter DbDataAdapter()
        {
            return new SQLiteDataAdapter();
        }

        #endregion
    }
}