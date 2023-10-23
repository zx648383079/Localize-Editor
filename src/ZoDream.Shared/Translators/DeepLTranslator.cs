using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class DeepLTranslator : ITranslator
    {

        public string AuthKey { get; set; } = string.Empty;

        public async Task<string> Translate(LangItem sourceLang, LangItem targetLang, string text)
        {
            var items = await RequestAsync(sourceLang.Code, targetLang.Code, new string[] { text });
            return items.FirstOrDefault();
        }

        public async Task<LanguagePackage> Translate(LangItem targetLang, LanguagePackage package)
        {
            var items = await RequestAsync(package.Language.Code, targetLang.Code, package.Items.Select(i => i.Source));
            return new LanguagePackage(package.Language, targetLang)
            {
                Title = package.Title,
                Description = package.Description,
                MetaItems = package.MetaItems,
                Items = items.Select((text, i) => {
                    var arg = package.Items[i].Clone();
                    arg.Target = text;
                    return arg;
                }).ToList()
            };
        }

        public async Task<IEnumerable<string>> RequestAsync(string from, string target, IEnumerable<string> items)
        {
            var uri = new Uri("https://api-free.deepl.com/v2/translate");
            using var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            request.Headers.TryAddWithoutValidation("Authorization", $"DeepL-Auth-Key {AuthKey}");
            request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            request.Content = new StringContent(JsonConvert.SerializeObject(new {
                text = items,
                source_lang = from,
                target_lang = target,
            }),
                Encoding.UTF8, "application/json");
            var res = await HttpHelper.RequestAsync(request);
            if (string.IsNullOrWhiteSpace(res))
            {
                return Array.Empty<string>();
            }
            var data = JsonConvert.DeserializeObject<DeepLTranslate>(res);
            if (data is null)
            {
                return Array.Empty<string>();
            }
            return data.TransItems.Select(i => i.Text);
        }

        private class DeepLTranslate
        {
            [JsonProperty("translations")]
            public List<DeepLTranslateItem> TransItems { get; set; } = new();
        }

        private class DeepLTranslateItem
        {
            [JsonProperty("text")]
            public string Text { get; set; } = string.Empty;

        }
    }
}
