namespace DotNet.Standard.NParsing.Utilities
{
    /// <summary>
    /// 算术运算符
    /// </summary>
    public enum DbAriSymbol
    {
        Null,

        /// <summary>
        /// 加 a+b
        /// </summary>
        Add,

        /// <summary>
        /// 减 a-b
        /// </summary>
        Subtract,

        /// <summary>
        /// 乘 a*b
        /// </summary>
        Multiply,

        /// <summary>
        /// 除 a/b
        /// </summary>
        Divide,

        /// <summary>
        /// 取余 a%b
        /// </summary>
        Modulo,

        /// <summary>
        /// 按位与 a&b
        /// </summary>
        And,

        /// <summary>
        /// 按拉或 a|b
        /// </summary>
        Or,

        /// <summary>
        /// 异或 a^b
        /// </summary>
        ExclusiveOr,

        /// <summary>
        /// 按位取反 ~a
        /// </summary>
        Not,

        /// <summary>
        /// 左移 a<<b
        /// </summary>
        LeftShift,

        /// <summary>
        /// 右移 a>>b
        /// </summary>
        RightShift,

        /// <summary>
        /// 取反 -a
        /// </summary>
        Negate
    }
}