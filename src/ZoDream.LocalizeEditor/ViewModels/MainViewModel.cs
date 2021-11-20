using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared;
using ZoDream.Shared.Models;
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

        private IDictionary<string, LanguagePackage> Packages = new Dictionary<string, LanguagePackage>();
        private LangItem? CurrentLanguage;

        private string[]? langItems = null;

        public string[] LangItems
        {
            get => langItems;
            set => Set(ref langItems, value);
        }

        private ObservableCollection<UnitItem> items = new ObservableCollection<UnitItem>();

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
                    (string.IsNullOrWhiteSpace(e.Target) || e.Target == items[i].Target))
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

        public void Add(UnitItem e, bool merge)
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
            if (!string.IsNullOrWhiteSpace(e.FileName) && e.FileName != target.FileName)
            {
                target.FileName = string.Empty;
                target.LineNumber = "";
            } else
            {
                target.AddLine(e.LineNumber);
            }
        }

        public void ChangeLanguage(string lang)
        {
            var l = LanguageFile.Format(lang);
            if (l == CurrentLanguage)
            {
                return;
            }
            if (CurrentLanguage == null)
            {
                CurrentLanguage = l;
                return;
            }
            Save(CurrentLanguage, Items);
            Load(l);
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
    }
}
