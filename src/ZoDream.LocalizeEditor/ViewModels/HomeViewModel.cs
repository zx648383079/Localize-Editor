using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Shared;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class HomeViewModel : BindableBase
    {

        public HomeViewModel()
        {
            AddCommand = new RelayCommand(TapAdd);
            EditCommand = new RelayCommand(TapEdit);
            RemoveCommand = new RelayCommand(TapRemove);
            OpenCommand = new RelayCommand(TapOpen);
            NewCommand = new RelayCommand(TapNew);
            SaveAsCommand = new RelayCommand(TapSaveAs);
            SaveCommand = new RelayCommand(TapSave);
            SettingCommand = new RelayCommand(TapSetting);
            ImportCommand = new RelayCommand(TapImport);
            ExportCommand = new RelayCommand(TapExport);
            ExitCommand = new RelayCommand(TapExit);
            if (!AppViewModel.IsDesignMode)
            {
                LoadAsync();
            }
            LoadAsync(App.ViewModel.CurrentPackage, false);
        }


        public async void LoadAsync()
        {
            LangItems = await LanguageFile.LoadAsync("languages.txt");
        }

        public int IndexOf(UnitItem e)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (e.Source == items[i].Source && 
                    (string.IsNullOrWhiteSpace(e.Target) || e.Target == items[i].Target) && 
                    (string.IsNullOrWhiteSpace(e.Id) || e.Id == items[i].Id))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Add(UnitItem e)
        {
            Add(e, false);
        }

        public void Add(UnitItem e, bool merge, bool mergeLocation = true)
        {
            var i = IndexOf(e);
            if (i < 0 || !merge)
            {
                Items.Add(new UnitViewModel(e));
                return;
            }
            var target = items[i];
            if (string.IsNullOrWhiteSpace(e.Target))
            {
                target.Target = e.Target;
            }
            if (!string.IsNullOrWhiteSpace(e.Target) || mergeLocation)
            {
                target.AddLine(e.Location);
            }
        }

        
        public void ChangeLanguage(string lang)
        {
            var l = LanguageFile.Format(lang);
            if (l == CurrentLanguage)
            {
                return;
            }
            targetLang = l == null ? "" : l.Code;
            if (CurrentLanguage == null)
            {
                CurrentLanguage = l;
                return;
            }
            Save(CurrentLanguage, Items);
            Load(l);
        }


        public void Merge(IEnumerable<UnitItem> items, bool mergeLocation = true, bool fillEmpty = false)
        {
            foreach (var item in items)
            {
                if (mergeLocation)
                {
                    Add(item, true, mergeLocation);
                    continue;
                }
                if (string.IsNullOrWhiteSpace(item.Target))
                {
                    continue;
                }
                foreach (var it in Items)
                {
                    if (string.IsNullOrWhiteSpace(it.Target) 
                        && item.Source.Trim() == it.Source.Trim() 
                        && ShouldMerge(it, item, true, false))
                    {
                        it.Target = item.Target;
                    }
                }
            }
            if (!fillEmpty)
            {
                return;
            }
            foreach (var it in Items)
            {
                if (!string.IsNullOrWhiteSpace(it.Target))
                {
                    continue;
                }
                
                var same = GetUnit(items, it, true, true);
                if (same is not null)
                {
                    it.Target = same.Target;
                }
            }
        }

        private UnitItem? GetUnit(IEnumerable<UnitItem> items, UnitViewModel source, 
            bool notEmpty, bool checkSameFile)
        {
            foreach(var item in items)
            {
                if (item.Source.Trim() != source.Source.Trim())
                {
                    continue;
                }
                if (notEmpty && string.IsNullOrWhiteSpace(item.Target))
                {
                    continue;
                }
                if (checkSameFile && !HasSameFile(source, item))
                {
                    continue;
                }
                return item;
            }
            return null;
        }

        private bool HasSameFile(UnitViewModel dist, UnitItem source)
        {
            if (dist.Location.Count == 0 || source.Location.Count == 0)
            {
                return true;
            }
            foreach (var item in dist.Location)
            {
                foreach (var it in source.Location)
                {
                    if (item.FileName == it.FileName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool ShouldMerge(UnitViewModel dist, UnitItem source, bool byId = true, bool byPath = false)
        {
            if (byId)
            {
                return string.IsNullOrWhiteSpace(source.Id) ||
                        string.IsNullOrWhiteSpace(dist.Id) || source.Id == dist.Id;
            }
            if (byPath)
            {
                return HasSameFile(dist, source);
            }
            return true;
        }

        public void Load(LangItem? lang)
        {
            CurrentLanguage = lang;
            App.ViewModel.PackageLanguage = lang?.Code ?? "new";
            if (lang is null || !App.ViewModel.Packages.TryGetValue(lang.Code, out var package))
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i] = Items[i].Clone();
                }
                return;
            }
            Items.Clear();
            foreach (var item in package.Items)
            {
                Items.Add(new UnitViewModel(item));
            }
        }

        public void Load()
        {
            App.ViewModel.Packages.Clear();
            SourceLang = "en";
            TargetLang = string.Empty;
            Items.Clear();
        }

        public async Task LoadAsync(string fileName, bool fillEmpty = false)
        {
            var reader = AppViewModel.Reader(Path.GetExtension(fileName));
            if (reader == null)
            {
                return;
            }
            var package = await reader.ReadAsync(fileName);
            if (package == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(package.FileName))
            {
                package.FileName = fileName;
            }
            App.ViewModel.AddPackage(package);
            App.Current.Dispatcher.Invoke(() =>
            {
                LoadAsync(package, fillEmpty);
            });
        }

        public void LoadAsync(LanguagePackage? package, bool fillEmpty = false)
        {
            if (package == null)
            {
                return;
            }
            if (SourceLanguage == null && package.Language != null)
            {
                SourceLanguage = package.Language;
                sourceLang = package.Language.Code;
            }
            if (package.TargetLanguage != null)
            {
                targetLang = package.TargetLanguage.ToString();
                OnPropertyChanged(nameof(TargetLang));
            }
            Merge(package.Items, Items.Count == 0, fillEmpty);
        }



        public async Task SaveAsync(string fileName)
        {
            var reader = AppViewModel.Reader(Path.GetExtension(fileName));
            if (reader == null)
            {
                return;
            }
            var package = new LanguagePackage(SourceLanguage, CurrentLanguage)
            {
                FileName = fileName,
            };
            foreach (var item in Items)
            {
                package.Items.Add(item.To());
            }
            await reader.WriteAsync(fileName, package);
            App.ViewModel.DispatcherQueue.Invoke(() => {
                MessageBox.Show("保存成功");
            });
        }
        public async Task SaveAsync()
        {
            var package = App.ViewModel.CurrentPackage;
            if (package is not null && !string.IsNullOrWhiteSpace(package.FileName))
            {
                await SaveAsync(package.FileName);
            }
        }

        public void Save(LangItem lang, IEnumerable<UnitViewModel> unitItems)
        {
            LanguagePackage package;
            var has = App.ViewModel.Packages.ContainsKey(lang.Code);
            if (has)
            {
                package = App.ViewModel.Packages[lang.Code];
                package.Items.Clear();
            }
            else
            {
                package = new LanguagePackage(lang);
            }
            foreach (var item in unitItems)
            {
                package.Items.Add(item.To());
            }
            App.ViewModel.AddPackage(package);
        }
    }
}
