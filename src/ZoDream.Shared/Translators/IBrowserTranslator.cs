using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Translators
{
    public interface IBrowserTranslator
    {
        public string EntryURL { get; }

        public string TranslateScript(string sourceLang, string targetLang, string text);

        public string GetScript();
    }
}
