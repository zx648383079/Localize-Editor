using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ZoDream.Shared.Routes;
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class StartupViewModel: BindableBase
    {
        public StartupViewModel()
        {
            OpenCommand = new RelayCommand(TapOpen);
            CreateCommand = new RelayCommand(TapCreate);
            Version = App.ViewModel.Version;
        }

        private string version = string.Empty;

        public string Version {
            get => version;
            set => Set(ref version, value);
        }

        private string tip = string.Empty;

        public string Tip {
            get => tip;
            set => Set(ref tip, value);
        }

        public ICommand OpenCommand { get; private set; }

        public ICommand CreateCommand { get; private set; }


        private async void TapOpen(object? _)
        {
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = AppViewModel.FileFilters,
                Title = "选择文件"
            };
            if (open.ShowDialog() != true)
            {
                return;
            }
            await App.ViewModel.OpenPackageAsync(open.FileName);
            ShellManager.GoToAsync("home");
        }

        private async void TapCreate(object? _)
        {
            await App.ViewModel.CreatePackageAsync();
            ShellManager.GoToAsync("home");
        }
    }
}
