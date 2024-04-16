using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Windows.Input;
using ZoDream.Shared.Models;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class HomeViewModel
    {
        private bool panelVisible;

        public bool PanelVisible {
            get => panelVisible;
            set => Set(ref panelVisible, value);
        }

        private ObservableCollection<LangePackageViewModel> panelItems = [];

        public ObservableCollection<LangePackageViewModel> PanelItems {
            get => panelItems;
            set => Set(ref panelItems, value);
        }

        public ICommand PanelCloseCommand { get; private set; }
        public ICommand PanelAddCommand { get; private set; }
        public ICommand PanelOpenCommand { get; private set; }
        public ICommand PanelRemoveCommand { get; private set; }

        private void TapPanelRemove(object? arg)
        {
            if (arg is LangePackageViewModel package)
            {
                App.ViewModel.Packages.Remove(
                    App.ViewModel.LangDictionary.RepairCode(package.TargetLanguage));
                PanelItems.Remove(package);
            }
        }

        private void TapPanelOpen(object? arg)
        {
            if (arg is LangePackageViewModel package)
            {
                var lange = App.ViewModel.LangDictionary.RepairCode(package.TargetLanguage);
                TargetLang = package.TargetLanguage;
                Load(lange);
            }
        }

        private void TapPanelClose(object? _)
        {
            PanelVisible = false;
        }

        private void TapPanelAdd(object? _)
        {
            DialogOpen(lang => {
                TargetLang = lang;
                Load(App.ViewModel.LangDictionary.RepairCode(lang));
                PanelRefresh();
            });
        }

        private void PanelOpen()
        {
            PanelRefresh();
            PanelVisible = true;
        }

        private void PanelRefresh()
        {
            PanelItems.Clear();
            foreach (var item in App.ViewModel.Packages)
            {
                PanelItems.Add(new LangePackageViewModel(
                  App.ViewModel.LangDictionary.CodeToString(item.Value.Language),
                  App.ViewModel.LangDictionary.CodeToString(item.Value.TargetLanguage)
                    ));
            }
        }

    }
}
