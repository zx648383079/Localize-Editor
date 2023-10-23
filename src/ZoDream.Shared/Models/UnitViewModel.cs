using System;
using System.Collections.Generic;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class UnitViewModel: BindableBase
    {

        private string id = string.Empty;

        public string Id {
            get => id;
            set => Set(ref id, value);
        }

        private string source = string.Empty;

        public string Source {
            get => source;
            set => Set(ref source, value);
        }

        private string sourcePlural = string.Empty; // 翻译的文字复数形式

        public string SourcePlural {
            get => sourcePlural;
            set => Set(ref sourcePlural, value);
        }


        private string target = string.Empty;

        public string Target {
            get => target;
            set => Set(ref target, value);
        }

        public List<string> TargetPlural { get; set; } = new(); // 翻译的文字复数形式，可以有多种

        public List<SourceLocation> Location { get; set; } = new();

        private string comment = string.Empty;

        public string Comment {
            get => comment;
            set => Set(ref comment, value);
        }

        public UnitViewModel()
        {

        }

        public UnitViewModel(UnitItem source)
        {
            Id = source.Id;
            Source = source.Source;
            SourcePlural = source.SourcePlural;
            Target = source.Target;
            TargetPlural = source.TargetPlural;
            Location = source.Location;
            Comment = source.Comment;
        }

        public UnitItem To()
        {
            return new UnitItem()
            {
                Id = Id,
                Source = Source,
                SourcePlural = SourcePlural,
                Target = Target,
                TargetPlural = TargetPlural,
                Location = Location,
                Comment = Comment
            };
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

        public UnitViewModel Clone()
        {
            return new UnitViewModel()
            {
                Source = Source,
                Target = string.Empty,
                Id = Id,
                Location = Location,
                Comment = Comment,
            };
        }
    }
}
