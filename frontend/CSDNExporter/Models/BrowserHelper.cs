using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Models
{
    /// <summary>
    /// 浏览器帮助类
    /// </summary>
    public class BrowserHelper
    {
        #region private const

        /// <summary>
        /// IE浏览器注册表地址
        /// </summary>
        private const string IEAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\iexplore.exe";

        /// <summary>
        /// 谷歌浏览器注册表地址
        /// </summary>
        private const string ChromeAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";

        /// <summary>
        /// 火狐浏览器注册表地址
        /// </summary>
        private const string FirefoxAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe";

        /// <summary>
        /// Edge浏览器注册表地址
        /// </summary>
        private const string MSEdgeAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe";

        #endregion

        #region private static fields

        /// <summary>
        /// 360极速浏览器注册表地址（针对不同电脑上注册表地址不一样，维护多个）
        /// </summary>
        private static readonly string[] Chrome360AppKeys = new string[] {
            "\\Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\360chrome.exe",
            "\\Software\\Microsoft\\Windows\\CurrentVersion\\App Paths\\360se6.exe"};

        #endregion

        #region private static methods

        /// <summary>
        /// 通过默认浏览器打开Url
        /// </summary>
        /// <param name="url">Url地址</param>
        private static void OpenDefaultBrowserUrl(string url)
        {
            Process.Start(url);
        }

        /// <summary>
        /// 通过IE浏览器打开Url
        /// </summary>
        /// <param name="url">Url地址</param>
        private static void OpenIEBrowserUrl(string url)
        {
            try
            {
                // 通过注册表找到IE浏览器安装路径
                string ieAppFileName = (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + IEAppKey, "", null) ?? Registry.GetValue("HKEY_CURRENT_USER" + IEAppKey, "", null));
                // 如果未找到IE浏览器则使用默认浏览器打开
                if (String.IsNullOrWhiteSpace(ieAppFileName))
                {
                    OpenDefaultBrowserUrl(url);
                    return;
                }

                // 打开IE浏览器
                Process.Start(ieAppFileName, url);
            }
            catch
            {
                // 如果发生异常则使用默认浏览器打开
                OpenDefaultBrowserUrl(url);
            }
        }

        /// <summary>
        /// 通过谷歌浏览器打开Url
        /// </summary>
        /// <param name="url">Url地址</param>
        private static void OpenGoogleBrowserUrl(string url)
        {
            try
            {
                // 通过注册表找到谷歌浏览器安装路径
                string chromeAppFileName = (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + ChromeAppKey, "", null) ?? Registry.GetValue("HKEY_CURRENT_USER" + ChromeAppKey, "", null));
                // 如果未找到谷歌浏览器则使用默认浏览器打开
                if (String.IsNullOrWhiteSpace(chromeAppFileName))
                {
                    OpenDefaultBrowserUrl(url);
                    return;
                }

                // 打开谷歌浏览器
                Process.Start(chromeAppFileName, url);
            }
            catch
            {
                // 如果发生异常则使用默认浏览器打开
                OpenDefaultBrowserUrl(url);
            }
        }

        /// <summary>
        /// 通过火狐浏览器打开Url
        /// </summary>
        /// <param name="url">Url地址</param>
        private static void OpenFirefoxBrowserUrl(string url)
        {
            try
            {
                // 通过注册表找到火狐浏览器安装路径
                string firefoxAppFileName = (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + FirefoxAppKey, "", null) ?? Registry.GetValue("HKEY_CURRENT_USER" + FirefoxAppKey, "", null));
                // 如果未找到火狐浏览器则使用默认浏览器打开
                if (String.IsNullOrWhiteSpace(firefoxAppFileName))
                {
                    OpenDefaultBrowserUrl(url);
                    return;
                }

                // 打开火狐浏览器
                Process.Start(firefoxAppFileName, url);
            }
            catch
            {
                // 如果发生异常则使用默认浏览器打开
                OpenDefaultBrowserUrl(url);
            }
        }

        /// <summary>
        /// 通过Edge浏览器打开Url
        /// </summary>
        /// <param name="url">Url地址</param>
        private static void OpenMSEdgeBrowserUrl(string url)
        {
            try
            {
                // 通过注册表找到Edge浏览器安装路径
                string msedgeAppFileName = (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + MSEdgeAppKey, "", null) ?? Registry.GetValue("HKEY_CURRENT_USER" + MSEdgeAppKey, "", null));
                // 如果未找到Edge浏览器则使用默认浏览器打开
                if (String.IsNullOrWhiteSpace(msedgeAppFileName))
                {
                    OpenDefaultBrowserUrl(url);
                    return;
                }

                // 打开Edge浏览器
                Process.Start(msedgeAppFileName, url);
            }
            catch
            {
                // 如果发生异常则使用默认浏览器打开
                OpenDefaultBrowserUrl(url);
            }
        }

        /// <summary>
        /// 通过360极速浏览器打开Url
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="kernelType">浏览器内核模式</param>
        private static void OpenChrome360BrowserUrl(string url, BrowserKernelType kernelType = BrowserKernelType.Node)
        {
            try
            {
                // 通过注册表找到360极速浏览器安装路径(判断电脑是否安装了360浏览器或360极速浏览器)
                foreach (var Chrome360AppKey in Chrome360AppKeys)
                {
                    string chrome360AppFileName = (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + Chrome360AppKey, "", null) ?? Registry.GetValue("HKEY_CURRENT_USER" + Chrome360AppKey, "", null));

                    // 如果在注册表中找到360极速浏览器的注册信息，那么直接用360极速浏览器打开
                    if (!String.IsNullOrWhiteSpace(chrome360AppFileName))
                    {
                        #region 指定360浏览器或360极速浏览器打开链接的内核模式

                        // 判断是否指定内核打开地址
                        if (kernelType != BrowserKernelType.Node)
                        {
                            // 定义本地记录不同站点的内核模式
                            string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "switcher.txt");
                            // 如果文件不存在
                            if (!File.Exists(file))
                            {
                                File.Create(file).Close();
                                // 写入头部信息(固定写法)
                                using (StreamWriter sw = File.AppendText(file))
                                {
                                    sw.WriteLine("[360ee switcher]");
                                }
                                AddUrlKernelType(file, url, kernelType);
                            }
                            else
                            {
                                bool isExist = false;
                                using (StreamReader reader = new StreamReader(file))
                                {
                                    string line;
                                    while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
                                    {
                                        if (line.Contains(GetAddress(url)))
                                        {
                                            isExist = true;
                                            break;
                                        }
                                    }
                                }

                                // 判断已写入的内容是否存在
                                if (!isExist)
                                {
                                    // 在最后一行插入内容
                                    AddUrlKernelType(file, url, kernelType);
                                }
                            }

                            url = url + " --switcher-file=" + file;
                        }

                        #endregion 指定360浏览器或360极速浏览器打开链接的内核模式

                        Process.Start(chrome360AppFileName, url);
                        return;
                    }
                }
                // 如果未找到360极速浏览器则使用默认浏览器打开
                OpenDefaultBrowserUrl(url);
            }
            catch
            {
                // 如果发生异常则使用默认浏览器打开
                OpenDefaultBrowserUrl(url);
            }
        }

        /// <summary>
        /// 向指定文件中追加链接在360浏览器上打开的内核模式
        /// </summary>
        /// <param name="file">本地记录文件</param>
        /// <param name="url">Url地址</param>
        /// <param name="kernelType">浏览器内核模式</param>
        private static void AddUrlKernelType(string file, string url, BrowserKernelType kernelType)
        {
            using (StreamWriter sw = File.AppendText(file))
            {
                // 在最后一行插入内容
                switch (kernelType)
                {
                    case BrowserKernelType.Webkit:
                        sw.WriteLine(string.Format("{0}=Webkit", GetAddress(url)));
                        break;
                    case BrowserKernelType.IE:
                        sw.WriteLine(string.Format("{0}=IE", GetAddress(url)));
                        break;
                    case BrowserKernelType.IE7:
                        sw.WriteLine(string.Format("{0}=IE 7", GetAddress(url)));
                        break;
                    case BrowserKernelType.IE8:
                        sw.WriteLine(string.Format("{0}=IE 8", GetAddress(url)));
                        break;
                    case BrowserKernelType.IE9:
                        sw.WriteLine(string.Format("{0}=IE 9", GetAddress(url)));
                        break;
                    case BrowserKernelType.IE10:
                        sw.WriteLine(string.Format("{0}=IE 10", GetAddress(url)));
                        break;
                    case BrowserKernelType.IE11:
                        sw.WriteLine(string.Format("{0}=IE 11", GetAddress(url)));
                        break;
                }
            }
        }

        /// <summary>
        /// 获取请求链接的地址
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <returns>IP或域名</returns>
        private static string GetAddress(string url)
        {
            // 解析链接字符串
            Uri uri = new Uri(url);

            // 判断是否为IP请求
            if (IPAddress.TryParse(uri.Host, out IPAddress ipAddress))
            {
                // 链接是IP请求
                return ipAddress.ToString();
            }
            else
            {
                // 链接是域名请求
                return uri.Host;
            }
        }

        #endregion

        #region public static methods

        /// <summary>
        /// 通过浏览器打开Url
        /// </summary>
        /// <param name="url">Url地址</param>
        /// <param name="type">指定打开的浏览器类型</param>
        /// <param name="kernelType">浏览器内核模式(360浏览器或360极速浏览器)</param>
        public static void OpenBrowserUrl(string url, BrowserType type = BrowserType.Default, BrowserKernelType kernelType = BrowserKernelType.Node)
        {
            switch (type)
            {
                case BrowserType.Default:
                    OpenDefaultBrowserUrl(url);
                    break;
                case BrowserType.IE:
                    OpenIEBrowserUrl(url);
                    break;
                case BrowserType.Google:
                    OpenGoogleBrowserUrl(url);
                    break;
                case BrowserType.Firefox:
                    OpenFirefoxBrowserUrl(url);
                    break;
                case BrowserType.Edge:
                    OpenMSEdgeBrowserUrl(url);
                    break;
                case BrowserType.Chrome360:
                    OpenChrome360BrowserUrl(url, kernelType);
                    break;
                default:
                    OpenDefaultBrowserUrl(url);
                    break;
            }
        }

        /// <summary>
        /// 指定浏览器地址打开Url
        /// </summary>
        /// <param name="fileName">指定的浏览器地址</param>
        /// <param name="url">Url地址</param>
        public static void OpenBrowserUrl(string fileName, string url)
        {
            Process.Start(fileName, url);
        }

        #endregion
    }

    /// <summary>
    /// 浏览器内核模式(360浏览器或360极速浏览器)
    /// </summary>
    public enum BrowserKernelType
    {
        /// <summary>
        /// 默认模式(不指定内核模式)
        /// </summary>
        Node,
        /// <summary>
        /// 极速模式
        /// </summary>
        Webkit,
        /// <summary>
        /// IE(兼容IE)
        /// </summary>
        IE,
        /// <summary>
        /// IE7(兼容IE7) 
        /// </summary>
        IE7,
        /// <summary>
        /// IE8(兼容IE8)
        /// </summary>
        IE8,
        /// <summary>
        /// IE9(兼容IE9)
        /// </summary>
        IE9,
        /// <summary>
        /// IE10(兼容IE10)
        /// </summary>
        IE10,
        /// <summary>
        /// IE11(兼容IE11)
        /// </summary>
        IE11,
    }

    /// <summary>
    /// 浏览器类型
    /// </summary>
    public enum BrowserType
    {
        /// <summary>
        /// 默认浏览器
        /// </summary>
        Default = 0,
        /// <summary>
        /// IE浏览器
        /// </summary>
        IE = 1,
        /// <summary>
        /// Google浏览器
        /// </summary>
        Google = 2,
        /// <summary>
        /// 火狐
        /// </summary>
        Firefox = 3,
        /// <summary>
        /// Microsoft Edge
        /// </summary>
        Edge = 4,
        /// <summary>
        /// 360极速浏览器
        /// </summary>
        Chrome360 = 5,
    }

}
