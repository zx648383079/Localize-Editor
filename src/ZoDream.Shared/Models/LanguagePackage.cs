using System.Collections.Generic;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Models
{
    public class LanguagePackage
    {
        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public string TargetLanguage { get; set; } = string.Empty;

        public IList<UnitItem> Items { get; set; } = new List<UnitItem>();

        /// <summary>
        /// 一些配置信息
        /// </summary>
        public Dictionary<string, string> MetaItems { get; set; } = new Dictionary<string, string>();

        public LanguagePackage(LangItem lang, LangItem target): this(lang.Code, target.Code)
        {
        }

        public LanguagePackage(LangItem lang): this(lang.Code)
        {
        }

        public LanguagePackage(string lang, string target): this(lang)
        {
            TargetLanguage = target;
        }

        public LanguagePackage(string lang)
        {
            Language = lang;
        }
    }
}
