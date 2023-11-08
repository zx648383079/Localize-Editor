using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Translators
{
    public static class HttpHelper
    {

        public static string MD5Encode(string source) {
            var sor = Encoding.UTF8.GetBytes(source);
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(sor);
            var sb = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return sb.ToString();
        }

        public static string SHA256Encode(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var hash = SHA256.Create().ComputeHash(bytes);
            var builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static byte[] HMAC_SHA256(byte[] key, byte[] msg)
        {
            using var mac = new HMACSHA256(key);
            return mac.ComputeHash(msg);
        }

        public static int Timestamp(DateTime time)
        {
            var dateTimeStart = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return Convert.ToInt32((time.Ticks - dateTimeStart.Ticks) / 10000000);
        }
        public static async Task<string> RequestAsync(HttpRequestMessage request)
        {
            using var client = new HttpClient();
            using var response = await client.SendAsync(request);
            if (response == null || response.StatusCode != HttpStatusCode.OK)
            {
                return string.Empty;
            }
            using var stream = response.Content.Headers.ContentEncoding.Contains("gzip") ?
                new GZipStream(await response.Content.ReadAsStreamAsync(), mode: CompressionMode.Decompress)
                : await response.Content.ReadAsStreamAsync();
            using var ms = new MemoryStream();
            var buffer = new byte[1024];
            while (true)
            {
                if (stream == null) continue;
                var sz = stream.Read(buffer, 0, 1024);
                if (sz == 0) break;
                ms.Write(buffer, 0, sz);
            }
            var bytes = ms.ToArray();
            return ReadCharset(response).GetString(bytes);
        }

        private static Encoding ReadCharset(HttpResponseMessage response)
        {
            var items = response.Content.Headers.GetValues("Content-Type");
            foreach (var item in items)
            {
                var i = item.IndexOf("charset=");
                if (i < 0)
                {
                    continue;
                }
                return Encoding.GetEncoding(item.Substring(i + 7));
            }
            return Encoding.UTF8;
        }

        public static Uri BuildUri(string path, IDictionary<string, string>? queries = null)
        {
            var query = BuildQueries(queries);
            if (string.IsNullOrEmpty(query))
            {
                return new Uri(path);
            }
            if (path.Contains("?"))
            {
                return new Uri(path + "&" + query);
            }
            return new Uri(path + "?" + query);
        }

        private static string BuildQueries(IDictionary<string, string>? queries)
        {
            if (queries == null || !queries.Any()) return string.Empty;
            var builder = new StringBuilder();
            foreach (var content in queries)
            {
                if (builder.Length > 0)
                {
                    builder.Append("&");
                }
                builder.Append($"{WebUtility.UrlEncode(content.Key)}={WebUtility.UrlEncode(content.Value)}");
            }
            return builder.ToString();
        }
    }
}
