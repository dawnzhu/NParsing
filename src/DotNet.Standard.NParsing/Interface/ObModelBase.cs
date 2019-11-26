using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Utilities;

namespace DotNet.Standard.NParsing.Interface
{
    [Serializable]
    public class ObModelBase<TModel> : ObModelBase
    {
        protected override void SetPropertyValid(MethodBase currentMethod)
        {
            SetPropertyValid(typeof(TModel).ToPropertyInfo(currentMethod).Name);
        }
    }

    [Serializable]
    public abstract class ObModelBase : IObModel
    {
        public IList<string> ObValidProperties { get; private set; }

        protected ObModelBase()
        {
            ObValidProperties = new List<string>();
        }

        protected void ProxySet(ObModelBase obModelBase)
        {
            ObValidProperties = obModelBase.ObValidProperties;
        }

        protected virtual void SetPropertyValid(MethodBase currentMethod)
        {
            SetPropertyValid(currentMethod.Name.Substring(4));
        }

        protected void SetPropertyValid(string propertyName)
        {
            if (ObValidProperties == null)
                ObValidProperties = new List<string>();
            if (!ObValidProperties.Contains(propertyName))
            {
                ObValidProperties.Add(propertyName);
            }
        }

        public bool IsPropertyValid(string propertyName)
        {
            return ObValidProperties != null && ObValidProperties.Contains(propertyName);
        }
    }

    public class ObTermBase<TModel> : ObTermBase
    {
        public ObTermBase() : base(typeof(TModel))
        {
        }

        public ObTermBase(string rename) : base(typeof(TModel), rename)
        {
        }

        public ObTermBase(ObTermBase parent, string rename) : base(typeof(TModel), parent, rename)
        {
        }

        public ObTermBase(ObTermBase parent, MethodBase currentMethod) : base(typeof(TModel), parent, currentMethod)
        {
        }
    }

    public abstract class ObTermBase
    {
        public List<string> NotJoinModels { get; private set; }
        public IObRedefine ObRedefine { get; private set; }
        public Type ModelType { get; private set; }
        public string ObTableName { get; private set; }

        protected ObTermBase(Type modelType)
        {
            Init(modelType);
        }

        protected ObTermBase(Type modelType, string rename)
        {
            Init(modelType, rename);
        }

        protected ObTermBase(Type modelType, ObTermBase parent, string rename)
        {
            Init(modelType, parent, rename);
        }

        protected ObTermBase(Type modelType, ObTermBase parent, MethodBase currentMethod)
        {
            Init(modelType, parent, currentMethod);
        }

        protected void Init(Type modelType)
        {
            Init(modelType, null);
        }

        protected void Init(Type modelType, string rename)
        {
            Init(modelType, null, rename);
        }

        protected void Init(Type modelType, ObTermBase parent, string rename)
        {
            ModelType = modelType;
            NotJoinModels = new List<string>();
            ObRedefine = rename == null ? null : Factory.ObRedefine.Create(modelType, parent, rename);
            ObTableName = ModelType.ToTableName();
        }

        protected void Init(Type modelType, ObTermBase parent, MethodBase currentMethod)
        {
            ModelType = modelType;
            NotJoinModels = new List<string>();
            ObRedefine = Factory.ObRedefine.Create(modelType, parent, currentMethod);
            ObTableName = modelType.ToTableName(ObRedefine.Models);
        }

        protected void ProxySet(ObTermBase obTermBase)
        {
            ModelType = obTermBase.ModelType;
            ObRedefine = obTermBase.ObRedefine;
            ObTableName = obTermBase.ObTableName;
            NotJoinModels = obTermBase.NotJoinModels;
        }

        internal void AddNotJoin(ObTermBase obTermBase)
        {
            NotJoinModels.Add(obTermBase.ModelType.ToTableName(obTermBase.ObRedefine?.Models));
        }

        internal void AddNotJoin(params ObTermBase[] obTermBases)
        {
            /*foreach (var obj in obTermBases)
            {
                if (obj.IObRedefine == null) continue;
                _notJoinModels.Add(obj.ModelType.ToTableName(obj.IObRedefine.Models));
            }*/
            NotJoinModels.AddRange(obTermBases.Select(obj => obj.ModelType.ToTableName(obj.ObRedefine?.Models)));
        }

        public IObHelper<TModel> Helper<TModel>(string connectionString, string providerName)
        {
            if (typeof(TModel) != ModelType)
            {
                throw new Exception("模型类型不正确");
            }
            return ObHelper.Create<TModel>(connectionString, providerName, ObRedefine, NotJoinModels);
        }

        public IObHelper<IObModel> Helper(string connectionString, string providerName)
        {
            return ObHelper.Create(ModelType, connectionString, providerName, ObRedefine, NotJoinModels);
        }

        public IObHelper<TModel> Helper<TModel>(string readConnectionString, string writeConnectionString, string providerName)
        {
            if (typeof(TModel) != ModelType)
            {
                throw new Exception("模型类型不正确");
            }
            return ObHelper.Create<TModel>(readConnectionString, writeConnectionString, providerName, ObRedefine, NotJoinModels);
        }

        public IObHelper<IObModel> Helper(string readConnectionString, string writeConnectionString, string providerName)
        {
            return ObHelper.Create(ModelType, readConnectionString, writeConnectionString, providerName, ObRedefine, NotJoinModels);
        }

        protected ObProperty GetProperty(MethodBase currentMethod)
        {
            return ObProperty.Create(ModelType, ObRedefine, currentMethod);
        }

        public ObProperty GetProperty(string propertyName)
        {
            return ObProperty.Create(ModelType, ObRedefine, propertyName);
        }
    }

    public static class ObTermFunc
    {
        public static void AddNotJoin<TSource>(this TSource source, Func<TSource, ObTermBase> keySelector) 
            where TSource : ObTermBase
        {
            var obTermBase = keySelector(source);
            source.AddNotJoin(obTermBase);
        }

        public static void AddNotJoin<TSource>(this TSource source, Func<TSource, ObTermBase[]> keySelector) 
            where TSource : ObTermBase
        {
            var obTermBase = keySelector(source);
            source.AddNotJoin(obTermBase);
        }
    }
}
