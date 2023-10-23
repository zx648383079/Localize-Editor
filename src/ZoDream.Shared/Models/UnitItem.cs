using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class UnitItem
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
            Source = source;
        }

        public UnitItem(string source, string target)
        {
            Source = source;
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

        public void AddLine(string fileName, string line)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            foreach (var item in Location)
            {
                if (item.FileName == fileName)
                {
                    item.Add(line);
                    return;
                }
            }
            Location.Add(new SourceLocation(fileName, line));
        }


        public void AddLine(SourceLocation location)
        {
            if (string.IsNullOrEmpty(location.FileName))
            {
                return;
            }
            foreach (var item in Location)
            {
                if (item.FileName == location.FileName)
                {
                    item.Add(location.LineNumber);
                    return;
                }
            }
            Location.Add(location);
        }

        public int IndexOf(SourceLocation e)
        {
            return IndexOf(e.FileName);
        }

        public int IndexOf(string file)
        {
            for (int i = 0; i < Location.Count; i++)
            {
                if (Location[i].FileName == file)
                {
                    return i;
                }
            }
            return -1;
        }

        public void AddLine(List<SourceLocation> items)
        {
            if (items.Count == 0)
            {
                return;
            }
            foreach (var item in items)
            {
                var i = IndexOf(item);
                if (i < 0)
                {
                    Location.Add(item);
                    continue;
                }
                Location[i].Add(item.LineNumber);
            }
        }

        public UnitItem Clone()
        {
            return new UnitItem(Source, string.Empty)
            {
                Id = Id,
                Location = Location,
            };
        }

        public void SetTarget(string[] items)
        {
            for (var i = 0; i < items.Length; i++)
            {
                if (i < 1)
                {
                    Target = items[i];
                } else
                {
                    TargetPlural.Add(items[i]);
                }
            }
        }

        public void AddTarget(string item)
        {
            if (string.IsNullOrEmpty(Target))
            {
                Target = item;
                return;
            }
            TargetPlural.Add(item);
        }
    }
}
