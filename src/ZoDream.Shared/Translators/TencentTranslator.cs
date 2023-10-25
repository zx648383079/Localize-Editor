using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Extensions;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public class TencentTranslator : ITranslator
    {

        public string ProjectId { get; set; } = string.Empty;
        public string SecretId { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Region { get; set; } = "ap-beijing";

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

        public async Task<IEnumerable<string>> RequestAsync(string from, string target, IEnumerable<string> items)
        {
            var uri = new Uri("https://tmt.tencentcloudapi.com");
            using var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post
            };
            var now = DateTime.UtcNow;
            var timestamp = HttpHelper.Timestamp(now).ToString();

            request.Headers.TryAddWithoutValidation("X-TC-Action", "TextTranslateBatch");
            request.Headers.TryAddWithoutValidation("X-TC-Region", Region);
            request.Headers.TryAddWithoutValidation("X-TC-Timestamp", timestamp);
            request.Headers.TryAddWithoutValidation("X-TC-Version", "2018-03-21");
            //request.Headers.TryAddWithoutValidation("X-TC-Token", "2018-03-21");

            var form = JsonConvert.SerializeObject(new {
                SourceTextList = items,
                Source = from,
                Target = target,
                ProjectId
            });
            var date = now.ToString("yyyy-MM-dd");
            var service = uri.Host.Split(new char[] { '.' }, 2)[0];
            var signData = $"POST\n/\ncontent-type:application/json; charset=utf-8\nhost:${uri.Host}\ncontent-type;host\n${HttpHelper.SHA256Encode(form)}";

            signData = $"TC3-HMAC-SHA256\n${timestamp}\n${date}/${service}/tc3_request\n${HttpHelper.SHA256Encode(signData)}";
            
            var sign = Sign(signData, date, service);
            request.Headers.TryAddWithoutValidation("Authorization", 
                $"TC3-HMAC-SHA256 Credential=${SecretId}/${date}/${service}/tc3_request, SignedHeaders=content-type;host, Signature=${sign}");
            // request.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            request.Content = new StringContent(form, Encoding.UTF8, "application/json");
            var res = await HttpHelper.RequestAsync(request);
            if (string.IsNullOrWhiteSpace(res))
            {
                return Array.Empty<string>();
            }
            var data = JsonConvert.DeserializeObject<TencentTranslateObject>(res);
            if (data?.Response is null)
            {
                return Array.Empty<string>();
            }
            return data.Response.TransItems;
        }

        private string Sign(string signData, string date, string service)
        {
            var tc3SecretKey = Encoding.UTF8.GetBytes("TC3" + SecretKey);
            var secretDate = HmacSHA256(tc3SecretKey, Encoding.UTF8.GetBytes(date));
            var secretService = HmacSHA256(secretDate, Encoding.UTF8.GetBytes(service));
            var secretSigning = HmacSHA256(secretService, Encoding.UTF8.GetBytes("tc3_request"));
            var signatureBytes = HmacSHA256(secretSigning, Encoding.UTF8.GetBytes(signData));
            return BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();
        }

        private static byte[] HmacSHA256(byte[] key, byte[] msg)
        {
            using var mac = new HMACSHA256(key);
            return mac.ComputeHash(msg);
        }
        private class TencentTranslateObject
        {
            public TencentTranslate? Response { get; set; }
        }
        private class TencentTranslate
        {
            [JsonProperty("TargetTextList")]
            public List<string> TransItems { get; set; } = new();
        }
    }
}
