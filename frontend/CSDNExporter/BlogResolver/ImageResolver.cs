using CSDNExporter.Models;
using Microsoft.VisualBasic;
using System.IO;
using System.Text.RegularExpressions;

namespace CSDNExporter.BlogResolver
{
    public class ImageResolver : IBlogResolver
    {
        public async Task<string[]> Resolve(string[] contents, object context)
        {
            var dic = context as Dictionary<string, object>;
            var path = dic["Path"] as string;
            var contents_copy = contents.ToArray();

            var baseDir=Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var imageDir=Path.Combine(baseDir, fileName);

            if(!Directory.Exists(imageDir)) Directory.CreateDirectory(imageDir);
            

            Regex imageRegex = new Regex(@"!\[\S*]\((\S+)\)");
            int imageNumber = 1;
            for(int i=0;i<contents.Length;i++)
            {
                var content = contents[i];
                var matchResult=imageRegex.Match(content);
                if (!matchResult.Success) continue;

                if (matchResult.Groups.Count != 2) continue;

                var imageUrl = matchResult.Groups[1].Value;
                
                var imagePath = Path.Combine(imageDir,$"{imageNumber}.png");
                try
                {
                    //1.下载图片，取一个数字名称（递增），存入文章名称文件夹
                    var bytes =await HttpHelper.GetFileAsync(imageUrl);
                    File.WriteAllBytes(imagePath, bytes);
                    //2.将图片链接替换为本地链接,使用相对路径
                    contents_copy[i] = contents_copy[i].Replace(imageUrl, @$".\{fileName}\{imageNumber}.png");
                    imageNumber++;
                }
                catch (Exception e)
                {
                    continue;
                }                
            }
            return contents_copy;
        }
        
    }
}
