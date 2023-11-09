using System;
using System.Windows.Input;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class EditViewModel
    {

        private Action<string>? DialogCallbackFn;

        private bool dialogVisible;

        public bool DialogVisible {
            get => dialogVisible;
            set {
                Set(ref dialogVisible, value);
                if (value)
                {
                    DialogContent = UpdatedData.Comment;
                }
            }
        }



        private string dialogContent = string.Empty;

        public string DialogContent {
            get => dialogContent;
            set => Set(ref dialogContent, value);
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
            DialogFileVisible = DialogVisible = false;
            DialogCallbackFn = null;
        }

        private void TapDialogConfirm(object? _)
        {
            DialogVisible = false;
            if (DialogCallbackFn is not null)
            {
                DialogCallbackFn?.Invoke(DialogContent);
                return;
            }
        }

    }
}
