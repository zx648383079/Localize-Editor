using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers.Angular
{
    public class JsonReader : IReader
    {
        public async Task<LanguagePackage> ReadAsync(string file)
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
                package.Items.Add(new()
                {
                    Source = item.Key,
                    Target = item.Value?.ToString() ?? string.Empty,
                });
            }
            return package;
        }

        public async Task WriteAsync(string file, LanguagePackage package)
        {
            var data = new JObject();
            foreach (var item in package.Items)
            {
                data.Add(item.Source, item.Target);
            }
            await LocationStorage.WriteAsync(file, JsonConvert.SerializeObject(data));
        }
    }
}
