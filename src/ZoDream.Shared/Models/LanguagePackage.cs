using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Models
{
    public class LanguagePackage
    {
        public LangItem Language { get; set; }

        public LangItem? TargetLanguage { get; set; }

        public IList<UnitItem> Items { get; set; } = new List<UnitItem>();

        public LanguagePackage(LangItem lang, LangItem target)
        {
            Language = lang;
            TargetLanguage = target;
        }

        public LanguagePackage(LangItem lang)
        {
            Language = lang;
        }

        public LanguagePackage(string lang, string target)
        {
            Language = LanguageFile.Format(lang);
            TargetLanguage = LanguageFile.Format(target);
        }

        public LanguagePackage(string lang)
        {
            Language = LanguageFile.Format(lang);
        }
    }
}
