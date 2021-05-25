using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Ting.Util
{
    /// <summary>
    /// 请求器
    /// </summary>
    public class TingClient
    {
        public HttpClient httpClient { get; private set; }
        public TingClient(HttpClient _httpClient)
        {
            //配置自定义属性
            _httpClient.BaseAddress = new Uri("http://m.ting56.com");
            _httpClient.DefaultRequestHeaders.Add("Referer", "http://m.ting56.com/");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Mobile Safari/537.36");
            httpClient = _httpClient;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        public async Task<string> Get(string module)
        {
            var buff = await httpClient.GetByteArrayAsync(module);
            string content = Encoding.Default.GetString(buff);
            string result = GetEncodingByContent(content).GetString(buff);
            return result;
        }





        /// <summary>
        /// 根据返回文档获取编码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static Encoding GetEncodingByContent(string content)
        {
            string charset = string.Empty;
            Regex regex = new Regex(@"<meta[\s\S]+?charset=((.*?))""[\s\S]+?>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Match match = regex.Match(content);
            if (match.Success)
            {
                if (!string.IsNullOrEmpty(match.Groups[1].ToString())) charset = match.Groups[1].ToString();
                else
                {
                    int startIndex = match.Value.ToLower().IndexOf("\"");
                    int endIndex = match.Value.ToLower().LastIndexOf("\"");
                    charset = match.Value.Substring(startIndex + 1, endIndex - startIndex - 1);
                    if (charset.Length > 8) return Encoding.Default;
                }
                return Encoding.GetEncoding(charset);
            }
            return Encoding.Default;
        }





        #region httphelper


        /// <summary>
        /// 使用指定的编码 <see cref="Encoding"/> 对 URL 进行编码
        /// </summary>
        /// <param name="url">要编码的地址</param>
        /// <param name="encoding">编码类型,默认UTF-8</param>
        /// <returns>编码后的地址</returns>
        public static string UrlEncode(string url, Encoding encoding = null)
        {
            return HttpUtility.UrlEncode(url, encoding ??= Encoding.Default);
        }



        /// <summary>
        /// 使用指定的编码 <see cref="Encoding"/> 对 URL 进行解码
        /// </summary>
        /// <param name="url">要解码的地址</param>
        /// <param name="encoding">编码类型 默认UTF-8</param>
        /// <returns>编码后的地址</returns>
        public static string UrlDecode(string url, Encoding encoding = null)
        {
            return HttpUtility.UrlDecode(url, encoding ??= Encoding.Default);
        }

        #endregion


    }

}