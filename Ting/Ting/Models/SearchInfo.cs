using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ting.Models
{
    public class BookInfo
    {
        /// <summary>
        /// 书名
        /// </summary>
        public string bookname { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string link { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string anthor { get; set; }
        /// <summary>
        /// 播音员
        /// </summary>
        public string announcer { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string desc { get; set; }
    }
}
