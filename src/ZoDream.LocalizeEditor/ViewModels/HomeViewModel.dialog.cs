using System;
using System.Windows;
using System.Windows.Input;
using ZoDream.LocalizeEditor.Pages;
using ZoDream.Shared.Routes;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class HomeViewModel
    {
        private Action<string>? DialogCallbackFn;

        private bool dialogVisible;

        public bool DialogVisible {
            get => dialogVisible;
            set {
                if (value && LangItems.Length == 0)
                {
                    LangItems = App.ViewModel.LangDictionary.ToStringArray();
                }
                Set(ref dialogVisible, value);
                if (value)
                {
                    DialogTargetLang = TargetLang;
                }
            }
        }

        private string dialogTargetLang = string.Empty;

        public string DialogTargetLang {
            get => dialogTargetLang;
            set => Set(ref dialogTargetLang, value);
        }

        public ICommand DialogConfirmCommand { get; private set; }
        public ICommand DialogCancelCommand { get; private set; }


        public void DialogOpen(Action<string>? callback = null)
        {
            DialogVisible = true;
            DialogCallbackFn = callback;
        }

        private void TapDialogCancel(object? _)
        {
            DialogVisible = false;
            DialogCallbackFn = null;
        }

        private void TapDialogConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(DialogTargetLang))
            {
                return;
            }
            DialogVisible = false;
            if (DialogCallbackFn is not null)
            {
                DialogCallbackFn?.Invoke(DialogTargetLang);
                return;
            }
            TargetLang = DialogTargetLang;
            Load(App.ViewModel.LangDictionary.RepairCode(DialogTargetLang));
        }

    }
}
