using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class UnitItem: BindableBase
    {
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

        private string _fileName = string.Empty;

        public string FileName
        {
            get => _fileName;
            set => Set(ref _fileName, value);
        }


        private string _lineNumber = string.Empty;

        public string LineNumber
        {
            get => _lineNumber;
            set => Set(ref _lineNumber, value);
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
            FileName = fileName;
            LineNumber = line.ToString();
        }

        public UnitItem(string source, string target, string fileName, string line) : this(source, target)
        {
            FileName = fileName;
            LineNumber = line;
        }

        public void AddLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(LineNumber))
            {
                LineNumber = line;
                return;
            }
            LineNumber = $"{LineNumber},{line}";
        }

        public UnitItem Clone()
        {
            return new UnitItem(Source, string.Empty, FileName, LineNumber);
        }
    }
}
