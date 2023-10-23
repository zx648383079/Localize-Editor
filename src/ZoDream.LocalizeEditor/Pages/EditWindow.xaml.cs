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
    /// EditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditWindow : Window
    {
        public EditWindow()
        {
            InitializeComponent();
        }

        public EditViewModel ViewModel => (EditViewModel)DataContext;

        public event EmptyEventHandler? OnConfirm;
        public event EmptyEventHandler? OnPrevious;
        public event EmptyEventHandler? OnNext;

        public UnitViewModel Data
        {
            get
            {
                return new UnitViewModel()
                {
                    Source = ViewModel.Source,
                    Target = ViewModel.Target,
                    Id = ViewModel.Id,
                    Location = ViewModel.LocationItems.ToList()
                };
            }
            set
            {
                ViewModel.Source = value.Source;
                ViewModel.Target = value.Target;
                ViewModel.Id = value.Id;
                ViewModel.LocationItems.Clear();
                foreach (var item in value.Location)
                {
                    ViewModel.LocationItems.Add(item);
                }
                ViewModel.SaveEnabled = false;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            OnConfirm?.Invoke();
            ViewModel.SaveEnabled = false;
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            OnPrevious?.Invoke();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            OnNext?.Invoke();
        }
    }
}
