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
        public async static Task<OperationResult<List<ArticleInfo>?>> GetArticleInfos(string userName)
        {
            //先获取一条，看看总数有多少 
            var url = string.Format(get_article_info_url, 1, 1, userName);
            try
            {
                var cookies = @"uuid_tt_dd=10_21033657930-1655984758752-338217; Hm_up_6bcd52f51e9b3dce32bec4a3997715ac=%7B%22islogin%22%3A%7B%22value%22%3A%220%22%2C%22scope%22%3A1%7D%2C%22isonline%22%3A%7B%22value%22%3A%220%22%2C%22scope%22%3A1%7D%2C%22isvip%22%3A%7B%22value%22%3A%220%22%2C%22scope%22%3A1%7D%7D; Hm_lvt_6bcd52f51e9b3dce32bec4a3997715ac=1697638672,1699784960; __gads=ID=25288461fb916e41:T=1697638687:RT=1699784962:S=ALNI_Mbkx3y1W3EBTpgKIYforK7BiYy01w; __gpi=UID=00000c66e67b9df4:T=1697638687:RT=1699784962:S=ALNI_MaF7mqxyTCgsugximeE48shA3uCLA; log_Id_pv=11; log_Id_click=8; log_Id_view=113; waf_captcha_marker=6dff200e4cf12a93c80788f991c3cfde803e4b6df897bdfd732926bdf635daa7; fpv=05607ea9279f3bf7a5e1743ebd86dd8d; dc_session_id=10_1706175279873.963571; yd_captcha_token=ycvv5UZGpSgySqm9nwBkdPefUPwGx7HTPAMdkCVnHTPqjw5ROBLenF3UxSG8biRdRo7jdFTZ9irjehT8vpFvOg%3D%3D; dc_sid=eb71a6df395a1582557bc276310f115d";
                var result = await HttpHelper.Get<GetArticleInfoResult>(url, null, new Dictionary<string, string>()
                {
                    { "Cookie",cookies}
                });
                if (result?.Code != 200)
                {
                    return new OperationResult<List<ArticleInfo>?>() { IsSuccess = false, ErrorMsg = result?.Message };
                }

                var total = result.Data.Total;

                url = string.Format(get_article_info_url, 1, total, userName);

                return new OperationResult<List<ArticleInfo>?>()
                {
                    IsSuccess = true,
                    Data = (await HttpHelper.Get<GetArticleInfoResult>(url, null,
                        new Dictionary<string, string>()
                        {
                            { "Cookie", cookies }
                        })
                    )?.Data.List.OrderByDescending(x => x.ArticleId).ToList()
                };
            }
            catch (Exception e)
            {
                return new OperationResult<List<ArticleInfo>?>() { IsSuccess = false, ErrorMsg = e.Message };
            }
        }
    }
}
