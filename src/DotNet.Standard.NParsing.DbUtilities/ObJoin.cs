using DotNet.Standard.NParsing.Interface;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.DbUtilities
{
    public class ObJoin : ObJoinBase
    {
        public ObJoin()
        { }

        public ObJoin(ObTermBase obTermBase)
        {
            JoinModels.Add(obTermBase.ModelType.ToTableName(obTermBase.ObRedefine?.Models));
        }

        public override IObJoin AddJoin(ObTermBase obTermBase)
        {
            JoinModels.Add(obTermBase.ModelType.ToTableName(obTermBase.ObRedefine?.Models));
            return this;
        }

        public override IObJoin AddJoin(params  ObTermBase[] obTermBases)
        {
            foreach (var obTermBase in obTermBases)
            {
                JoinModels.Add(obTermBase.ModelType.ToTableName(obTermBase.ObRedefine?.Models));
            }
            return this;
        }
    }
}
