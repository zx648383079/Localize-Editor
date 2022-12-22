using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.LocalizeEditor.Pages;
using ZoDream.LocalizeEditor.ViewModels;
using ZoDream.Shared.Models;

namespace ZoDream.LocalizeEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public MainViewModel ViewModel = new();

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            QuickAdd();
        }

        private void QuickAdd()
        {
            var page = new InputWindow();
            page.OnCreate += (sender, e) =>
            {
                var merge = false;
                if (ViewModel.IndexOf(e) >= 0 &&
                MessageBox.Show("存在相同待翻译内容，是否合并?") == MessageBoxResult.OK)
                {
                    merge = true;
                }
                ViewModel.Add(e, merge);
            };
            page.Show();
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            var menu = sender as MenuItem;
            if (menu != null)
            {
                Save(menu.Header.ToString());
            }
        }

        private void UnitListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditUnit(UnitListBox.SelectedIndex);
        }

        private void EditUnit(int i)
        {
            if (i < 0)
            {
                return;
            }
            var page = new EditWindow();
            page.Data = ViewModel.Items[i];
            page.OnConfirm += () =>
            {
                var res = page.Data;
                ViewModel.Items[i].Id = res.Id;
                ViewModel.Items[i].Source = res.Source;
                ViewModel.Items[i].Target = res.Target;
                ViewModel.Items[i].Location = res.Location;
            };
            page.OnPrevious += () =>
            {
                if (i < 1)
                {
                    return;
                }
                i--;
                page.Data = ViewModel.Items[i];
                UnitListBox.SelectedIndex = i;
            };
            page.OnNext += () =>
            {
                if (i >= ViewModel.Items.Count - 1)
                {
                    return;
                }
                i++;
                page.Data = ViewModel.Items[i];
                UnitListBox.SelectedIndex = i;
            };
            page.Show();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuItem).Header)
            {
                case "退出":
                    Close();
                    break;
                case "设置":
                    new SettingWindow().ShowDialog();
                    break;
                case "新建":
                    ViewModel.Load();
                    break;
                case "打开":
                    ViewModel.Load();
                    Open();
                    break;
                case "保存":
                    Save();
                    break;
                case "快速添加":
                    QuickAdd();
                    break;
                default:
                    break;
            }
        }

        private void Open()
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "XLFF文件|*.xlf|JSON文件|*.json|RESW文件|*.resw|所有文件|*.*",
                Title = "选择文件"
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            var fillEmpty = ViewModel.Items.Count > 0 && MessageBox.Show("是否需要再根据ID更新之后自动补充空白内容？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            _ = ViewModel.LoadAsync(picker.FileName, fillEmpty);
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(ViewModel.SourceFile))
            {
                Save("xlf");
                return;
            }
            _ = ViewModel.SaveAsync(ViewModel.SourceFile);
        }

        private void Save(string ext)
        {
            var picker = new Microsoft.Win32.SaveFileDialog
            {
                Title = "选择保存路径",
                Filter = "XLFF文件|*.xlf|JSON文件|*.json|RESW文件|*.resw|所有文件|*.*",
                FileName = "undefine." + ext,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            _ = ViewModel.SaveAsync(picker.FileName);
        }
    }
}
