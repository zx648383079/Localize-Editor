using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class LangItem
    {
        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public LangItem(string code, string name)
        {
            Name = name;
            Code = code;
        }

        public LangItem(string code): this(code, code)
        {

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
            if (obj is LangItem o)
            {
                return o.Code == Code;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            var hashCode = -168117446;
            return hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Code);
        }
    }
}
