using System;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ObPropertyAttribute : Attribute
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public int Length { get; set; }
        
        /// <summary>
        /// 数据长度精确到小数位数
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// 可为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 可修改
        /// </summary>
        public bool Modifiable { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        public ObPropertyAttribute()
        {
            Nullable = true;
            Modifiable = true;
            Description = "";
        }
    }
}
