using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.ViewModel;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class LangePackageViewModel(string sourceLang, string targetLang): BindableBase
    {

        public string SourceLanguage { get; set; } = sourceLang;

        public string TargetLanguage { get; set; } = targetLang;

        private bool isActived;

        public bool IsActived {
            get => isActived;
            set => Set(ref isActived, value);
        }

    }
}
