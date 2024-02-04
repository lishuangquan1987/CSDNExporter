using CSDNExporter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Config
{
    /// <summary>
    /// 对配置保存与加载的统一封装
    /// 继承类可以写成单例模式，在构造时加载配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseConfig<T> where T : class, new()
    {
        public T Config { get; set; } = new T();
        public BaseConfig()
        {
            LoadConfig();
        }
        /// <summary>
        /// 需要子类重写配置保存在哪里
        /// </summary>
        public abstract string ConfigPath { get; }
        /// <summary>
        /// 子类在静态构造函数中构造单例
        /// </summary>

        protected virtual void LoadConfig()
        {
            if (!File.Exists(ConfigPath))
            {
                return;
            }
            try
            {
                var str=File.ReadAllText(ConfigPath);
                Config = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            }
            catch (Exception e)
            {

            }
        }
        public virtual void SaveConfig()
        {
            var dir = Path.GetDirectoryName(ConfigPath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var str= Newtonsoft.Json.JsonConvert.SerializeObject(Config);
            File.WriteAllText(ConfigPath, str);
        }
    }
}
