using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDNExporter.Config
{
    internal class CSDNExportConfig : BaseConfig<CSDNExportConfigModel>
    {
        public override string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Config/CSDNExporterConfig.cfg");
        private CSDNExportConfig() { }
        public static CSDNExportConfig Instance { get; } = new CSDNExportConfig();
    }
    public class CSDNExportConfigModel
    {
        public string Cookie { get; set; }
        public string UserName { get; set; }
        public bool IsDownloadImage { get; set; }
    }
}
