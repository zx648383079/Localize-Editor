using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Storage
{
    public static class LanguageFile
    {
        public static async Task<LanguageDictionary> LoadAsync(string fileName)
        {
            var data = new LanguageDictionary();
            using var reader = LocationStorage.Reader(fileName);
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null)
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var args = line.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length < 2) 
                {
                    data.Push(args[0]);
                } else
                {
                    data.Push(args[0], args[1]);
                }
            }
            return data;
        }
    }
}
