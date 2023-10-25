using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.WordPress
{
    public class PhpReader : IReader
    {
        public async Task<LanguagePackage> ReadAsync(string file)
        {
            var package = new LanguagePackage("en");
            var content = await LocationStorage.ReadAsync(file);
            var matches = Regex.Matches(content, @"['""](.+?)['""]\s=>\s['""](.+?)['""]");
            if (matches is null)
            {
                return package;                
            }
            foreach (Match item in matches)
            {
                package.Items.Add(new UnitItem(item.Groups[1].Value, item.Groups[2].Value)
                {
                    Id = item.Groups[1].Value,
                });
            }
            return package;
        }

        public async Task WriteAsync(string file, LanguagePackage package)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<?php");
            sb.AppendLine("/**");
            sb.AppendLine($" * @language {package.TargetLanguage}");
            sb.AppendLine($" * @date {DateTime.Now}");
            sb.AppendLine("*/");
            sb.AppendLine("return [");
            foreach (var item in package.Items)
            {
                var source = string.IsNullOrWhiteSpace(item.Id) ? item.Source : item.Id;
                sb.AppendLine($"    '{source}' => '${item.Target}',");
            }
            sb.AppendLine("];");
            await LocationStorage.WriteAsync(file, sb.ToString());
        }
    }
}
