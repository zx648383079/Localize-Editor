using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class AzureTranslator : ITranslator
    {

        public string AuthKey { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public async Task<string> Translate(LangItem sourceLang, LangItem targetLang, string text)
        {
            var items = await RequestAsync(targetLang.Code, new string[] { text });
            return items.FirstOrDefault();
        }

        public async Task<LanguagePackage> Translate(LangItem targetLang, LanguagePackage package)
        {
            var items = await RequestAsync(targetLang.Code, package.Items.Select(i => i.Source));
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

        public async Task<IEnumerable<string>> RequestAsync(string target, IEnumerable<string> items)
        {
            var uri = HttpHelper.BuildUri("https://api.cognitive.microsofttranslator.com/translate", 
                new Dictionary<string, string>
                {
                    {"api-version", "3.0"},
                    { "to", target},
                    { "includeSentenceLength", "true" }
                });
            using var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            request.Headers.TryAddWithoutValidation("Ocp-Apim-Subscription-Key", AuthKey);
            request.Headers.TryAddWithoutValidation("OOcp-Apim-Subscription-Region", Region);
            request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            request.Content = new StringContent(JsonConvert.SerializeObject(
                items.Select(i => new AzureTranslateItem(i))), 
                Encoding.UTF8, "application/json");
            var res = await HttpHelper.RequestAsync(request);
            if (string.IsNullOrWhiteSpace(res))
            {
                return Array.Empty<string>();
            }
            var data = JsonConvert.DeserializeObject<List<AzureTranslate>>(res);
            if (data is null)
            {
                return Array.Empty<string>();
            }
            return data[0].Translations.Select(i => i.Text);
        }

        private class AzureTranslate
        {
            [JsonProperty("translations")]
            public List<AzureTranslateItem> Translations { get; set; } = new();
        }

        private class AzureTranslateItem
        {
            [JsonProperty("text")]
            public string Text { get; set; } = string.Empty;

            public AzureTranslateItem()
            {
                
            }

            public AzureTranslateItem(string text)
            {
                Text = text;
            }
        }
    }
}
