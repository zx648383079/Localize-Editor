using System;
using System.Collections.Generic;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class UnitViewModel: BindableBase, ITranslateUnit
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

        public bool HasSameFile(UnitItem source)
        {
            throw new NotImplementedException();
        }
    }
}
