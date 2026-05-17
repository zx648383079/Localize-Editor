using System.Collections.ObjectModel;
using System.Windows.Input;

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
                _app.Packages.Remove(
                    _app.LangDictionary.RepairCode(package.TargetLanguage));
                PanelItems.Remove(package);
            }
        }

        private void TapPanelOpen(object? arg)
        {
            if (arg is LangePackageViewModel package)
            {
                var lange = _app.LangDictionary.RepairCode(package.TargetLanguage);
                TargetLang = package.TargetLanguage;
                Load(lange);
                PanelVisible = false;
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
                Load(_app.LangDictionary.RepairCode(lang));
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
            foreach (var item in _app.Packages)
            {
                PanelItems.Add(new LangePackageViewModel(
                  _app.LangDictionary.CodeToString(item.Value.Language),
                  _app.LangDictionary.CodeToString(item.Value.TargetLanguage)
                    ));
            }
        }

    }
}
