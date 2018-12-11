using System;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ObSettledAttribute : Attribute
    {
        public object Value { get; set; }
    }
}
