using System;
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
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public partial class HomeViewModel
    {


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

        private string[] langItems = Array.Empty<string>();

        public string[] LangItems
        {
            get => langItems;
            set => Set(ref langItems, value);
        }

        private int unitSelectedIndex = -1;

        public int UnitSelectedIndex 
        {
            get => unitSelectedIndex;
            set => Set(ref unitSelectedIndex, value);
        }



        private ObservableCollection<UnitViewModel> items = new();

        public ObservableCollection<UnitViewModel> Items
        {
            get => items;
            set => Set(ref items, value);
        }

    }
}
