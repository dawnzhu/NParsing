namespace DotNet.Standard.NParsing.Interface
{
    public interface IObModel
    {
        /// <summary>
        /// 判断对象属性是否被赋值
        /// </summary>
        /// <returns></returns>
        bool IsPropertyValid(string propertyName);
    }
}
