using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace  Z.Tools
{
    public class RegEx
    {
        /// <summary>
        /// 从源中匹配所有内容
        /// </summary>
        /// <returns>匹配的数据.</returns>
        /// <param name="sourceData"> 源文本.</param>
        /// <param name="regexPrefix"> 匹配前缀.</param>
        /// <param name="regexPostFix">匹配后缀.</param>
        public static List<string> FindAll(string sourceData, string regexPrefix, string regexPostFix)
        {
            return FindAll(sourceData, regexPrefix, regexPostFix, false, true);
        }

        /// <summary>
        /// 从源中匹配所有内容.
        /// </summary>
        /// <returns>匹配的数据.</returns>
        /// <param name="sourceData">源文本.</param>
        /// <param name="regexPattern">正则表达式.</param>
        /// <param name="ignoreCase">If set to <c>true</c> 忽略大小写.</param>
        public static List<string> FindAll(string sourceData, string regexPattern, bool ignoreCase)
        {
            List<string> result = new List<string>();

            MatchCollection matches;

            if (ignoreCase)
            {
                matches = Regex.Matches(sourceData, regexPattern, RegexOptions.IgnoreCase);
            }
            else
            {
                matches = Regex.Matches(sourceData, regexPattern);
            }

            foreach (Match matchItem in matches)
            {
                result.Add(matchItem.Value);
            }

            return result;

        }

        /// <summary>
        /// 从源中匹配所有内容
        /// </summary>
        /// <returns>匹配的数据.</returns>
        /// <param name="sourceData">源文本.</param>
        /// <param name="regexPrefix">匹配前缀.</param>
        /// <param name="regexPostFix">匹配后缀.</param>
        /// <param name="OnlyDigit">If set to <c>true</c> 只提取数字.</param>
        public static List<string> FindAll(string sourceData, string regexPrefix, string regexPostFix, bool OnlyDigit)
        {
            return FindAll(sourceData, regexPrefix, regexPostFix, OnlyDigit, true);
        }

        /// <summary>
        /// 从源中匹配所有内容
        /// </summary>
        /// <returns>匹配的数据.</returns>
        /// <param name="sourceData">源文本.</param>
        /// <param name="regexPreFix">匹配前缀.</param>
        /// <param name="regexPostFix">匹配后缀.</param>
        /// <param name="OnlyDigit">If set to <c>true</c> 只提取数字.</param>
        /// <param name="ignoreCase">If set to <c>true</c> 忽略大小写.</param>
        public static List<string> FindAll(string sourceData, string regexPreFix, string regexPostFix, bool OnlyDigit, bool ignoreCase)
        {
            List<string> result = new List<string>();

            MatchCollection matches;

            if (ignoreCase)
            {
                matches = Regex.Matches(sourceData,
                                        regexPreFix + (OnlyDigit ? @"(\d*?)" : @"(.*?)") + regexPostFix,
                                        RegexOptions.IgnoreCase);
            }
            else
            {
                matches = Regex.Matches(sourceData,
                                        regexPreFix + (OnlyDigit ? @"(\d*?)" : @"(.*?)") + regexPostFix);
            }

            foreach (Match matchItem in matches)
            {
                result.Add(matchItem.Value
                           .Replace(regexPreFix, "")
                           .Replace(regexPostFix, ""));
            }

            return result;

        }

        private RegEx() { }

    }
}
