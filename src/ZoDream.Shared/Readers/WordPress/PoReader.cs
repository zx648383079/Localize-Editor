﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.WordPress
{
    public class PoReader : IReader
    {
        public const string LanguageTag = "Language";

        private string? LastLine;
        private bool MoveNextStop;
        public Task<LanguagePackage> ReadAsync(string file)
        {
            return Task.Factory.StartNew(() => 
            {
                return ReadFile(file);
            });
        }

        public LanguagePackage ReadFile(string file)
        {
            using var reader = LocationStorage.Reader(file);
            return ReadStream(reader);
        }

        private string? ReadLine(StreamReader reader)
        {
            if (MoveNextStop)
            {
                MoveNextStop = false;
                return LastLine;
            }
            LastLine = reader.ReadLine();
            return LastLine;
        }

        public LanguagePackage ReadStream(StreamReader reader)
        {
            var package = new LanguagePackage("en");
            ReadHeader(package, reader);
            while (true)
            {
                var item = ReadItem(reader);
                if (item is null)
                {
                    break;
                }
                package.Items.Add(item);
            }
            return package;
        }

        private void ReadHeader(LanguagePackage package, StreamReader reader)
        {
            var item = ReadItem(reader);
            if (item is null)
            {
                return;
            }
            if (!string.IsNullOrEmpty(item.Id))
            {
                package.Items.Add(item);
                return;
            }
            var comment = item.Comment.Split('\n');
            package.Title = comment[0];
            if (comment.Length > 1)
            {
                package.Description = comment[1];
            }
            foreach (var line in item.Target.Split('\n'))
            {
                var args = line.Split(new char[] { ':' }, 2);
                if (args.Length < 2)
                {
                    continue;
                }
                var name = args[0].Trim();
                var value = args[1].EndsWith("\\n") ? args[1].Substring(0, args[1].Length - 2) : args[1];
                if (name == LanguageTag)
                {
                    package.TargetLanguage = LanguageFile.Format(value);
                    continue;
                }
                package.MetaItems.Add(name, value);
            }
        }

        private string ReadString(string line)
        {
            if (line.Length > 0 && line[0] == '"' && line[line.Length - 1] == '"')
            {
                return line.Substring(1, line.Length - 2).Replace("\\\"", "\"");
            }
            return line;
        }

        private UnitItem? ReadItem(StreamReader reader)
        {
            var item = new UnitItem();
            var found = false;
            while (true)
            {
                var line = ReadLine(reader);
                if (found && string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                if (line == null)
                {
                    return null;
                }
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var args = line.Split(new char[] { ' ' }, 2);
                switch (args[0])
                {
                    case "#":
                    case "#.": // 参数说明
                        AddItemComment(item, args[1].Trim());
                        break;
                    case "#,":
                        // 不支持
                        break;
                    case "#:":
                        var next = args[1].Trim();
                        var i = next.LastIndexOf(':');
                        if (i < 0)
                        {
                            item.AddLine(next, string.Empty);
                        }
                        else
                        {
                            item.AddLine(next.Substring(0, i), next.Substring(i + 1));
                        }
                        break;
                    case "msgid_plural":
                        item.SourcePlural = ReadIfManyLine(reader, args[1].Trim());
                        break;
                    case "msgid":
                        item.Source = ReadIfManyLine(reader, args[1].Trim());
                        break;
                    case "msgctxt": // 上下文，消除同id的歧义
                        // 不支持
                        item.Comment = args[1].Trim();
                        break;
                    default:
                        break;
                }
                if (args[0].StartsWith("msgstr"))
                {
                    found = true;
                    var i = 6;
                    var offset = 0;
                    if (line[6] == '[')
                    {
                        i = line.IndexOf(']', 6);
                        int.TryParse(line.Substring(7, i - 7), out offset);
                    }
                    var val = line.Substring(i + 1).Trim();
                    if (offset < 1)
                    {
                        item.Target = ReadIfManyLine(reader, val);
                    } else
                    {
                        item.TargetPlural.Add(ReadIfManyLine(reader, val));
                    }
                    if (i == 6)
                    {
                        break;
                    }
                }
            }
            return item;
        }

        private string ReadIfManyLine(StreamReader reader, string val)
        {
            val = ReadString(val);
            if (!string.IsNullOrEmpty(val))
            {
                return val;
            }
            long begin;
            var sb = new StringBuilder();
            while (true)
            {
                var line = ReadLine(reader);
                if (line == null)
                {
                    break;
                }
                if (line.Length < 1 || line[0] != '"')
                {
                    MoveNextStop = true;
                    break;
                }
                sb.Append(ReadString(line).Replace("\\n", "\n"));
            }
            return sb.ToString();
        }

        private void AddItemComment(UnitItem item, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(item.Comment))
            {
                item.Comment = comment;
                return;
            }
            item.Comment = '\n' + comment;
        }

        public Task WriteAsync(string file, LanguagePackage package)
        {
            return Task.Factory.StartNew(() => {
                WriteFile(file, package);
            });
        }

        public void WriteFile(string file, LanguagePackage package)
        {
            using var writer = LocationStorage.Writer(file);
            Write(writer, package);
        }

        public void Write(StreamWriter writer, LanguagePackage package)
        {
            WriteHeader(writer, package);
            foreach (var item in package.Items)
            {
                WriteItem(writer, item);
            }
        }

        private void WriteHeader(StreamWriter writer, LanguagePackage package)
        {
            WriteComment(writer, package.Title);
            WriteComment(writer, package.Description);
            WriteString(writer, "msgid", string.Empty);
            WriteString(writer, "msgstr", string.Empty);
            foreach (var item in package.MetaItems)
            {
                WriteMeta(writer, item.Key, item.Value);
            }
            WriteMeta(writer, LanguageTag, package.TargetLanguage!.Code);
            writer.WriteLine();
        }
        private void WriteString(StreamWriter writer, string tag, string content)
        {
            content = content.Replace("\"", "\\\"").TrimEnd('\n');
            var args = content.Split('\n');
            if (args.Length < 2)
            {
                writer.WriteLine($"{tag} \"{content}\"");
                return;
            }
            writer.WriteLine($"{tag} \"\"");
            foreach (var item in args)
            {
                writer.WriteLine($"\"${item}\\n\"");
            }
        }

        private void WriteString(StreamWriter writer, string content)
        {
            content = content.Replace("\"", "\\\"");
            writer.WriteLine($"\"{content}\"");
        }

        private void WriteMeta(StreamWriter writer, string key, string value)
        {
            writer.WriteLine($"\"{key}:{value}\\n\"");
        }

        private void WriteComment(StreamWriter writer, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }
            foreach (var item in content.Split('\n'))
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                writer.WriteLine($"# {item}");
            }
        }

        private void WriteItem(StreamWriter writer, UnitItem item)
        {
            WriteComment(writer, item.Comment);
            foreach (var line in item.Location)
            {
                writer.WriteLine($"#: {line.FileName}:{line.LineNumberFormat}");
            }
            WriteString(writer, "msgid", item.Source);
            if (string.IsNullOrEmpty(item.SourcePlural))
            {
                WriteString(writer, "msgstr", item.Target);
                return;
            }
            WriteString(writer, "msgid_plural", item.SourcePlural);
            WriteString(writer, "msgstr[0]", item.Target);
            var i = 0;
            foreach (var it in item.TargetPlural)
            {
                i++;
                WriteString(writer, $"msgstr[{i}]", it);
            }
        }
    }
}
