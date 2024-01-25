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
        public static async Task<string> Get(string url, object? query = null, Dictionary<string, string> extraHeader = null)
        {
            var request = CreateHttpRequest(url, query, WebRequestMethods.Http.Get);
            if (extraHeader != null && extraHeader.Count > 0)
            {
                foreach (var key in extraHeader.Keys)
                {
                    request.Headers.Add(key, extraHeader[key]);
                }
            }
            using (HttpWebResponse? response =await request.GetResponseAsync() as HttpWebResponse)
            {
                var stream = response?.GetResponseStream();
                if (stream == null) return "";
                //以流的形式读取，返回的就是字符串的json格式
                StreamReader reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }

        }
        public static async Task<T?> Get<T>(string url, object? query = null, Dictionary<string, string> extraHeader = null)
        {
            var str =await Get(url, query, extraHeader);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
        }
        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">写入body的数据，会转换为Json</param>
        /// <param name="query">拼接到地址的参数</param>
        /// <param name="extraHeader">额外的头部</param>
        /// <returns></returns>
        public static async Task<string> Post(string url, object data, object query = null, Dictionary<string, string> extraHeader = null)
        {
            try
            {
                var request = CreateHttpRequest(url, query, WebRequestMethods.Http.Post);

                if (extraHeader != null && extraHeader.Count > 0)
                {
                    foreach (var key in extraHeader.Keys)
                    {
                        request.Headers.Add(key, extraHeader[key]);
                    }
                }

                //创建参数
                string str = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(str);
                request.ContentLength = byteData.Length;

                //以流的形式附加参数
                using (Stream postStream =await request.GetRequestStreamAsync())
                {
                    await postStream.WriteAsync(byteData, 0, byteData.Length);
                }

                //接收来自restful的回复
                string json = string.Empty;  //返回的类型是json格式字符串，声明一个来接收
                using (HttpWebResponse response =await request.GetResponseAsync() as HttpWebResponse)
                {
                    //以流的形式读取，返回的就是字符串的json格式
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    json =await reader.ReadToEndAsync();
                }
                return json;
            }
            catch (System.Net.WebException webException)
            {
                HttpWebResponse response = webException.Response as HttpWebResponse;
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    using (response)
                    {
                        //以流的形式读取，返回的就是字符串的json格式
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        return await reader.ReadToEndAsync();
                    }
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static async Task< T> Post<T>(string url, object data, object query = null, Dictionary<string, string> extraHeader = null)
        {
            var str =await Post(url, data, query, extraHeader);
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
