using System;
using System.Windows.Input;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.Models;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class EditViewModel
    {

        private SourceLocation? locationSelectedItem;

        public SourceLocation? LocationSelectedItem {
            get => locationSelectedItem;
            set => Set(ref locationSelectedItem, value);
        }

        private bool dialogFileVisible;

        public bool DialogFileVisible {
            get => dialogFileVisible;
            set => Set(ref dialogFileVisible, value);
        }

        private string dialogFileName = string.Empty;

        public string DialogFileName {
            get => dialogFileName;
            set => Set(ref dialogFileName, value);
        }

        private string dialogFileLine = string.Empty;

        public string DialogFileLine {
            get => dialogFileLine;
            set => Set(ref dialogFileLine, value);
        }


        public ICommand DialogFileConfirmCommand {  get; private set; }
        public ICommand AddFileCommand {  get; private set; }
        public ICommand EditFileCommand {  get; private set; }
        public ICommand RemoveFileCommand {  get; private set; }


        private void TapDialogFileConfirm(object? _)
        {
            if (string.IsNullOrWhiteSpace(DialogFileName) || 
                string.IsNullOrWhiteSpace(DialogFileLine))
            {
                return;
            }
            DialogFileVisible = false;
            if (LocationSelectedItem is null)
            {
                LocationItems.AddLine(DialogFileName, DialogFileLine);
                return;
            }
            var i = LocationItems.IndexOf(LocationSelectedItem);
            if (i < -1)
            {
                return;
            }
            LocationItems[i] = new SourceLocation(DialogFileName, DialogFileLine);
        }

        private void TapAddFile(object? _)
        {
            DialogFileVisible = true;
            DialogFileName = string.Empty;
            DialogFileLine = string.Empty;
            LocationSelectedItem = null;
        }
        private void TapEditFile(object? _)
        {
            if (LocationSelectedItem is null)
            {
                return;
            }
            DialogFileName = LocationSelectedItem.FileName;
            DialogFileLine = LocationSelectedItem.LineNumberFormat;
            DialogFileVisible = true;
        }
        private void TapRemoveFile(object? _)
        {
            if (LocationSelectedItem is null)
            {
                return;
            }
            LocationItems.Remove(LocationSelectedItem);
        }
    }
}
