using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.BlogResolver
{
    /// <summary>
    /// 提供对内容过滤，只提取博客的有效内容
    /// </summary>
    public class ContentResolver : IBlogResolver
    {
        public async Task< string[]> Resolve(string[] content,object context)
        {
            List<string> result = new List<string>(content);
            //步骤1：去除头部
            result.RemoveAt(0);
            result.RemoveAt(0);

            var content_本文链接 = result.FirstOrDefault(x => x.StartsWith("本文链接"));
            if (content_本文链接 != null)
            {
                var index = result.IndexOf(content_本文链接);
                result.RemoveRange(0, index);
            }

            //步骤2：去除尾部
            var content_优惠劵 = result.FirstOrDefault(x => x.StartsWith("优惠劵"));
            if (content_优惠劵 != null)
            {
                var index = result.IndexOf(content_优惠劵);
                result.RemoveRange(index, result.Count-index);
            }

            //代码标签去掉prism
            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].Contains("```prism"))
                {
                    result[i] = result[i].Replace("```prism", "```");
                }
            }

            return await Task.FromResult( result.ToArray());
        }
    }
}
