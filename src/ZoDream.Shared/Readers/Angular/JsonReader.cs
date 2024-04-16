using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.Angular
{
    public class JsonReader : IReader
    {
        public async Task<IList<LanguagePackage>> ReadAsync(string file)
        {
            return [await ReadFileAsync(file)];
        }

        public async Task<LanguagePackage> ReadFileAsync(string file)
        {
            var package = new LanguagePackage("en");
            var content = await LocationStorage.ReadAsync(file);
            var data = JsonConvert.DeserializeObject<JObject>(content);
            if (data == null )
            {
                return package;
            }
            foreach (var item in data)
            {
                package.Items.Add(new(item.Key, item.Value?.ToString() ?? string.Empty)
                {
                    Id = item.Key,
                });
            }
            return package;
        }

        public async Task WriteAsync(string file, LanguagePackage package)
        {
            var data = new JObject();
            foreach (var item in package.Items)
            {
                data.Add(string.IsNullOrWhiteSpace(item.Id) ? item.Source : item.Id, item.Target);
            }
            await LocationStorage.WriteAsync(ReaderFactory.RenderFileName(file, package), 
                JsonConvert.SerializeObject(data));
        }

        public async Task WriteAsync(string file, IEnumerable<LanguagePackage> items)
        {
            foreach (var item in items)
            {
                await WriteAsync(file, item);
            }
        }
    }
}
