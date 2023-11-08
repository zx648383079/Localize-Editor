using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class AlibabaTranslator : ITranslator, IBrowserTranslator
    {

        public string AccessKeyId { get; set; } = string.Empty;
        public string AccessKeySecret { get; set; } = string.Empty;

        public string EntryURL => "https://translate.alibaba.com/";

        public async Task<string> Translate(string sourceLang, string targetLang, string text)
        {
            var items = await RequestAsync(sourceLang, targetLang, new string[] { text });
            return items.FirstOrDefault();
        }

        public async Task<LanguagePackage> Translate(string targetLang, LanguagePackage package)
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

        public async Task<IEnumerable<string>> RequestAsync(string source, string target, IEnumerable<string> items)
        {
            var uri = new Uri("http://mt.cn-hangzhou.aliyuncs.com/api/translate/web/ecommerce");
            using var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };

            var now = DateTime.UtcNow;

            var headers = new Dictionary<string, string>
            {
                {"x-acs-action", "TranslateGeneral" },
                {"x-acs-version", "2015-12-15" },
                {"x-acs-signature-nonce", HttpHelper.Timestamp(now).ToString() },
                {"x-acs-date", now.ToString() },
                {"Content-Type", "application/json" },
            };

            var form = JsonConvert.SerializeObject(
                new {
                    FormatType = "text",
                    SourceLanguage = source,
                    TargetLanguage = target,
                    SourceText = string.Join("\n", items),
                    Scene = "general",
                }
                );

            headers.Add("Authorization", Sign(form, uri, headers));

            foreach (var item in headers)
            {
                request.Headers.TryAddWithoutValidation(item.Key, item.Value);
            }
            
            request.Headers.TryAddWithoutValidation("accept", "application/json");
            request.Content = new StringContent(form, 
                Encoding.UTF8, "application/json");
            var res = await HttpHelper.RequestAsync(request);
            if (string.IsNullOrWhiteSpace(res))
            {
                return Array.Empty<string>();
            }
            var data = JsonConvert.DeserializeObject<AlibabaTranslateObject>(res);
            if (data?.Response?.Data?.Text is null)
            {
                return Array.Empty<string>();
            }
            return new string[] { data.Response.Data.Text };
        }

        private string Sign(string formData, Uri uri, IDictionary<string, string> headers)
        {
            var canonicalHeaders = string.Empty;
            var signedHeaders = string.Empty;
            foreach (var item in headers)
            {
                if (!string.IsNullOrEmpty(canonicalHeaders))
                {
                    canonicalHeaders += "\n";
                    signedHeaders += ";";
                }
                canonicalHeaders += item.Key.ToLower() + ":" + item.Value.Trim();
                signedHeaders += item.Key.ToLower();
            }
            var canonicalRequest = $"POST\n{uri.AbsolutePath}\n{uri.Query}\n{canonicalHeaders}\n{signedHeaders}\n" + HttpHelper.SHA256Encode(formData);
            var secretKey = Encoding.UTF8.GetBytes(AccessKeySecret);

            var signatureBytes = HttpHelper.HMAC_SHA256(secretKey, Encoding.UTF8.GetBytes(HttpHelper.SHA256Encode(canonicalRequest)));

            return $"ACS3-HMAC-SHA256 Credential={AccessKeyId},SignedHeaders=${signedHeaders},Signature=" + BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();
        }


        public string TranslateScript(string sourceLang, string targetLang, string text)
        {
            return "var input = document.getElementById('source');"
                + "input.value ='" + text + "';"
                + JavaScriptHelper.Blur("input")
                + "var output = document.querySelector('.target-translat').querySelector('pre');"
                + "function trf(){ " +
                JavaScriptHelper.Callback("output")
                + "output.removeEventListener('change', trf); }"
                + "output.addEventListener('change', trf)";
        }

        public string GetScript()
        {
            return JavaScriptHelper.Callback("document.querySelector('.target-translat').querySelector('pre')");
        }

        private class AlibabaTranslateObject
        {
            [JsonProperty("TranslateGeneralResponse")]
            public AlibabaTranslate? Response { get; set; }
        }

        private class AlibabaTranslate
        {
            public AlibabaTranslateItem? Data { get; set; }
        }

        private class AlibabaTranslateItem
        {
            [JsonProperty("Translated")]
            public string Text { get; set; } = string.Empty;

            public AlibabaTranslateItem()
            {
                
            }

            public AlibabaTranslateItem(string text)
            {
                Text = text;
            }
        }
    }
}
