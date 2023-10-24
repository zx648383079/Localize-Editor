using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using ZoDream.Shared.Models;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class HomeViewModel
    {

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

        private string sourceLang = string.Empty;

        public string SourceLang
        {
            get => sourceLang;
            set {
                Set(ref sourceLang, value);
                OnPropertyChanged(nameof(SourceHeader));
            }
        }

        private string targetLang = string.Empty;

        public string TargetLang
        {
            get => targetLang;
            set {
                Set(ref targetLang, value);
                OnPropertyChanged(nameof(TargetHeader));
            }
        }

        public string SourceHeader => $"源: {SourceLang}";
        public string TargetHeader => $"翻译: {TargetLang}";

        private string dialogTargetLang = string.Empty;

        public string DialogTargetLang {
            get => dialogTargetLang;
            set => Set(ref dialogTargetLang, value);
        }

        private string[] langItems = Array.Empty<string>();

        public string[] LangItems
        {
            get => langItems;
            set => Set(ref langItems, value);
        }

        private UnitViewModel? unitSelectedItem;

        public UnitViewModel? UnitSelectedItem 
        {
            get => unitSelectedItem;
            set => Set(ref unitSelectedItem, value);
        }


        private string keywords = string.Empty;

        public string Keywords 
        {
            get => keywords;
            set => Set(ref keywords, value);
        }

        public ICollectionView FilteredItems { get; set; }

        private ObservableCollection<UnitViewModel> Items = new();

    }
}
