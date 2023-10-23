using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class SourceLocation
    {
        public string FileName { get; set; } = string.Empty;

        public List<int> LineNumber { get; set; } = new();

        public string LineNumberFormat => string.Join(",", LineNumber);

        public SourceLocation()
        {

        }

        public SourceLocation(string fileName, int line)
        {
            FileName = fileName;
            Add(line);
        }

        public SourceLocation(string fileName, string line)
        {
            FileName = fileName;
            Add(line);
        }

        public SourceLocation(string fileName, IEnumerable<object> lines)
        {
            FileName = fileName;
            Add(lines);
        }

        public void Add(int line)
        {
            if (line < 0 || LineNumber.IndexOf(line) >= 0)
            {
                return;
            }
            LineNumber.Add(line);
        }

        public void Add(string line)
        {
            Add(line.Split(','));
        }

        public void Add(IEnumerable<int> lines)
        {
            foreach (var item in lines)
            {
                Add(item);
            }
        }

        public void Add(IEnumerable<object> lines)
        {
            foreach (var item in lines)
            {
                if (item == null)
                {
                    continue;
                }
                if (item is int)
                {
                    Add((int)item);
                    continue;
                }
                var val = item.ToString();
                if (string.IsNullOrWhiteSpace(val))
                {
                    continue;
                }
                Add(Convert.ToInt32(val));
            }
        }
    }
}
