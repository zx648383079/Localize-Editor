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
using static System.Net.Mime.MediaTypeNames;

namespace ZoDream.Shared.Translators
{
    public class DeepLTranslator : ITranslator, IBrowserTranslator
    {

        public string AuthKey { get; set; } = string.Empty;

        public string EntryURL => "https://www.deepl.com/translator"; // "#zh/de/文字"

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

        public string TranslateScript(string sourceLang, string targetLang, string text)
        {
            return "var items = document.querySelectorAll('d-textarea');"
                + JavaScriptHelper.Value("items[0].firstChild", text)
                + JavaScriptHelper.Paste("items[0].firstChild", text)
                + "function trf(){ " +
                JavaScriptHelper.Callback("items[1].firstChild") +
                " }"
                + JavaScriptHelper.ListenChange("items[1].firstChild", "trf");
        }

        public string GetScript()
        {
            return JavaScriptHelper.Callback("document.querySelectorAll('d-textarea')[1].firstChild");
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
