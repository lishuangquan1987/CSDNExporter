using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Models
{
    public class GetArticleInfoResult
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string TraceId { get; set; }
        public GetArticleInfoData Data { get; set; }
    }
    public class GetArticleInfoData
    {
        public List<ArticleInfo> List { get; set; }
        public int Total { get; set; }
    }
}
