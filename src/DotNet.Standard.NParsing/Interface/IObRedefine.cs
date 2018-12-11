using System.Collections.Generic;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObRedefine
    {
        void Add<TModel>(string rename);
        IDictionary<string, string> Models { get; }
    }
}
