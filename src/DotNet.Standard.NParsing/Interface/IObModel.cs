namespace DotNet.Standard.NParsing.Interface
{
    public interface IObModel
    {
        /// <summary>
        /// 判断属性是否有效
        /// </summary>
        /// <returns></returns>
        bool IsPropertyValid(string propertyName);
    }
}
