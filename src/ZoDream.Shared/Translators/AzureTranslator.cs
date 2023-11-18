using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class AzureTranslator : ITranslator, IBrowserTranslator
    {

        public string AuthKey { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string EntryURL => "https://cn.bing.com/translator";//?from=en&to=zh-Hans";

        public async Task<string> TranslateAsync(string sourceLang, 
            string targetLang, string text, CancellationToken token = default)
        {
            var items = await RequestAsync(targetLang, new string[] { text });
            return items.FirstOrDefault();
        }

        public async Task<LanguagePackage> TranslateAsync(string targetLang, 
            LanguagePackage package, CancellationToken token = default)
        {
            var items = await RequestAsync(targetLang, package.Items.Select(i => i.Source));
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

        public string TranslateScript(string sourceLang, string targetLang, string text)
        {
            return "var input = document.getElementById('tta_input_ta');"
                 + JavaScriptHelper.Value("input", text, true)
                + JavaScriptHelper.EmitInput("input")
                + "var output = document.getElementById('tta_output_ta');"
                + "function trf(){ " +
                JavaScriptHelper.Callback("output", true)
                + "}"
                + JavaScriptHelper.LoopChange("output", "trf");
        }

        public string GetScript()
        {
            return JavaScriptHelper.Callback("document.getElementById('tta_output_ta')", true);
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
