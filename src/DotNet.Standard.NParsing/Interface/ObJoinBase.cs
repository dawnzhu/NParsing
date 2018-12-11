using System;
using System.Collections.Generic;

namespace DotNet.Standard.NParsing.Interface
{
    public abstract class ObJoinBase : IObJoin
    {
        protected ObJoinBase()
        {
            JoinModels = new List<string>();
        }

        public IList<string> JoinModels { get; }

        public virtual IObJoin AddJoin(ObTermBase obTermBase)
        {
            throw new NotImplementedException();
        }

        public virtual IObJoin AddJoin(params ObTermBase[] obTermBases)
        {
            throw new NotImplementedException();
        }
    }
}
