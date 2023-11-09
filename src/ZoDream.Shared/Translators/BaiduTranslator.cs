using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class BaiduTranslator : ITranslator, IBrowserTranslator
    {

        public string AppId { get; set; } = string.Empty;

        public string Secret { get; set; } = string.Empty;

        public string EntryURL => "https://fanyi.baidu.com/";

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
            var uri = new Uri("http://api.fanyi.baidu.com/api/trans/vip/translate");
            using var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            var q = string.Join("\n", items);
            var salt = new Random().Next(10000, 99999).ToString();
            var sign = HttpHelper.MD5Encode($"{AppId}{q}{salt}{Secret}");
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"q", q},
                {"appid", AppId},
                { "salt", salt},
                { "from", from},
                { "to", target},
                { "sign", sign},
            });
            var res = await HttpHelper.RequestAsync(request);
            if (string.IsNullOrWhiteSpace(res))
            {
                return Array.Empty<string>();
            }
            var data = JsonConvert.DeserializeObject<BaiduTranslate>(res);
            if (data is null)
            {
                return Array.Empty<string>();
            }
            return data.TransItems.Select(i => i.Text);
        }

        public string TranslateScript(string sourceLang, string targetLang, string text)
        {
            return "var input = document.getElementById('baidu_translate_input');"
                 + JavaScriptHelper.Value("input", text, true)
                + JavaScriptHelper.Blur("input")
                + "var output = document.querySelector('.target-output');"
                + "function trf(){ " +
                JavaScriptHelper.Callback("output")
                + "output.removeEventListener('change', trf); }"
                + "output.addEventListener('change', trf)";
        }

        public string GetScript()
        {
            return JavaScriptHelper.Callback("document.querySelector('.target-output')");
        }

        private class BaiduTranslate
        {
            [JsonProperty("trans_result")]
            public List<BaiduTranslateItem> TransItems { get; set; } = new();
        }

        private class BaiduTranslateItem
        {
            [JsonProperty("dst")]
            public string Text { get; set; } = string.Empty;

        }
    }
}
