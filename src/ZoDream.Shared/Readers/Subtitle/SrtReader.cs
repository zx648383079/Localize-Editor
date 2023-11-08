using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.Subtitle
{
    public class SrtReader : IReader
    {
        public Task<LanguagePackage> ReadAsync(string file)
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
                if (!line.Contains("-->"))
                {
                    continue;
                }
                package.Items.Add(new UnitItem(ReadContent(reader), string.Empty)
                {
                    Id = line,
                });
            }
            return package;
        }

        private string ReadContent(StreamReader reader)
        {
            var content = string.Empty;
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null || string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                if (!string.IsNullOrEmpty(content))
                {
                    content += "\n";
                }
                content += line;
            }
            return content;
        }

        public Task WriteAsync(string file, LanguagePackage package)
        {
            return Task.Factory.StartNew(() => 
            {
                Write(file, package);
            });
        }

        public void Write(string file, LanguagePackage package)
        {
            var writer = LocationStorage.Writer(file);
            var i = 0;
            foreach (var item in package.Items)
            {
                writer.WriteLine(++i);
                writer.WriteLine(item.Id);
                foreach(var line in item.Target.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    writer.WriteLine(line);
                }
                writer.WriteLine();
            }
        }
    }
}
