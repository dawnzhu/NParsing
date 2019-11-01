using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DotNet.Standard.NParsing.ComponentModel
{
    public class ObException : Exception
    {
        public ObException(Exception er, CommandType commandType, string commandText, DbParameter[] commandParameters) : base(er.Message, er)
        {
            CurrentExeSql = (commandType, commandText, commandParameters);
        }

        public (CommandType Type, string Text, IList<DbParameter> Parameters) CurrentExeSql { get; }

    }
}
