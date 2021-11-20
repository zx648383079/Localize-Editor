using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Models
{
    public class LanguagePackage
    {
        public LangItem Language { get; set; }

        public IList<UnitItem> Items { get; set; } = new List<UnitItem>();

        public LanguagePackage(LangItem lang)
        {
            Language = lang;
        }

        public LanguagePackage(string lang)
        {
            Language = LanguageFile.Format(lang);
        }
    }
}
