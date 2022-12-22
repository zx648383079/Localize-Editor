using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.Angular
{
    public class XlfReader : IReader
    {
        const string TAB = "    ";

        public async Task<LanguagePackage> ReadAsync(string file)
        {
            var reader = new CharReader(await LocationStorage.ReadAsync(file));
            var package = new LanguagePackage("en");
            while (reader.MoveNext())
            {
                var code = reader.Current;
                if (code != '<')
                {
                    continue;
                }
                var node = GetNode(reader);
                if (node == "file")
                {
                    var data = GetAttribute(reader);
                    if (data.ContainsKey("source-language"))
                    {
                        package.Language = new LangItem(data["source-language"]);
                    }
                    if (data.ContainsKey("target-language"))
                    {
                        package.TargetLanguage = new LangItem(data["target-language"]);
                    }
                    MoveNodeEnd(reader);
                    continue;
                }
                if (node == "trans-unit")
                {
                    package.Items.Add(GetUnit(reader));
                    continue;
                }
                MoveNodeEnd(reader);
            }
            return package;
        }

        private UnitItem GetUnit(CharReader reader)
        {
            var data = new UnitItem();
            var attrs = GetAttribute(reader);
            if (attrs.ContainsKey("id"))
            {
                data.Id = attrs["id"];
            }
            MoveNodeEnd(reader);
            while (reader.MoveNext())
            {
                var code = reader.Current;
                if (code != '<')
                {
                    continue;
                }
                var node = GetNode(reader);
                if (node == "source")
                {
                    data.Source = GetNodeValue(reader, "source");
                    continue;
                }
                if (node == "target")
                {
                    data.Target = GetNodeValue(reader, "target");
                    continue;
                }
                if (node == "context-group")
                {
                    var attr1 = GetAttribute(reader);
                    MoveNodeEnd(reader);
                    if (attr1["purpose"] == "location")
                    {
                        data.AddLine(GetLocation(reader));
                    }
                    continue;
                }
                if (IsEndNode(reader, "trans-unit"))
                {
                    break;
                }
                MoveNodeEnd(reader);
            }
            return data;
        }

        private SourceLocation GetLocation(CharReader reader)
        {
            var data = new SourceLocation();
            while (reader.MoveNext())
            {
                var code = reader.Current;
                if (code != '<')
                {
                    continue;
                }
                var node = GetNode(reader);
                if (node == "context")
                {
                    var attrs = GetAttribute(reader);
                    var value = GetNodeValue(reader, "context");
                    if (attrs["context-type"] == "sourcefile")
                    {
                        data.FileName = value;
                    }
                    else if (attrs["context-type"] == "linenumber")
                    {
                        data.Add(value);
                    }
                    continue;
                }
                if (IsEndNode(reader, "context-group"))
                {
                    break;
                }
                MoveNodeEnd(reader);
            }
            return data;
        }

        private void MoveNodeEnd(CharReader reader)
        {
            if (reader.Current == '>')
            {
                return;
            }
            while (reader.MoveNext())
            {
                if (reader.Current == '>')
                {
                    break;
                }
            }
        }

        private bool IsEndNode(CharReader reader, string node)
        {
            if (reader.Current == '/' && reader.NextIs(node))
            {
                MoveNodeEnd(reader);
                return true;
            }
            return false;
        }

        private string GetNodeValue(CharReader reader, string node)
        {
            MoveNodeEnd(reader);
            var end = reader.IndexOf($"</{node}>");
            if (end < 0)
            {
                return string.Empty;
            }
            var value = reader.ReadSeek(reader.Position + 1, end - reader.Position - 1);
            reader.Position = end + node.Length;
            return value;
        }

        private Dictionary<string, string> GetAttribute(CharReader reader)
        {
            var data = new Dictionary<string, string>();
            var code = reader.Current;
            if (code == '/' || code == '>')
            {
                return data;
            }
            var sb = new StringBuilder();
            while (reader.MoveNext())
            {
                code = reader.Current;
                if (code == '/' || code == '>')
                {
                    break;
                }
                sb.Append(code);
            }
            var blocks = sb.ToString().Split(' ');
            foreach (var item in blocks)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    continue;
                }
                var i = item.IndexOf('=');
                if (i < 0)
                {
                    continue;
                }
                data.Add(item.Substring(0, i), item.Substring(i + 2, item.Length - i - 3));
            }
            return data;
        }

        private string GetNode(CharReader reader)
        {
            var sb = new StringBuilder();
            while (reader.MoveNext())
            {
                var code = reader.Current;
                if (code == '/' || code == '>' || code == ' ')
                {
                    break;
                }
                sb.Append(code);
            }
            return sb.ToString();
        }

        public async Task WriteAsync(string file, LanguagePackage package)
        {
            using var writer = LocationStorage.Writer(file);
            await WriteLineAsync(writer, "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            await WriteLineAsync(writer, "<xliff version=\"1.2\" xmlns=\"urn: oasis:names: tc:xliff: document:1.2\">");
            var targetLang = package.TargetLanguage == null ? string.Empty : $" target-language=\"{package.TargetLanguage.Code}\"";
            var lang = package.Language == null ? string.Empty : $" source-language=\"{package.Language.Code}\"";
            await WriteLineAsync(writer, 1, $"<file{lang}{targetLang} datatype=\"plaintext\" original=\"ng2.template\">");
            await WriteLineAsync(writer, 2, "<body>");
            foreach (var item in package.Items)
            {
                if (string.IsNullOrWhiteSpace(item.Id) && string.IsNullOrWhiteSpace(item.Source))
                {
                    continue;
                }
                await WriteLineAsync(writer, 3, "<trans-unit", string.IsNullOrWhiteSpace(item.Id) ? string.Empty : $"id=\"{item.Id}\"", "datatype=\"html\">");
                if (!string.IsNullOrWhiteSpace(item.Source))
                {
                    await WriteLineAsync(writer, 4, $"<source>{item.Source}</source>");
                }
                if (!string.IsNullOrWhiteSpace(item.Target))
                {
                    await WriteLineAsync(writer, 4, $"<target>{item.Target}</target>");
                }
                foreach (var loc in item.Location)
                {
                    await WriteLineAsync(writer, 4, "<context-group purpose=\"location\">");
                    await WriteLineAsync(writer, 5, $"<context context-type=\"sourcefile\">{loc.FileName}</context>");
                    await WriteLineAsync(writer, 5, $"<context context-type=\"linenumber\">{loc.LineNumberFormat}</context>");
                    await WriteLineAsync(writer, 4, $"</context-group>");
                }
                await WriteLineAsync(writer, 3, "</trans-unit>");
            }
            await WriteLineAsync(writer, 2, "</body>");
            await WriteLineAsync(writer, 1, "</file>");
            await WriteLineAsync(writer, "</xliff>");
        }

        private async Task WriteLineAsync(TextWriter writer, int level, params string[] blocks)
        {
            if (blocks.Length == 0)
            {
                return;
            }
            if (level < 1 && blocks.Length == 1)
            {
                await writer.WriteLineAsync(blocks[0]);
                return;
            }
            var sb = new StringBuilder();
            while (level > 0)
            {
                sb.Append(TAB);
                level--;
            }
            var i = 0;
            foreach (var block in blocks)
            {
                if (string.IsNullOrEmpty(block))
                {
                    continue;
                }
                if (i > 0)
                {
                    sb.Append(' ');
                }
                sb.Append(block);
                i++;
            }
            await writer.WriteLineAsync(sb.ToString());
        }

        private async Task WriteLineAsync(TextWriter writer, string line)
        {
            await WriteLineAsync(writer, 0, line);
        }
    }
}
