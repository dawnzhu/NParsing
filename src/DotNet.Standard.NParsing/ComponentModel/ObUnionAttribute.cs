using System;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ObUnionAttribute : Attribute
    {
        public Type[] ModelTypes { get; }
        public ObUnionAttribute(params Type[] modelTypes)
        {
            ModelTypes = modelTypes;
        }
    }
}