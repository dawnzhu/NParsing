using System;
using DotNet.Standard.NParsing.Factory;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ObIndexAttribute : Attribute
    {
        public string Name { get; set; }
        public Sort Sort { get; set; }
        public string FileGroup { get; set; }

        public ObIndexAttribute()
        {
            Sort = Sort.Ascending;
            FileGroup = null;
        }
    }
}
