using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.ViewModel;

namespace ZoDream.Shared.Models
{
    public class InputItem: BindableBase
    {
        public string Name { get; set; }

        public string Label { get; set; }

        private string inputValue = string.Empty;

        public string Value {
            get => inputValue;
            set => Set(ref inputValue, value);
        }

        public InputItem(string name, string label)
        {
            Name = name;
            Label = label;
        }

        public InputItem(string name, string label, string? val): this(name, label)
        {
            if (val is null)
            {
                return;
            }
            Value = val;
        }

        public InputItem(string name, string label, Dictionary<string, string>? data): this(name, label)
        {
            if (data is not null && data.TryGetValue(name, out var val))
            {
                Value = val;
            }
        }

        public InputItem(string name, Dictionary<string, string>? data) : this(name, name, data)
        {
        }
    }
}
