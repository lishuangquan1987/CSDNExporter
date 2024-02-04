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
        public async static Task<OperationResult<List<ArticleInfo>?>> GetArticleInfos(string userName,string cookies)
        {
            //先获取一条，看看总数有多少 
            var url = string.Format(get_article_info_url, 1, 1, userName);
            try
            {
                //var cookies = @"UN=lishuangquan1987; _ga=GA1.2.1660620909.1643291806; _ga_VHSCGE70LW=GS1.1.1644138453.9.1.1644138453.0; __bid_n=1845264295a69b6b164207; FPTOKEN=W6JBjDLNtL58Va6SAokA6m7PLf9eic3HVuxvttahS2UkteVxrujSwA4FlQoC8eIrwhOcSfoq4po21Wo8WB2LTl2vGErIKNT9wIcerufjl8pTletHEWjx+UUBYO+/to4paOgg05ZUS51MLhA5aYlpeOwztREhFa9EYDc6dteCBcf08YTukJO+yurlExmVZokVhaSqecnmMw9YzS1MCC788Txwpo8MUC9lXuHAT8Vvw+Z100wMjIpfD1fKpxcd+cO+TbzZ/2v4Ybh1ihzarWmikDIAff4F3nQRZQjcdIYhRnQ+odpuSG0+dLMhSV6SkzrFTmisRU2vdoUAoXlDCKcLaxJz+MA/psbnYAVReJhwu2VrvlQphTZ6ZUynEoi2kn1fXmIE0XmbjprDhueo7fxOx4R8PdvqJOe0eGbfaGcfu3MaLrWYA1VhnilZFVBoojn4|zBmfWpxdin1X5WW/gZbqaKqyg6tUIJM7LOM7lNRopOg=|10|0bbcc099941be73f3ba6e24268f33949; p_uid=U010000; Hm_lvt_e5ef47b9f471504959267fd614d579cd=1705074105; ssxmod_itna=Yqfx2DyDBDnD0ACGCDz2QnmwBiy6qw4P0QqODlOiixA5D8D6DQeGTW2+tWqtPiYi2hWKoZWx2h3KqQG4x4D9GbgtQDU4i8DCuWITD4fKGwD0eG+DD4DWfx03DoxGYBGx0bSyuLvzQGRD0YDzqDgD7QVbqDEDG3D03=bDo6RDYQDGqDSelR/DD3Df+=5DDN5tc54tkDDbo=gR+iVQQTbmuDqbqDMD7tD/+Ds1Y6dV=750Ttk2paWKTixxBQD7drB9RiDCh=ZAqQFghxWKGx39OGrYiPbQixozGmA3sDopDhIWBDzW0GTpAVxePuphxSDN2Ux8DxD=; ssxmod_itna2=Yqfx2DyDBDnD0ACGCDz2QnmwBiy6qw4P0QqD61+B7D05xQ03t2LUnQ2e6n32o6KeTRND9nRiQsDP+B54m238QAUvruxQO9w4u4=oofnef=FO0vXbdEhwoOqpZcmpNtyCkgfx+V/N1KY3RWGv7DAp9e20qPkvmbnoutCDn8rQ3pl=ev+GTD7Q58xGcDiQ5eD=; c_dl_prid=-; c_dl_rid=1705074916203_654496; c_dl_fref=https://html-to-markdown.com/; c_dl_fpage=/download/bjhermes/10997631; c_dl_um=-; loginbox_strategy=%7B%22taskId%22%3A317%2C%22abCheckTime%22%3A1705591181874%2C%22version%22%3A%22ExpA%22%2C%22nickName%22%3A%22lishuangquan1987%22%2C%22blog-threeH-dialog-expa%22%3A1705323001478%7D; __gads=ID=e18045904829cae4-2264a9b9ffd90055:T=1676819825:RT=1705591182:S=ALNI_MZ8j0PB2UgNPpi6qiPCE40c8bJFhg; __gpi=UID=00000c89a76a46ac:T=1688203150:RT=1705591182:S=ALNI_MYd8ib4ZymbN1XgRj96ed47YaI6TQ; uuid_tt_dd=10_19030218890-1705591244761-969116; UserName=lishuangquan1987; UserInfo=e95148cd63784b1ebadb7adf91639024; UserToken=e95148cd63784b1ebadb7adf91639024; UserNick=lishuangquan1987; AU=0EC; BT=1705591257873; Hm_up_6bcd52f51e9b3dce32bec4a3997715ac=%7B%22islogin%22%3A%7B%22value%22%3A%221%22%2C%22scope%22%3A1%7D%2C%22isonline%22%3A%7B%22value%22%3A%221%22%2C%22scope%22%3A1%7D%2C%22isvip%22%3A%7B%22value%22%3A%220%22%2C%22scope%22%3A1%7D%2C%22uid_%22%3A%7B%22value%22%3A%22lishuangquan1987%22%2C%22scope%22%3A1%7D%7D; lishuangquan1987comment_new=1705328565590; Hm_lvt_6bcd52f51e9b3dce32bec4a3997715ac=1706701406,1706754948,1706793467,1706860039; fpv=29513fc752784e4aeb3ea40cf5130b9e; log_Id_pv=1074; log_Id_click=891; log_Id_view=14588; dc_sid=bdef5ac89d2376b0a613dfd430be04b1; waf_captcha_marker=ce9ef720b825ff88b4960ffbb22b6676248c3625bcdf6d22e1f41c6382a43aab; yd_captcha_token=ycvv5EdDoSs/Qa2xllUxIfKfAvpTzeGLbwFOlnMxTDW83F8EakXcyVTVwyPobiBcSou+dFXY+izmehb6vpJtMg%3D%3D; dc_session_id=10_1707021160932.346455";
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
