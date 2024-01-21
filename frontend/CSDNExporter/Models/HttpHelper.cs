using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Models
{
    internal class HttpHelper
    {
        public static string Get(string url, object? query = null, Dictionary<string, string> extraHeader = null)
        {
            var request = CreateHttpRequest(url, query, WebRequestMethods.Http.Get);
            if (extraHeader != null && extraHeader.Count > 0)
            {
                foreach (var key in extraHeader.Keys)
                {
                    request.Headers.Add(key, extraHeader[key]);
                }
            }
            using (HttpWebResponse? response = request.GetResponse() as HttpWebResponse)
            {
                var stream = response?.GetResponseStream();
                if (stream == null) return "";
                //以流的形式读取，返回的就是字符串的json格式
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }

        }
        public static T? Get<T>(string url, object? query = null, Dictionary<string, string> extraHeader = null)
        {
            var str = Get(url, query, extraHeader);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
        }
        /// <summary>
        /// 创建http的请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="query"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpRequest(string url, object query, string method)
        {
            if (query != null)
            {
                if (url.EndsWith("/"))
                {
                    url = $"{url.TrimEnd('/')}?";
                }
                else
                {
                    url = $"{url}?";
                }
                var dic = GetProperties(query);
                string queryParameter = "";
                foreach (var key in dic.Keys)
                {
                    if (dic[key] == null)
                    {
                        queryParameter += $"&{key}=";//url中空值不能写a=null,而要写a=
                    }
                    else if (dic[key] is IEnumerable && !(dic[key] is string))//处理数组的问题,字符串也是一种数组
                    {
                        IEnumerable values = dic[key] as IEnumerable;
                        foreach (var value in values)
                        {
                            queryParameter += $"&{key}={value}";
                        }
                    }
                    else if (dic[key] is string)
                    {
                        queryParameter += $"&{key}={dic[key]}";
                    }
                    else if (dic[key] is DateTime)
                    {
                        queryParameter += $"&{key}={dic[key]?.ToString()}";
                    }
                    else //处理Object的问题
                    {
                        queryParameter += $"&{key}={Newtonsoft.Json.JsonConvert.SerializeObject(dic[key])}";
                    }
                }
                if (!string.IsNullOrEmpty(queryParameter))
                {
                    url += queryParameter.TrimStart('&');
                }
            }

            //创建restful的请求
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = method;
            request.ContentType = "application/json";
            return request;
        }

        private static Dictionary<string, object> GetProperties(object obj)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            foreach (var property in obj.GetType().GetProperties())
            {
                //这里要看有没有JsonProperty标签，有的话就要变成标签里的值
                var propertyName = property.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? property.Name;
                dic.Add(propertyName, property.GetValue(obj, null));
            }
            return dic;
        }
    }
}
