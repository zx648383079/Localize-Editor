using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
            DialogConfirmCommand = new RelayCommand(TapDialogConfirm);
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


        private bool dialogVisible;

        public bool DialogVisible {
            get => dialogVisible;
            set {
                if (value && LangItems.Length == 0)
                {
                    LangItems = App.ViewModel.LangDictionary.ToStringArray();
                }
                Set(ref dialogVisible, value);
            }
        }


        private string sourceLang = string.Empty;

        public string SourceLang {
            get => sourceLang;
            set => Set(ref sourceLang, value);
        }

        private string targetLang = string.Empty;

        public string TargetLang {
            get => targetLang;
            set => Set(ref targetLang, value);
        }

        private string[] langItems = Array.Empty<string>();

        public string[] LangItems {
            get => langItems;
            set => Set(ref langItems, value);
        }

        public ICommand OpenCommand { get; private set; }

        public ICommand CreateCommand { get; private set; }

        public ICommand DialogConfirmCommand { get; private set; }


        private async void TapDialogConfirm(object? _)
        {
            var sLang = App.ViewModel.LangDictionary.RepairCode(SourceLang);
            var tLang = App.ViewModel.LangDictionary.RepairCode(TargetLang);
            if (string.IsNullOrEmpty(sLang) || string.IsNullOrEmpty(tLang)) 
            {
                MessageBox.Show("请选择语言");
                return;
            }
            var package = App.ViewModel.CurrentPackage;
            if (package is null)
            {
                await App.ViewModel.CreatePackageAsync(sLang, tLang);
            } else
            {
                App.ViewModel.Packages.Clear();
                package.Language = sLang;
                package.TargetLanguage = tLang;
                App.ViewModel.CurrentPackage = package;
            }
            DialogVisible = false;
            ShellManager.GoToAsync("home");
        }

        private async void TapOpen(object? _)
        {
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Filter = AppViewModel.FileFilters,
                Title = "选择文件"
            };
            if (open.ShowDialog() != true)
            {
                return;
            }
            var res = await App.ViewModel.OpenPackageAsync(open.FileName);
            if (!res)
            {
                MessageBox.Show("文件不支持");
                return;
            }
            if (string.IsNullOrWhiteSpace(App.ViewModel.PackageLanguage))
            {
                SourceLang = App.ViewModel.LangDictionary.CodeToString(App.ViewModel.CurrentPackage!.Language);
                if (string.IsNullOrEmpty(SourceLang))
                {
                    SourceLang = App.ViewModel.LangDictionary.CodeToString("en");
                }
                DialogVisible = true;
                return;
            }
            ShellManager.GoToAsync("home");
        }

        private void TapCreate(object? _)
        {
            SourceLang = App.ViewModel.LangDictionary.CodeToString("en");
            DialogVisible = true;
        }
    }
}
