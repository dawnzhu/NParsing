using System.Collections.Generic;

namespace DotNet.Standard.NParsing.Interface
{
    public interface IObJoin
    {
        IList<string> JoinModels { get; }

        IObJoin AddJoin(ObTermBase obTermBase);

        IObJoin AddJoin(params ObTermBase[] obTermBases);
    }
}
