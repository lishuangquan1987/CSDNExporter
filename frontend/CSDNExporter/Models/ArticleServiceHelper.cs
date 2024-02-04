using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Models
{
    public class ArticleServiceHelper
    {
        public static async Task<ArticleServiceResponse> GetMarkDownStr(string url)
        {
            var request = new ArticleServiceRequest() { Url = url };
            if(!StartService("html_to_markdown_service","",out var errMsg)) throw new Exception(errMsg);
            return await HttpHelper.Post<ArticleServiceResponse>("http://127.0.0.1:8081/get_markdown_string",request);
        }
        private static object lockObj = new object();
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="name"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool StartService(string name, string parameter, out string errMsg)
        {
            errMsg = "";
            try
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var process = Process.GetProcessesByName(name);
                if (process == null || process.Length == 0)
                {
                    lock (lockObj)
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = Path.Combine(dir, $"{name}.exe");
                        p.StartInfo.Arguments = parameter;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.WorkingDirectory = dir;

                        if (p.Start())
                        {
                            Thread.Sleep(100);//等待程序启动
                            return true;
                        }
                        else
                        {
                            errMsg = $"启动 {name} 失败";
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return false;
            }
        }
    }
}
