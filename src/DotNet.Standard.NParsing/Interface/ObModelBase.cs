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
        private IList<string> _obValidProperties;

        protected ObModelBase()
        {
            _obValidProperties = new List<string>();
        }

        protected virtual void SetPropertyValid(MethodBase currentMethod)
        {
            SetPropertyValid(currentMethod.Name.Substring(4));
        }

        protected void SetPropertyValid(string propertyName)
        {
            if (_obValidProperties == null)
                _obValidProperties = new List<string>();
            if (!_obValidProperties.Contains(propertyName))
            {
                _obValidProperties.Add(propertyName);
            }
        }

        public bool IsPropertyValid(string propertyName)
        {
            return _obValidProperties != null &&_obValidProperties.Contains(propertyName);
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

        public ObTermBase(ObTermBase parent, MethodBase currentMethod) : base(typeof(TModel), parent, currentMethod)
        {
        }
    }

    public abstract class ObTermBase
    {
        private readonly List<string> _notJoinModels;
        public IObRedefine ObRedefine { get; }
        public Type ModelType { get; }
        public string ObTableName { get; }

        protected ObTermBase(Type modelType)
        {
            ModelType = modelType;
            _notJoinModels = new List<string>();
            ObRedefine = null;
            ObTableName = ModelType.ToTableName();
        }

        protected ObTermBase(Type modelType, string rename)
        {
            ModelType = modelType;
            _notJoinModels = new List<string>();
            ObRedefine = Factory.ObRedefine.Create(modelType, rename);
            ObTableName = modelType.ToTableName(ObRedefine.Models);
        }

        protected ObTermBase(Type modelType, ObTermBase parent, MethodBase currentMethod)
        {
            ModelType = modelType;
            _notJoinModels = new List<string>();
            ObRedefine = Factory.ObRedefine.Create(modelType, parent, currentMethod);
            ObTableName = modelType.ToTableName(ObRedefine.Models);
        }

        internal void AddNotJoin(ObTermBase obTermBase)
        {
            _notJoinModels.Add(obTermBase.ModelType.ToTableName(obTermBase.ObRedefine?.Models));
        }

        internal void AddNotJoin(params ObTermBase[] obTermBases)
        {
            /*foreach (var obj in obTermBases)
            {
                if (obj.IObRedefine == null) continue;
                _notJoinModels.Add(obj.ModelType.ToTableName(obj.IObRedefine.Models));
            }*/
            _notJoinModels.AddRange(obTermBases.Select(obj => obj.ModelType.ToTableName(obj.ObRedefine?.Models)));
        }

        public IObHelper<TModel> Helper<TModel>(string connectionString, string providerName)
        {
            if (typeof(TModel) != ModelType)
            {
                throw new Exception("模型类型不正确");
            }
            return ObHelper.Create<TModel>(connectionString, providerName, ObRedefine, _notJoinModels);
        }

        public IObHelper<IObModel> Helper(string connectionString, string providerName)
        {
            return ObHelper.Create(ModelType, connectionString, providerName, ObRedefine, _notJoinModels);
        }

        public IObHelper<TModel> Helper<TModel>(string readConnectionString, string writeConnectionString, string providerName)
        {
            if (typeof(TModel) != ModelType)
            {
                throw new Exception("模型类型不正确");
            }
            return ObHelper.Create<TModel>(readConnectionString, writeConnectionString, providerName, ObRedefine, _notJoinModels);
        }

        public IObHelper<IObModel> Helper(string readConnectionString, string writeConnectionString, string providerName)
        {
            return ObHelper.Create(ModelType, readConnectionString, writeConnectionString, providerName, ObRedefine, _notJoinModels);
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
