using System;

namespace DotNet.Standard.NParsing.ComponentModel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ObConstraintAttribute : Attribute
    {
        /// <summary>
        /// 约束类型
        /// </summary>
        public ObConstraint ObConstraint { get; }

        /// <summary>
        /// 引用类
        /// </summary>
        public Type Refclass { get; set; }

        /// <summary>
        /// 引用属性
        /// </summary>
        public string Refproperty { get; set; }

        /// <summary>
        /// 对应属性
        /// </summary>
        public string Property { get; set; }

/*        /// <summary>
        /// 对应主键名称 “对象名.属性名”
        /// </summary>
        public string TableName_PrimaryKey { get; private set; }*/

        public ObConstraintAttribute(ObConstraint obConstraint)
        {
            ObConstraint = obConstraint;
            //Refproperty = null;
            //Refclass = null;
            //Property = null;
/*            if (Refclass != null && Refproperty != null)
                TableName_PrimaryKey = Refclass.Name + "." + Refproperty;
            else
                TableName_PrimaryKey = null;*/
        }

        //public ObConstraintAttribute(ObConstraint obConstraint, string primaryKey)
        //{
        //    ObConstraint = obConstraint;
        //    PrimaryKey = primaryKey;
        //}
    }
}
