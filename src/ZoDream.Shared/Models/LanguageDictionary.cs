using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoDream.Shared.Models
{
    public class LanguageDictionary: Dictionary<string, string>
    {

        public void Push(string code)
        {
            Push(code, code);
        }

        public void Push(string code, string label)
        {
            code = FormatCode(code);
            if (string.IsNullOrEmpty(code) || ContainsKey(code))
            {
                return;
            }
            Add(code, label);
        }

        public string RepairCode(string key)
        {
            if (key.Contains(" "))
            {
                key = key.Trim().Split(new char[] { ' ' }, 2)[0];
            }
            var args = SplitCode(key);
            var format = FormatCode(key);
            if (args.Length == 0)
            {
                return format;
            }
            if (ContainsKey(format))
            {
                return format;
            }
            if (ContainsKey(args[0]))
            {
                return args[0];
            }
            return format;
        }

        public string GetShortCode(string key)
        {
            var args = SplitCode(key);
            if (args.Length == 0)
            {
                return string.Empty;
            }
            return args[0];
        }

        public bool TryGetLabel(string key, out string value)
        {
            var args = SplitCode(key);
            if (args.Length == 0)
            {
                value = key;
                return false;
            }
            if (TryGetValue(FormatCode(args), out value))
            {
                return true;
            }
            if (TryGetValue(args[0], out value))
            {
                return true;
            }
            value = key;
            return false;
        }

        public LangItem[] ToArray()
        {
            return this.Select(item => new LangItem(item.Key, item.Value)).ToArray();
        }

        public string[] ToStringArray()
        {
            return this.Select(item => $"{item.Key} {item.Value}").ToArray();
        }

        public string CodeToString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            key = RepairCode(key);
            if (TryGetValue(key, out var val))
            {
                return $"{key} {val}";
            }
            return string.Empty;
        }

        private string FormatCode(string code)
        {
            return FormatCode(SplitCode(code));
        }

        private string FormatCode(string[] args) 
        {
            if (args.Length == 0)
            {
                return string.Empty;
            }
            if (args.Length == 1)
            {
                return args[0];
            }
            return string.Join("-", args);
        }

        private string[] SplitCode(string code)
        {
            var args = code.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0)
            {
                return args;
            }
            args[0] = args[0].ToLower();
            if (args.Length == 1)
            {
                return args;
            }
            args[1] = args[1].ToUpper();
            return args;
        }

    
    }
}
