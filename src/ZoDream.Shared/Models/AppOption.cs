using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class AppOption
    {

        public bool UseBrowser { get; set; } = true;
        public string Translator { get; set; } = string.Empty;

        public Dictionary<string, string> TranslatorData { get; set; } = new();
    }
}
