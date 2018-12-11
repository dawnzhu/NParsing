using System;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [Flags]
    public enum ObConstraint
    {
        /// <summary>
        /// 主键约束
        /// </summary>
        PrimaryKey = 1,

        /// <summary>
        /// 外键约束
        /// </summary>
        ForeignKey = 2
    }
}