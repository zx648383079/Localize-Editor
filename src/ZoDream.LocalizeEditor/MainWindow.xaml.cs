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

        private void LangTb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
