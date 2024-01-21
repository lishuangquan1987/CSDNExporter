using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Models
{
    /// <summary>
    /// 文章信息
    /// </summary>
    public class ArticleInfo
    {
        public int ArticleId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? url { get; set; }
        public int Type { get; set; }
        public bool Top { get; set; }
        public bool ForcePlan { get; set; }
        public bool ViewCount { get; set; }
        public int CommentCount { get; set; }
        public string? EditUrl { get; set; }
        public string? PostTime { get; set; }
        public int DiggCount { get; set; }
        public string? FormatTime { get; set; }
        public string[]? PicList { get; set; }
        public int CollectCount { get; set; }
    }
}
