﻿using System;
using System.Linq;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public static class ReaderFactory
    {

        public static IReader? Reader(string name)
        {
            var i = name.LastIndexOf('.');
            if (i >= 0)
            {
                name = name.Substring(i + 1);
            }
            return name.ToLower() switch
            {
                "xlf" => new Angular.XlfReader(),
                "mo" => new WordPress.MoReader(),
                "po" => new WordPress.PoReader(),
                "resx" => new CSharp.ResXReader(),
                "csv" => new Godot.CsvReader(),
                "php" => new WordPress.PhpReader(),
                "json" => new Angular.JsonReader(),
                "lrc" => new Subtitle.LrcReader(),
                "srt" => new Subtitle.SrtReader(),
                _ => null,
            };
        }

        /// <summary>
        /// 生成文件过滤
        /// </summary>
        /// <param name="firstExtension"></param>
        /// <returns></returns>
        public static string RenderFileFilter(string? firstExtension = null)
        {
            var items = new string[] { 
                "XLFF文件|*.xlf",
                "JSON文件|*.json",
                "RESW文件|*.resw",
                "CSV文件|*.csv",
                "PO文件|*.po;*.mo",
                "PHP文件|*.php",
                "字幕及歌词文件|*.srt;*.lrc",
                "所有文件|*.*"
            };
            if (string.IsNullOrWhiteSpace(firstExtension))
            {
                return string.Join("|", items);
            }
            var i = firstExtension!.LastIndexOf('.');
            if (i >= 0)
            {
                firstExtension = firstExtension.Substring(i + 1);
            }
            return string.Join("|", items.OrderBy(item => item.IndexOf(firstExtension) >= 0 ? -1 : 0));
        }

        public static string RenderFileName(string fileName, LanguagePackage package)
        {
            var tag = "{lang}";
            var i = fileName.IndexOf(tag);
            if (i == 0)
            {
                return fileName.Replace(tag, package.TargetLanguage);
            }
            else if (i < 0)
            {
                return fileName;
            }
            if (package.TargetLanguage != package.Language)
            {
                return fileName.Replace(tag, package.TargetLanguage);
            }
            var end = i + tag.Length;
            var prefix = fileName[i - 1];
            if (prefix == ' ' || prefix == '.' 
                || prefix == '-' || prefix == '/' || prefix == '\\')
            {
                i--;
            }
            return fileName.Substring(0, i) + fileName.Substring(end);
        }

        public static string RenderFileName(string fileName, string extension, LanguagePackage package)
        {
            fileName = RenderFileName(fileName, package);
            if (string.IsNullOrWhiteSpace(extension))
            {
                return fileName;
            }
            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }
            if (!fileName.EndsWith(extension, StringComparison.CurrentCultureIgnoreCase))
            {
                fileName += extension;
            }
            return fileName;
        }
    }
}
