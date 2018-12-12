using System.Globalization;
using System.Text;

namespace DotNet.Standard.Common.Utilities
{
    public class StringUtil
    {
        /// <summary>
        /// 删除不可见字符
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string DeleteUnVisibleChar(string sourceString)
        {
            var sBuilder = new StringBuilder(131);
            for (var i = 0; i < sourceString.Length; i++)
            {
                int unicode = sourceString[i];
                if (unicode >= 16)
                {
                    sBuilder.Append(sourceString[i].ToString(CultureInfo.InvariantCulture));
                }
            }
            return sBuilder.ToString();
        }
    }
}