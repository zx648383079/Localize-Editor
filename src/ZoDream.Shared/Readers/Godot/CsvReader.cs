using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.Godot
{
    public class CsvReader : IReader
    {
        public Task<IList<LanguagePackage>> ReadAsync(string file)
        {
            return Task.Factory.StartNew(() => {
                return Read(file);
            });
        }
        public Task<LanguagePackage> ReadFileAsync(string file)
        {
            return Task.Factory.StartNew(() => 
            {
                return Read(file)[0];
            });
        }

        public IList<LanguagePackage> Read(string file)
        {
            using var reader = LocationStorage.Reader(file);
            var items = new List<LanguagePackage>();
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var args = line.Split(',');
                if (items.Count == 0)
                {
                    for (var i = 1; i < args.Length; i++)
                    {
                        items.Add(new LanguagePackage(args[1], args[i]));
                    }
                    continue;
                }
                var id = args[0];
                for (var i = 1; i < Math.Min(args.Length, items.Count + 1); i++)
                {
                    items[i - 1].Items.Add(new UnitItem(args[1], args[i])
                    {
                        Id = id,
                    });
                }
            }
            return items;
        }

        public Task WriteAsync(string file, LanguagePackage package)
        {
            return Task.Factory.StartNew(() => 
            {
                Write(file, [package]);
            });
        }

        public Task WriteAsync(string file, IEnumerable<LanguagePackage> items)
        {
            return Task.Factory.StartNew(() => {
                Write(file, items);
            });
        }

        public void Write(string file, IEnumerable<LanguagePackage> items)
        {
            using var writer = LocationStorage.Writer(file);
            IList<UnitItem>? maxKeys = null;
            foreach (var item in items) 
            {
                if (string.IsNullOrWhiteSpace(item.TargetLanguage))
                {
                    continue;
                }
                writer.Write($",{item.TargetLanguage}");
                if (maxKeys is null || item.Items.Count > maxKeys.Count)
                {
                    maxKeys = item.Items;
                }
            }
            if (maxKeys is null)
            {
                return;
            }
            writer.WriteLine();
            foreach (var key in maxKeys)
            {
                writer.Write(key.Id);
                foreach (var item in items)
                {
                    if (string.IsNullOrWhiteSpace(item.TargetLanguage))
                    {
                        continue;
                    }
                    var target = string.Empty;
                    foreach (var it in item.Items)
                    {
                        if (it.Id == key.Id && !string.IsNullOrWhiteSpace(it.Target))
                        {
                            target = it.Target;
                        }
                    }
                    writer.Write($",{target}");
                }
                writer.WriteLine();
            }
        }

        
    }
}
