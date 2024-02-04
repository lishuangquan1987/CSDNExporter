using CommunityToolkit.Mvvm.Input;
using CSDNExporter.Config;
using CSDNExporter.Models;
using CSDNExporter.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSDNExporter.ViewModels
{
    internal class CookieSettingViewModel : INotifyPropertyChanged
    {
        private CookieSettingView _view;
        public CookieSettingViewModel(CookieSettingView view)
        {
            this.Cookie=CSDNExportConfig.Instance.Config.Cookie;
            this.TestUrlClickCmd = new RelayCommand(TestUrl);
            this.SaveCmd = new RelayCommand(Save);
            this._view = view;
        }

        private void Save()
        {
            CSDNExportConfig.Instance.Config.Cookie=this.Cookie;
            CSDNExportConfig.Instance.SaveConfig();
            this._view.DialogResult = true;
        }

        private void TestUrl()
        {
            try
            {
                string url = "blog.csdn.net/community/home-api/v1/get-business-list?page=1&size=1&businessType=blog&orderby=&noMore=false&year=&month=&username=lishuangquan1987";
                BrowserHelper.OpenBrowserUrl(url, BrowserType.Edge);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand TestUrlClickCmd { get; set; }
        public RelayCommand SaveCmd { get; set; }
        public string Cookie { get; set; }
    }
}
