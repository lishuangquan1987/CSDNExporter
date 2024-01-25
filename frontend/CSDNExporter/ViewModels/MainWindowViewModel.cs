using CommunityToolkit.Mvvm.Input;
using ControlzEx.Standard;
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
            Articles.CollectionChanged += (s, e) => this.ExportCmd?.NotifyCanExecuteChanged();
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
            try
            {
                Articles.Clear();
                this.TotalArticles = 0;
                IsBusy = true;
                var result = await CSDNHelper.GetArticleInfos(this.CSDNUserName);
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
                await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", e.Message, MessageDialogStyle.Affirmative);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void Export()
        {
            try
            {
                var articlesToExport = this.Articles.Where(x => x.IsSelected).ToList();
                if (!articlesToExport.Any())
                {
                    await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", "Please select at least one article to export", MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary);
                    return;
                }

                OpenFolderDialog openFolderDialog = new OpenFolderDialog();
                openFolderDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (openFolderDialog.ShowDialog() != true) return;

                IsBusy = true;
                foreach (var article in articlesToExport)
                {
                    var result = await ArticleServiceHelper.GetMarkDownStr(article.Url);
                    if (!result.IsSuccess)
                    {
                        await (App.Current.MainWindow as MainWindow).ShowMessageAsync("Error", result.ErrorMsg, MessageDialogStyle.Affirmative);
                        return;
                    }

                    string path = Path.Combine(openFolderDialog.FolderName, $"{ResolvePath(article.Title)}.md");
                    File.WriteAllText(path, result.Result);
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
            }
        }

        private string ResolvePath(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var c in str)
            {
                if (Path.InvalidPathChars.Contains(c))
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
    }
}
