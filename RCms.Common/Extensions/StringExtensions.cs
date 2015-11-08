using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RCms.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// replace \n | \r\n on <br/>
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string ReplaceLineBreakToBrTag(this string plainText)
        {
            return Regex.Replace(plainText ?? "", @"\n|\r\n?", "<br/>");
        }

        /// <summary>
        /// convert Html to plain text using HtmlAgilityPach lib.
        /// Return empty string if any error occured.
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string HtmlToPlainText(this string html)
        {
            return (new HtmlToTextViaHtmlAgilityPack()).HtmlToPlainText(html);
        }

        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string SafeStringForPatch(this string s)
        {
            s = s ?? "";
            return new string(s.Where(c => c != '{' && c != '}' && c != '\'' && c != '\"' && c != '\\' && c != '/').ToArray());
        }
    }
}
