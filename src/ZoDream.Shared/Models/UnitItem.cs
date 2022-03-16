using System;
using System.Collections.Generic;
using System.Linq;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class UnitItem: BindableBase
    {

        private string id;

        public string Id
        {
            get => id;
            set => Set(ref id, value);
        }


        private string _source;

        public string Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        private string _target = string.Empty;

        public string Target
        {
            get => _target;
            set => Set(ref _target, value);
        }

        private List<SourceLocation> _location = new();

        public List<SourceLocation> Location
        {
            get => _location;
            set => Set(ref _location, value);
        }

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

        
    }
}
