using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class LangItem
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public LangItem(string code, string name)
        {
            Name = name;
            Code = code;
        }

        public override string ToString()
        {
            if (Code == Name || string.IsNullOrWhiteSpace(Name))
            {
                return Code;
            }
            return $"{Code} {Name}";
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }
            if (obj is LangItem)
            {
                return (obj as LangItem).Code == Code;
            }
            return base.Equals(obj);
        }
    }
}
