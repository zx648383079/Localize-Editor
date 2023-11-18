using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Translators
{
    public static class TranslatorFactory
    {
        public static InputItem[] NameItems = new InputItem[]{
            new("DeepL", "DeepL翻译"),
            new("Baidu", "百度翻译"),
            new("Tencent", "腾讯翻译"),
            new("YouDao", "有道翻译"),
            new("Azure", "必应翻译"),
            new("Google", "谷歌翻译"),
            new("Alibaba", "阿里巴巴翻译"),
        };

        public static InputItem[] RenderForm(string name, IDictionary<string, string>? option)
        {
            return name switch
            {
                "DeepL" => new InputItem[]
                {
                    new("AuthKey", option)
                },
                "Baidu" => new InputItem[]
                {
                    new("AppId", option),
                    new("Secret", option)
                },
                "Tencent" => new InputItem[]
                {
                    new("SecretId", option),
                    new("SecretKey", option),
                    new("Region", option),
                    new("ProjectId", option),
                },
                "YouDao" => new InputItem[]
                {
                    new("AppKey", option),
                    new("Secret", option)
                },
                "Azure" => new InputItem[]
                {
                    new("AuthKey", option),
                    new("Region", option)
                },
                "Google" => new InputItem[]
                {
                    new("Token", option),
                    new("Project", option)
                },
                "Alibaba" => new InputItem[]
                {
                    new("AccessKeyId", option),
                    new("AccessKeySecret", option)
                },
                _ => Array.Empty<InputItem>()
            };
        }

        public static ITranslator? Translator(string name, IDictionary<string, string> option)
        {
            return name switch
            {
                "DeepL" => new DeepLTranslator()
                {
                    AuthKey = option["AuthKey"]
                },
                "Baidu" => new BaiduTranslator()
                {
                    AppId = option["AppId"],
                    Secret = option["Secret"],
                },
                "Tencent" => new TencentTranslator()
                {
                    ProjectId = option["ProjectId"],
                    Region = option["Region"],
                    SecretId = option["SecretId"],
                    SecretKey = option["SecretKey"],
                },
                "YouDao" => new YouDaoTranslator()
                {
                    AppKey = option["AppKey"],
                    Secret = option["Secret"],
                },
                "Azure" => new AzureTranslator()
                {
                    AuthKey = option["AuthKey"],
                    Region = option["Region"],
                },
                "Google" => new GoogleTranslator()
                {
                    Token = option["Token"],
                    Project = option["Project"],
                },
                "Alibaba" => new AlibabaTranslator()
                {
                    AccessKeyId = option["AccessKeyId"],
                    AccessKeySecret = option["AccessKeySecret"],
                },
                _ => null,
            };
        }
    }
}
