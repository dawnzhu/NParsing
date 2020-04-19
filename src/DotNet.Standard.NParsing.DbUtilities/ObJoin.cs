using System;
using System.Collections.Generic;
using System.Reflection;
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

    public class ObJoin<TTerm> : ObJoin, IObJoin<TTerm>
        where TTerm : ObTermBase
    {
        private readonly TTerm _term;

        public ObJoin(TTerm term)
        {
            _term = term;
        }

        public ObJoin(TTerm term, ObTermBase obTermBase) : base(obTermBase)
        {
            _term = term;
        }

        public IObJoin<TTerm> AddJoin(Func<TTerm, ObTermBase> keySelector)
        {
            var obTermBase = keySelector(_term);
            AddJoin(obTermBase);
            return this;
        }

        public IObJoin<TTerm> AddJoin(Func<TTerm, ObTermBase[]> keySelector)
        {
            var obTermBases = keySelector(_term);
            AddJoin(obTermBases);
            return this;
        }

        public IObJoin<TTerm> AddJoin<TKey>(Func<TTerm, TKey> keySelector)
        {
            var list = new List<ObTermBase>();
            var key = keySelector(_term);
            if (key is ObTermBase value)
            {
                list.Add(value);
            }
            else
            {
                foreach (var propertyInfo in key.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var k = propertyInfo.GetValue(key);
                    if (k is ObTermBase value2)
                    {
                        list.Add(value2);
                    }
                }
            }
            base.AddJoin(list.ToArray());
            return this;
        }
    }
}
