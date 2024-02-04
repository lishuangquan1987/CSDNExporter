using CommunityToolkit.Mvvm.Input;
using ControlzEx.Standard;
using CSDNExporter.BlogResolver;
using CSDNExporter.Config;
using CSDNExporter.Models;
using CSDNExporter.Views;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CSDNExporter.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        
        public MainWindowViewModel()
        {
            ExportCmd = new RelayCommand(Export, () => this.Articles.Count > 0);
            GetArticlesCmd = new RelayCommand(GetArticle, () => !string.IsNullOrEmpty(this.CSDNUserName));
            CookieSettingCmd = new RelayCommand(CookieSetting);
            Articles.CollectionChanged += (s, e) => this.ExportCmd?.NotifyCanExecuteChanged();
            this.IsDownloadImage = CSDNExportConfig.Instance.Config.IsDownloadImage;
            this.CSDNUserName = CSDNExportConfig.Instance.Config.UserName;
        }

        private void CookieSetting()
        {
            CookieSettingView v = new CookieSettingView();
            v.ShowDialog();
        }

        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            //Perform property validation
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

            if (propertyName == nameof(IsSelectAll))
            {
                if (IsSelectAll)
                {
                    foreach (var article in Articles)
                    {
                        article.IsSelected = true;
                    }
                }
                else
                {
                    foreach (var article in Articles)
                    {
                        article.IsSelected = false;
                    }
                }
            }
            else if (propertyName == nameof(CSDNUserName))
            {
                this.GetArticlesCmd?.NotifyCanExecuteChanged();
            }
        }

        private async void GetArticle()
        {
            if (string.IsNullOrEmpty(CSDNExportConfig.Instance.Config.Cookie))
            {
                await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", "请先设置Cookie", MessageDialogStyle.Affirmative);
                return;
            }
            //保存配置
            CSDNExportConfig.Instance.Config.IsDownloadImage = this.IsDownloadImage;
            CSDNExportConfig.Instance.Config.UserName = this.CSDNUserName;
            CSDNExportConfig.Instance.SaveConfig();
            try
            {
                Articles.Clear();
                this.TotalArticles = 0;
                IsBusy = true;
                IsPercentVisible = false;
                var result = await CSDNHelper.GetArticleInfos(this.CSDNUserName,CSDNExportConfig.Instance.Config.Cookie);
                if (!result.IsSuccess)
                {
                    await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", result.ErrorMsg, MessageDialogStyle.Affirmative);
                    return;
                }

                result.Data?.ForEach(x => this.Articles.Add(x));
                this.TotalArticles = result.Data!.Count;
            }
            catch (Exception e)
            {
                await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", e.Message+"\n有可能需要重新设置Cookie", MessageDialogStyle.Affirmative);
            }
            finally
            {
                IsBusy = false;
                IsPercentVisible = false;
            }
        }

        private async void Export()
        {
            try
            {
                var articlesToExport = this.Articles.Where(x => x.IsSelected).ToList();
                if (!articlesToExport.Any())
                {
                    await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", "Please select at least one article to export", MessageDialogStyle.Affirmative);
                    return;
                }

                OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                if (openFolderDialog.ShowDialog() != true) return;

                IsBusy = true;
                IsPercentVisible = true;
                ExportPercent = 0;
                List<IBlogResolver> resolvers = new List<IBlogResolver>();
                resolvers.Add(new ContentResolver());
                if (IsDownloadImage)
                {
                    resolvers.Add(new ImageResolver());
                }
                for (int i = 0; i < articlesToExport.Count; i++)
                {
                    var article = articlesToExport[i];
                    var result = await ArticleServiceHelper.GetMarkDownStr(article.Url);
                    if (!result.IsSuccess)
                    {
                        await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", result.ErrorMsg, MessageDialogStyle.Affirmative);
                        return;
                    }

                    string path = Path.Combine(openFolderDialog.FolderName, $"{ResolvePath(article.Title)}.md");

                    var contents = result.Result.Split('\n');
                    foreach (var resolver in resolvers)
                    {
                        contents =await resolver.Resolve(contents, new Dictionary<string, object>() { { "Path", path } });
                    }

                    await File.WriteAllLinesAsync(path, contents);

                    ExportPercent = Math.Round((i + 1) / (double)articlesToExport.Count * 100, 2);
                }
                await (App.Current.MainWindow as MainWindow).ShowMessageAsync("OK", "导出成功", MessageDialogStyle.Affirmative);
            }
            catch (Exception e)
            {
                await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", e.Message, MessageDialogStyle.Affirmative);
            }
            finally
            {
                IsBusy = false;
                IsPercentVisible = false;
            }
        }

        private string ResolvePath(string str)
        {
            StringBuilder sb = new StringBuilder();
            var invalidPathChars = Path.GetInvalidFileNameChars();
            foreach (var c in str)
            {
                if (invalidPathChars.Contains(c))
                {
                    sb.Append("_");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public RelayCommand ExportCmd { get; set; }
        public RelayCommand GetArticlesCmd { get; set; }

        public RelayCommand CookieSettingCmd { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<ArticleInfo> articles = new ObservableCollection<ArticleInfo>();
        public ObservableCollection<ArticleInfo> Articles
        {
            get { return articles; }
            set { articles = value; }
        }

        public int TotalArticles { get; set; }

        public string CSDNUserName { get; set; } = "lishuangquan1987";
        public bool IsBusy { get; set; }
        public bool IsSelectAll { get; set; }
        /// <summary>
        /// 导出进度百分比
        /// </summary>
        public double ExportPercent { get; set; }
        public bool IsDownloadImage { get; set; }
        public bool IsPercentVisible { get; set; }
    }
}
