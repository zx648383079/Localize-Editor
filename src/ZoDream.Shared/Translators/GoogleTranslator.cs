﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class GoogleTranslator : ITranslator
    {

        public string Token { get; set; } = string.Empty;
        public string Project { get; set; } = string.Empty;

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
