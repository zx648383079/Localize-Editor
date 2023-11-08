using System;
using System.Windows;
using System.Windows.Input;
using ZoDream.LocalizeEditor.Pages;
using ZoDream.Shared.Routes;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class HomeViewModel
    {
        public ICommand AddCommand { get; private set; }
        public ICommand EditCommand { get; private set; }

        public ICommand RemoveCommand { get; private set; }
        public ICommand OpenCommand { get; private set; }
        public ICommand NewCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }
        public ICommand ImportCommand { get; private set; }
        public ICommand ExportCommand { get; private set; }
        public ICommand SettingCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public ICommand ChangeCommand { get; private set; }

        public ICommand SearchCommand {  get; private set; }

        public ICommand OpenBrowserCommand {  get; private set; }

        public ICommand ImportDatabaseCommand {  get; private set; }
        public ICommand ExportDatabaseCommand {  get; private set; }


        private void TapOpenBrowser(object? _)
        {
            App.ViewModel.OpenBrowser();
        }

        private void TapSearch(object? _)
        {
            FilteredItems.Refresh();
        }

        private void TapChange(object? _)
        {
            DialogOpen();
        }

        private void TapExit(object? _)
        {
            App.ViewModel.Exit();
        }

        private void TapAdd(object? _)
        {
            var page = new InputWindow();
            page.OnCreate += (sender, e) => {
                var merge = false;
                if (IndexOf(e) >= 0 &&
                MessageBox.Show("存在相同待翻译内容，是否合并?") == MessageBoxResult.OK)
                {
                    merge = true;
                }
                Add(e, merge);
            };
            page.Show();
        }

        private void TapEdit(object? arg)
        {
            var item = UnitSelectedItem;
            if (item is null)
            {
                return;
            }
            var i = Items.IndexOf(item);
            var page = new EditWindow();
            var model = page.ViewModel;
            SyncEdit(model, i);
            model.OnConfirm += () => {
                var res = model.Data;
                Items[i].Id = res.Id;
                Items[i].Source = res.Source;
                Items[i].Target = res.Target;
                Items[i].Location = res.Location;
            };
            model.OnPrevious += () => {
                if (i < 1)
                {
                    return;
                }
                SyncEdit(model, --i);
            };
            model.OnNext += () => {
                if (i >= Items.Count - 1)
                {
                    return;
                }
                SyncEdit(model, ++i);
            };
            page.Show();
        }

        private void SyncEdit(EditViewModel model, int i)
        {
            if (UnitSelectedItem != Items[i])
            {
                UnitSelectedItem = Items[i];
            }
            model.Data = Items[i];
            model.PreviousEnabled = i > 0;
            model.NextEnabled = i < Items.Count - 1;
        }

        private void TapRemove(object? _)
        {
            var item = UnitSelectedItem;
            if (item is null)
            {
                return;
            }
            Items.Remove(item);
        }

        private void TapNew(object? _)
        {
            TapChange(_);
        }

        private void TapOpen(object? _)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Filter = AppViewModel.FileFilters,
                Title = "选择文件"
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            var fillEmpty = Items.Count > 0 && MessageBox.Show("是否需要再根据ID更新之后自动补充空白内容？", "提示", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            _ = LoadAsync(picker.FileName, fillEmpty);
        }

        private void TapSave(object? _)
        {
            var package = App.ViewModel.CurrentPackage;
            if (package is null || string.IsNullOrWhiteSpace(package.FileName))
            {
                TapSaveAs("xlf");
                return;
            }
            _ = SaveAsync(package.FileName);
        }

        private void TapSaveAs(object? arg)
        {
            var ext = arg?.ToString() ?? "xlf";
            var picker = new Microsoft.Win32.SaveFileDialog
            {
                Title = "选择保存路径",
                Filter = AppViewModel.FileFilters,
                FileName = "undefine." + ext,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            _ = SaveAsync(picker.FileName);
        }
        private void TapImport(object? _)
        {
            TapOpen(_);
        }
        private void TapExport(object? arg)
        {
            TapSaveAs(arg);
        }

        private void TapExportDatabase(object? _)
        {
            DatabaseDialogOpen((s, reader) => {
                _ = SaveAsync(reader, s);
            });
        }

        private void TapImportDatabase(object? _)
        {
            DatabaseDialogOpen((s, reader) => {
                _ = LoadAsync(reader, s);
            });
        }

        private void TapSetting(object? _)
        {
            ShellManager.GoToAsync("setting");
        }
    }
}
