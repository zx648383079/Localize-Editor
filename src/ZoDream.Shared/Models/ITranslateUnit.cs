using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public interface ITranslateUnit
    {

        public string Id { get; set; }


        public string Source { get; set; }

        public string SourcePlural { get; set; } // 翻译的文字复数形式

        public string Target { get; set; }

        public List<string> TargetPlural { get; set; }// 翻译的文字复数形式，可以有多种

        public List<SourceLocation> Location { get; set; }

        public string Comment { get; set; }

    }
}
