using System;
using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class UnitItem: ITranslateUnit
    {

        public string Id { get; set; } = string.Empty;


        public string Source { get; set; } = string.Empty;

        public string SourcePlural { get; set; } = string.Empty; // 翻译的文字复数形式

        public string Target { get; set; } = string.Empty;

        public List<string> TargetPlural { get; set; } = new(); // 翻译的文字复数形式，可以有多种

        public List<SourceLocation> Location { get; set; } = new();

        public string Comment { get; set; } = string.Empty;

        public UnitItem()
        {

        }

        public UnitItem(string source)
        {
            Id = Source = source;
        }

        public UnitItem(string source, string target): this(source)
        {
            Target = target;
        }

        public UnitItem(string source, string target, string fileName, int line): this(source, target)
        {
            Location.Add(new SourceLocation(fileName, line));
        }

        public UnitItem(string source, string target, string fileName, string line) : this(source, target)
        {
            Location.Add(new SourceLocation(fileName, line));
        }

        public void AddLine(string fileName, string lineNo)
        {
            throw new NotImplementedException();
        }
    }
}
