﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModels;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            LoadAsync();
        }

        public string SourceFile = string.Empty;
        private IDictionary<string, LanguagePackage> Packages = new Dictionary<string, LanguagePackage>();
        private LangItem? CurrentLanguage;
        private LangItem? SourceLanguage;

        private string sourceLang = "en";

        public string SourceLang
        {
            get => sourceLang;
            set {
                Set(ref sourceLang, value);
                SourceLanguage = LanguageFile.Format(value);
            }
        }

        private string targetLang = string.Empty;

        public string TargetLang
        {
            get => targetLang;
            set {
                Set(ref targetLang, value);
                ChangeLanguage(value);
            }
        }

        private string[]? langItems = null;

        public string[] LangItems
        {
            get => langItems;
            set => Set(ref langItems, value);
        }

        private ObservableCollection<UnitItem> items = new();

        public ObservableCollection<UnitItem> Items
        {
            get => items;
            set => Set(ref items, value);
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
                Items.Add(e);
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

        private UnitItem? GetUnit(IEnumerable<UnitItem> items, UnitItem source, bool notEmpty, bool checkSameFile)
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

        private bool HasSameFile(UnitItem dist, UnitItem source)
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

        private bool ShouldMerge(UnitItem dist, UnitItem source, bool byId = true, bool byPath = false)
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
            if (lang is null || !Packages.ContainsKey(lang.Code))
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    Items[i] = Items[i].Clone();
                }
                return;
            }
            var package = Packages[lang.Code];
            Items.Clear();
            foreach (var item in package.Items)
            {
                Items.Add(item);
            }
        }

        public void Load()
        {
            Packages.Clear();
            SourceLang = "en";
            TargetLang = string.Empty;
            Items.Clear();
            SourceFile = string.Empty;
        }

        public async Task LoadAsync(string fileName, bool fillEmpty = false)
        {
            var reader = Render(Path.GetExtension(fileName));
            if (reader == null)
            {
                return;
            }
            var package = await reader.ReadAsync(fileName);
            if (package == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(SourceFile))
            {
                SourceFile = fileName;
            }
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
                TargetLang = package.TargetLanguage.ToString();
            }
            Merge(package.Items, Items.Count == 0, fillEmpty);
        }



        public async Task SaveAsync(string fileName)
        {
            var reader = Render(Path.GetExtension(fileName));
            if (reader == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(SourceFile))
            {
                SourceFile = fileName;
            }
            var package = new LanguagePackage(SourceLanguage, CurrentLanguage);
            foreach (var item in Items)
            {
                package.Items.Add(item);
            }
            await reader.WriteAsync(fileName, package);
        }
        public async Task SaveAsync()
        {
            if (!string.IsNullOrWhiteSpace(SourceFile))
            {
                await SaveAsync(SourceFile);
            }
        }

        public void Save(LangItem lang, IEnumerable<UnitItem> unitItems)
        {
            LanguagePackage package;
            var has = Packages.ContainsKey(lang.Code);
            if (has)
            {
                package = Packages[lang.Code];
                package.Items.Clear();
            }
            else
            {
                package = new LanguagePackage(lang);
            }
            foreach (var item in unitItems)
            {
                package.Items.Add(item);
            }
            if (!has)
            {
                Packages.Add(lang.Code, package);
            }
        }

        public IReader? Render(string name)
        {
            switch (name.ToLower())
            {
                case "xlf":
                case ".xlf":
                    return new Shared.Readers.Angular.XlfReader();
                default:
                    return null;
            }
        }
    }
}
