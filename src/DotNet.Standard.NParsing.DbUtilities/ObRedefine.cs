using System;
using System.Collections.Generic;
using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObRedefine : IObRedefine
    {
        public ObRedefine()
        {
            Models = new Dictionary<string, string>();
        }

        public ObRedefine(Type t, string rename)
        {
            Models = new Dictionary<string, string> {{t.ToTableName(), rename}};
        }

        public void Add<TModel>(string rename)
        {
            Models.Add(typeof(TModel).ToTableName(), rename);
        }

        public IDictionary<string, string> Models { get; }
    }
}
