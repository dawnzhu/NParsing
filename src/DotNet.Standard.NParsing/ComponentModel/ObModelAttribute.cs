using System;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class ObModelAttribute : Attribute
    {
        /// <summary>
        /// 模型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 附加信息
        /// </summary>
        public string Extra { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
    }
}
