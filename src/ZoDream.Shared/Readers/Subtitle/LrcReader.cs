﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.Subtitle
{
    public class LrcReader : IReader
    {
        public async Task<IList<LanguagePackage>> ReadAsync(string file)
        {
            return [await ReadFileAsync(file)];
        }
        public Task<LanguagePackage> ReadFileAsync(string file)
        {
            return Task.Factory.StartNew(() => 
            {
                return Read(file);
            });
        }

        public LanguagePackage Read(string file)
        {
            var reader = LocationStorage.Reader(file);
            var package = new LanguagePackage("en");
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
                var i = line.LastIndexOf(']');
                if (i < 1)
                {
                    continue;
                }
                package.Items.Add(new UnitItem(line.Substring(i + 1), string.Empty)
                {
                    Id = line.Substring(0, i + 1),
                });
            }
            return package;
        }

        public async Task WriteAsync(string file, IEnumerable<LanguagePackage> items)
        {
            foreach (var item in items)
            {
                await WriteAsync(file, item);
            }
        }
        public Task WriteAsync(string file, LanguagePackage package)
        {
            return Task.Factory.StartNew(() => 
            {
                Write(ReaderFactory.RenderFileName(file, package), package);
            });
        }

        public void Write(string file, LanguagePackage package)
        {
            var writer = LocationStorage.Writer(file);
            foreach (var item in package.Items)
            {
                writer.WriteLine(item.Id + item.Target);
            }
        }
    }
}
