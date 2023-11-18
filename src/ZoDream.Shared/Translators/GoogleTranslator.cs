using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class GoogleTranslator : ITranslator, IBrowserTranslator
    {

        public string Token { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;

        public string EntryURL => "https://translate.google.com/";

        public async Task<string> TranslateAsync(string sourceLang, string targetLang, 
            string text, CancellationToken token = default)
        {
            var items = await RequestAsync(sourceLang, targetLang, new string[] { text });
            return items.FirstOrDefault();
        }

        public async Task<LanguagePackage> TranslateAsync(string targetLang, 
            LanguagePackage package, CancellationToken token = default)
        {
            var items = await RequestAsync(package.Language, targetLang, package.Items.Select(i => i.Source));
            return new LanguagePackage(package.Language, targetLang)
            {
                Title = package.Title,
                Description = package.Description,
                MetaItems = package.MetaItems,
                Items = items.Select((text, i) => {
                    var arg = package.Items[i].Instance<UnitItem>();
                    arg.Target = text;
                    return arg;
                }).ToList()
            };
        }

        public async Task<IEnumerable<string>> RequestAsync(string from, string target, IEnumerable<string> items)
        {
            var uri = new Uri("https://translation.googleapis.com/language/translate/v2");
            using var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {Token}");
            request.Headers.TryAddWithoutValidation("x-goog-user-project", Project);
            request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            request.Content = new StringContent(JsonConvert.SerializeObject(new {
                q = string.Join("\n", items),
                source = from,
                target = target,
            }),
                Encoding.UTF8, "application/json");
            var res = await HttpHelper.RequestAsync(request);
            if (string.IsNullOrWhiteSpace(res))
            {
                return Array.Empty<string>();
            }
            var data = JsonConvert.DeserializeObject<GoogleTranslateObject>(res);
            if (data?.Data?.TransItems is null)
            {
                return Array.Empty<string>();
            }
            return data.Data.TransItems.Select(i => i.Text);
        }

        public string TranslateScript(string sourceLang, string targetLang, string text)
        {
            return "var input = document.querySelector('textarea');"
                + JavaScriptHelper.Value("input", text, true)
                + JavaScriptHelper.EmitBlur("input")
                + "var output = document.querySelector('.lRu31');"
                + "function trf(){ " +
                JavaScriptHelper.Callback("output")
                + "output.removeEventListener('change', trf); }"
                + "output.addEventListener('change', trf)";
        }

        public string GetScript()
        {
            return JavaScriptHelper.Callback("document.querySelector('.lRu31')");
        }

        private class GoogleTranslateObject
        {
            [JsonProperty("data")]
            public GoogleTranslate? Data { get; set; }
        }

        private class GoogleTranslate
        {
            [JsonProperty("translations")]
            public List<GoogleTranslateItem> TransItems { get; set; } = new();
        }

        private class GoogleTranslateItem
        {
            [JsonProperty("translatedText")]
            public string Text { get; set; } = string.Empty;

        }
    }
}
