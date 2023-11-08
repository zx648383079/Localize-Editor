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
using ZoDream.LocalizeEditor.ViewModels;
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
            Height = ViewModel.WindowHeight;
            ViewModel.PropertyChanged += (_, e) => {
                if (e.PropertyName == nameof(ViewModel.WindowHeight))
                {
                    Height = ViewModel.WindowHeight;
                }
            };
            ViewModel.AddListener(() => {
                OnCreate?.Invoke(this, ViewModel.Data);
            });
        }

        public QuicklyAddViewModel ViewModel => (QuicklyAddViewModel)DataContext;

        public event ValueChangedEventHandler<UnitItem>? OnCreate;

    }
}
