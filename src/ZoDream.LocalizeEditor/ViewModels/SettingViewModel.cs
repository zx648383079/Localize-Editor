using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ZoDream.Shared.Models;
using ZoDream.Shared.Routes;
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


        private InputItem[] translatorItems = new InputItem[]{
            new("DeepL", "DeepL翻译"),
            new("Baidu", "百度翻译"),
            new("Tencent", "腾讯翻译"),
            new("YouDao", "有道翻译"),
            new("Azure", "必应翻译"),
            new("Google", "谷歌翻译"),
        };

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
            UpdateForm(option.Translator, option.TranslatorData);
        }

        private void UpdateForm(string name, Dictionary<string, string>? data = null)
        {
            InputItems.Clear();
            switch (name)
            {
                case "DeepL":
                    InputItems.Add(new("AuthKey", data));
                    break;
                case "Baidu":
                    InputItems.Add(new("AppId", data));
                    InputItems.Add(new("Secret", data));
                    break;
                case "Tencent":
                    InputItems.Add(new("SecretId", data));
                    InputItems.Add(new("SecretKey", data));
                    InputItems.Add(new("Region", data));
                    InputItems.Add(new("ProjectId", data));
                    break;
                case "YouDao":
                    InputItems.Add(new("AppKey", data));
                    InputItems.Add(new("Secret", data));
                    break;
                case "Azure":
                    InputItems.Add(new("AuthKey", data));
                    InputItems.Add(new("Region", data));
                    break;
                case "Google":
                    InputItems.Add(new("Token", data));
                    InputItems.Add(new("Project", data));
                    break;
                default:
                    break;
            }
        }

        public void ApplyExitAttributes()
        {
            if (IsSettingUpdated && Translator is not null)
            {
                var option = App.ViewModel.Option;
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
