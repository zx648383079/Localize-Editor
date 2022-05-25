using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Storage
{
    public static class LanguageFile
    {

        public static async Task<string[]> LoadAsync(string fileName)
        {
            var items = new List<string>();
            using (var reader = LocationStorage.Reader(fileName))
            {
                string? line;
                while (null != (line = await reader.ReadLineAsync()))
                {
                    var item = Format(line);
                    if (item == null)
                    {
                        continue;
                    }
                    items.Add(item.ToString());
                }
            }
            return items.ToArray();
        }

        public static LangItem? Format(string? line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }
            var args = line.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return new LangItem(args[0], args[args.Length - 1]);
        }
    }
}
