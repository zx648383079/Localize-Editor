using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class YouDaoTranslator : ITranslator
    {

        public string AppKey { get; set; } = string.Empty;

        public string Secret { get; set; } = string.Empty;

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
            var uri = new Uri("https://openapi.youdao.com/v2/api");
            using var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            var q = string.Join("\n", items);
            var salt = new Random().Next(10000, 99999).ToString();
            var sign = HttpHelper.SHA256Encode($"{AppKey}{q}{salt}{Secret}");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"q", q},
                {"appKey", AppKey},
                { "salt", salt},
                { "from", from},
                { "to", target},
                { "sign", sign},
                { "signType", "v3" }
            });
            var res = await HttpHelper.RequestAsync(request);
            if (string.IsNullOrWhiteSpace(res))
            {
                return Array.Empty<string>();
            }
            var data = JsonConvert.DeserializeObject<YouDaoTranslate>(res);
            if (data is null)
            {
                return Array.Empty<string>();
            }
            return data.TransItems.Select(i => i.Text);
        }

        private class YouDaoTranslate
        {
            [JsonProperty("translateResults")]
            public List<YouDaoTranslateItem> TransItems { get; set; } = new();
        }

        private class YouDaoTranslateItem
        {
            [JsonProperty("translation")]
            public string Text { get; set; } = string.Empty;

        }
    }
}
