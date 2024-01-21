using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Models
{
    internal class CSDNHelper
    {
        const string get_article_info_url = "https://blog.csdn.net/community/home-api/v1/get-business-list?page={0}&size={1}&businessType=blog&orderby=&noMore=false&year=&month=&username={2}";
        public static OperationResult<List<ArticleInfo>?> GetArticleInfos(string userName)
        {
            //先获取一条，看看总数有多少 
            var url = string.Format(get_article_info_url, 1, 1, userName);
            try
            {
                var result = HttpHelper.Get<GetArticleInfoResult>(url);
                if (result?.Code != 200)
                {
                    return new OperationResult<List<ArticleInfo>?>() { IsSuccess = false, ErrorMsg = result?.Message };
                }

                var total = result.Data.Total;

                url = string.Format(get_article_info_url, 1, total, userName);

                return new OperationResult<List<ArticleInfo>?>() { IsSuccess = true, Data = HttpHelper.Get<GetArticleInfoResult>(url)?.Data.List }; 
            }
            catch (Exception e)
            {
                return new OperationResult<List<ArticleInfo>?>() { IsSuccess = false, ErrorMsg = e.Message };
            }
        }
    }
}
