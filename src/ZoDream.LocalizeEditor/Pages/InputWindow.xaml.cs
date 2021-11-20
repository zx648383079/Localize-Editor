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
using System.Windows.Shapes;
using ZoDream.LocalizeEditor.Events;
using ZoDream.Shared.Models;

namespace ZoDream.LocalizeEditor.Pages
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow()
        {
            InitializeComponent();
        }

        private bool isOpen = false;

        public bool IsOpen
        {
            get { return isOpen; }
            set {
                isOpen = value;
                Height = value ? 250 : 100;
                MorePanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                MoreBtn.Content = value ? "\uE935" : "\uE936";
            }
        }

        public event ValueChangedEventHandler<UnitItem>? OnCreate;

        private void MoreBtn_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = !IsOpen;
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            OnCreate?.Invoke(this, new UnitItem(SourceTb.Text, TargetTb.Text, FileTb.FileName, LineTb.Text));
        }
    }
}
