namespace DotNet.Standard.NParsing.Utilities
{
    /// <summary>
    /// 逻辑运算符
    /// </summary>
    public enum DbSymbol
    {
        Equal,      //=
        NotEqual,   //<>
        LessEqual,  //<=
        ThanEqual,  //>=
        Less,       //<
        Than,       //>
        Like, 
        LikeLeft,
        LikeRight,
        NotLike,
        NotLikeLeft,
        NotLikeRight,
        In,
        NotIn,
        Between,
        NotBetween
    }
}