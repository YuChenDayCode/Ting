using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NSwag.Annotations;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Ting.Models;
using Ting.Util;

namespace Ting.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class TingController : ControllerBase
    {
        TingClient tingClient;
        Encoding encode = Encoding.GetEncoding("gb2312");

        public TingController(TingClient _tingClient) => this.tingClient = _tingClient;



        /// <summary>
        /// 获取播放改地址
        /// </summary>
        /// <param name="moudle"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetPlayUrl")]
        [OpenApiTag("获取播放地址", Description = "获取播放地址")]
        public string GetPlayUrl(string moudle)
        {
            //string moudle = "/video/20436-0-920.html";
            tingClient.httpClient.BaseAddress = new System.Uri("http://ting56.com");
            string result = tingClient.Get(moudle).Result;

            Match match = Regex.Match(result, @"<script>var datas=\(FonHen_JieMa\('(.*?)'\).*var part='(.*?)'; var play_vid='(.*?)';</script>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string title = match.Groups[2].Value.Replace("\t","");
                return JsonConvert.SerializeObject(new { url = match.Groups[1].Value, title = title, epis = title.Substring(title.LastIndexOf("-") + 1), vid = match.Groups[3].Value });
            }
            return "";
        }

        [HttpGet]
        [Route("GetRanking")]
        [OpenApiTag("获取热门", Description = "获取热门书籍")]
        public string GetRanking()
        {
            string moudle = "/paihangbang.html";
            string result = tingClient.Get(moudle).Result;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            var sn = doc.DocumentNode.SelectNodes("//div[@class='list-ov-w']");
            var list = new List<BookInfo>();
            foreach (var node in sn)
            {
                var childs = node.ChildNodes;
                list.Add(
                    new BookInfo { announcer = childs[2].InnerText, anthor = childs[1].InnerText, bookname = childs[0].InnerText, desc = childs[3].InnerText, link = childs[0].FirstChild.Attributes["href"].Value });
            }
            string jsonstr = JsonConvert.SerializeObject(list);
            return jsonstr;
        }




        [HttpGet]
        [Route("GetCatalog")]
        [OpenApiTag("获取书籍目录", Description = "获取书籍目录")]
        public string GetCatalog(string moudle)
        {
            if (string.IsNullOrEmpty(moudle)) return JsonConvert.SerializeObject(new { msg = "parament exption" });
            //string module = "mp3/17388.html";
            string result = tingClient.Get(moudle).Result;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            var list = new List<Catalog>();
            var playlist = doc.DocumentNode.SelectNodes("//*[@id='playlist']/ul/li");
            foreach (var node in playlist)
            {
                var node_a = node.FirstChild;

                list.Add(new Catalog
                {
                    catalogname = node_a.Attributes["title"].Value,
                    link = node_a.Attributes["href"].Value
                });
            }

            return JsonConvert.SerializeObject(list);
        }




        /// <summary>
        /// 搜索书籍
        /// </summary>
        /// <param name="bookname">书名</param>
        /// <returns></returns>
        [OpenApiTag("搜索", Description = "搜索书籍")]
        [HttpPost]
        public string Search(string bookname)
        {
            string moudle = "search.asp?searchword=" + TingClient.UrlEncode(bookname, encode).ToUpper();

            string result = tingClient.Get(moudle).Result;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(result);
            var sn = doc.DocumentNode.SelectNodes("//div[@class='list-ov-w']");
            var list = new List<BookInfo>();
            foreach (var node in sn)
            {
                var childs = node.ChildNodes;
                list.Add(
                    new BookInfo { announcer = childs[2].InnerText, anthor = childs[1].InnerText, bookname = childs[0].InnerText, desc = childs[3].InnerText, link = childs[0].FirstChild.Attributes["href"].Value });
            }

            return JsonConvert.SerializeObject(list);
        }

    }
    public class Results
    {
        public int id { get; set; }
        public string title { get; set; }
    }


}
