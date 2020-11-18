namespace DotNet.Standard.NParsing.Utilities
{
    /// <summary>
    /// 逻辑运算符
    /// </summary>
    public enum DbSymbol
    {
        /// <summary>
        /// 等于 a == b
        /// </summary>
        Equal, 

        /// <summary>
        /// 不等于 a <> b
        /// </summary>
        NotEqual,

        /// <summary>
        /// 小于等于 a <= b
        /// </summary>
        LessEqual, 

        /// <summary>
        /// 大于等于 a >= b
        /// </summary>
        ThanEqual, 

        /// <summary>
        /// 小于 a < b
        /// </summary>
        Less, 

        /// <summary>
        /// 大于 a > b
        /// </summary>
        Than,   

        /// <summary>
        /// 模糊匹配 LIKE '%abc%'
        /// </summary>
        Like, 

        /// <summary>
        /// 左边模糊匹配 LIKE '%abc'
        /// </summary>
        LikeLeft,

        /// <summary>
        /// 右边模糊匹配 LIKE 'abc%'
        /// </summary>
        LikeRight,

        /// <summary>
        /// NOT LIKE '%abc%'
        /// </summary>
        NotLike,

        /// <summary>
        /// NOT LIKE '%abc'
        /// </summary>
        NotLikeLeft,

        /// <summary>
        /// NOT LIKE 'abc%'
        /// </summary>
        NotLikeRight,

        /// <summary>
        /// IN (a,b,c)
        /// </summary>
        In,

        /// <summary>
        /// NOT IN (a,b,c)
        /// </summary>
        NotIn,

        /// <summary>
        /// BETWEEN a AND b
        /// </summary>
        Between,

        /// <summary>
        /// NOT BETWEEN a AND b
        /// </summary>
        NotBetween
    }
}