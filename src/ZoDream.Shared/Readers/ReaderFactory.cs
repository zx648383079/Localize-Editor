using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Readers
{
    public static class ReaderFactory
    {

        public const string FileFilters = "XLFF文件|*.xlf|JSON文件|*.json|RESW文件|*.resw|PO文件|*.po;*.mo|PHP文件|*.php|字幕及歌词文件|*.srt;*.lrc|所有文件|*.*";

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
                "php" => new WordPress.PhpReader(),
                "json" => new Angular.JsonReader(),
                "lrc" => new Subtitle.LrcReader(),
                "srt" => new Subtitle.SrtReader(),
                _ => null,
            };
        }
    }
}
