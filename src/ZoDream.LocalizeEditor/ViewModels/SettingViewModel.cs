using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Translators;
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class SettingViewModel: BindableBase, IExitAttributable
    {
        public SettingViewModel()
        {
            BackCommand = new RelayCommand(TapBack);
            HomeCommand = new RelayCommand(TapHome);
            PluginCommand = new RelayCommand(TapPlugin);
            Load();
        }
        private bool IsSettingUpdated = true;

        private string version = App.ViewModel.Version;

        public string Version {
            get => version;
            set => Set(ref version, value);
        }

        private bool useBrowser;

        public bool UseBrowser {
            get => useBrowser;
            set {
                Set(ref useBrowser, value);
                IsSettingUpdated = true;
            }
        }



        private InputItem[] translatorItems = TranslatorFactory.NameItems;

        public InputItem[] TranslatorItems {
            get => translatorItems;
            set => Set(ref translatorItems, value);
        }


        private InputItem? translator;

        public InputItem Translator {
            get => translator!;
            set {
                Set(ref translator, value);
                IsSettingUpdated = true;
                UpdateForm(value.Name);
            }
        }


        private ObservableCollection<InputItem> inputItems = new();

        public ObservableCollection<InputItem> InputItems {
            get => inputItems;
            set => Set(ref inputItems, value);
        }



        public ICommand BackCommand { get; private set; }
        public ICommand HomeCommand { get; private set; }
       
        public ICommand PluginCommand { get; private set; }

        private void TapBack(object? _)
        {
            ShellManager.BackAsync();
        }

        private void TapHome(object? _)
        {
            ShellManager.GoToAsync("home");
        }

        private void TapPlugin(object? _)
        {
            ShellManager.GoToAsync("plugin");
        }


        private void Load()
        {
            var option = App.ViewModel.Option;
            foreach (var item in TranslatorItems)
            {
                if (option.Translator == item.Name)
                {
                    Translator = item;
                    break;
                }
            }
            UseBrowser = option.UseBrowser;
            UpdateForm(option.Translator, option.TranslatorData);
        }

        private void UpdateForm(string name, Dictionary<string, string>? data = null)
        {
            InputItems.Clear();
            var items = TranslatorFactory.RenderForm(name, data);
            foreach (var item in items)
            {
                InputItems.Add(item);
            }
        }

        public void ApplyExitAttributes()
        {
            if (IsSettingUpdated && Translator is not null)
            {
                var option = App.ViewModel.Option;
                option.UseBrowser = UseBrowser;
                option.Translator = Translator.Name;
                option.TranslatorData.Clear();
                foreach (var item in InputItems)
                {
                    option.TranslatorData.Add(item.Name, item.Value);
                }
                _ = App.ViewModel.SaveOptionAsync();
            }
        }
    }
}
