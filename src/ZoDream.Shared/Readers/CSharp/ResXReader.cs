using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers.CSharp
{
    public class ResXReader : IReader
    {
        public Task<LanguagePackage> ReadAsync(string file)
        {
            return Task.Factory.StartNew(() => 
            {
                return ReadFile(file);
            });
        }

        public LanguagePackage ReadFile(string file)
        {
            var package = new LanguagePackage("en");
            var doc = XDocument.Load(file);
            var root = doc.Root;
            if (root == null)
                return package;

            var ns = XNamespace.Get(string.Empty);
            var items = root
                .Elements(ns + "data")
                // Exclude non-string types
                .Where(x => x.Attribute("type") == null && x.Attribute("mimetype") == null)
                .ToList();

            foreach (var item in items)
            {
                var name = item.Attribute("name")?.Value;
                var value = item.Element("value")?.Value;
                var comment = item.Element("comment")?.Value;

                if (name == null || value == null)
                {
                    continue;
                }

                if (name.StartsWith(">>"))
                {
                    continue;
                }
                package.Items.Add(new UnitItem()
                {
                    Id = name,
                    Target = value,
                    Comment = comment ?? string.Empty
                });
            }

            return package;
        }

        public Task WriteAsync(string file, LanguagePackage package)
        {
            return Task.Factory.StartNew(() => 
            {
                WriteFile(file, package);
            });
        }

        public void WriteFile(string file, LanguagePackage package)
        {
            var doc = new XDocument();
            var root = doc.Root;
            var ns = XNamespace.Get(string.Empty);
            foreach (var item in package.Items)
            {
                var node = new XElement(ns + "data")
                {
                    Value = item.Target,
                };
                node.Add(new XAttribute("name", item.Id));
                if (!string.IsNullOrWhiteSpace(item.Comment))
                {
                    node.Add(new XElement("comment", item.Comment));
                }
                root.Add(node);
            }
            doc.Save(file);
        }
    }
}
