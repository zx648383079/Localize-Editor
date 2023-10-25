using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Translators;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class AppViewModel : IDisposable
    {
        public const string FileFilters = "XLFF文件|*.xlf|JSON文件|*.json|RESW文件|*.resw|PO文件|*.po;*.mo|PHP文件|*.php|所有文件|*.*";

        public AppViewModel()
        {
            _ = LoadOptionAsync();
        }

        public AppOption Option { get; private set; } = new();
        public LanguageDictionary LangDictionary { get; private set; } = new();

        public readonly Dictionary<string, LanguagePackage> Packages = new();

        public string PackageLanguage { get; set; } = string.Empty;
        /// <summary>
        /// 获取当前项目的源语言
        /// </summary>
        public string PackageSourceLanguage => Packages.Values.First().Language ?? "en";

        public LanguagePackage? CurrentPackage 
        {
            get {
                if (Packages.TryGetValue(PackageLanguage, out var package))
                {
                    return package;
                }
                return null;
            }
            set {
                if (value == null)
                {
                    return;
                }
                PackageLanguage = value.TargetLanguage;
                if (Packages.ContainsKey(PackageLanguage))
                {
                    Packages[value.TargetLanguage] = value;
                } else
                {
                    Packages.Add(value.TargetLanguage, value);
                }
            }
        }

        private Window? _baseWindow;
        public Window BaseWindow {
            set {
                _baseWindow = value;
            }
        }

        public string Title {
            set {
                if (_baseWindow is null)
                {
                    return;
                }
                _baseWindow.Title = $"{value} - ZoDream LocalizeEditor";
            }
        }

        public string Version {
            get {
                var val = Application.ResourceAssembly?.GetName()?.Version?.ToString();
                return string.IsNullOrEmpty(val) ? "-" : val;
            }
        }

        private static bool? DesignMode = null;

        /// <summary>
        /// 判断是设计器还是程序运行
        /// </summary>
        public static bool IsDesignMode {
            get {
                if (null == DesignMode)
                {
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    DesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

                    if (!DesignMode.GetValueOrDefault(false)
                        && Process.GetCurrentProcess().ProcessName.StartsWith("devenv", StringComparison.Ordinal))
                    {
                        DesignMode = true;
                    }
                }

                return DesignMode.GetValueOrDefault(false);
            }
        }


        /// <summary>
        /// UI线程.
        /// </summary>
        public Dispatcher DispatcherQueue => Application.Current.Dispatcher;



        public async Task<AppOption> LoadOptionAsync()
        {
            LangDictionary = await LanguageFile.LoadAsync("languages.txt");
            var option = await AppData.LoadAsync<AppOption>();
            if (option != null)
            {
                Option = option;
            }
            return Option;
        }

        public async Task SaveOptionAsync()
        {
            await AppData.SaveAsync(Option);
        }

        public async Task<bool> OpenPackageAsync(string path)
        {
            var package = await LoadPackageAsync(path);
            if (package is null)
            {
                return false;
            }
            package.FileName = path;
            //if (string.IsNullOrWhiteSpace(package.TargetLanguage))
            //{
            //    package.TargetLanguage = "en";
            //}
            CurrentPackage = package;
            return true;
        }

        public Task<bool> CreatePackageAsync(string sourceLang, string targetLang)
        {
            CurrentPackage = new LanguagePackage(sourceLang, targetLang);
            return Task.FromResult(true);
        }

        public async Task<LanguagePackage?> LoadPackageAsync(string path)
        {
            var reader = Reader(Path.GetExtension(path));
            if (reader is null)
            {
                return null;
            }
            var package = await reader.ReadAsync(path);
            package.FileName = path;
            return package;
        }

        public void Dispose()
        {
            _baseWindow = null;
            Packages.Clear();
        }

        public void Exit()
        {
            _baseWindow?.Close();
            Application.Current.Shutdown();
        }

        public void AddPackage(LanguagePackage package)
        {
            CurrentPackage = package;
        }

        public async Task<string> TranslateAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            var client = GetTranslator();
            if (client is null)
            {
                return string.Empty;
            }
            var package = CurrentPackage;
            if (package is null)
            {
                return string.Empty;
            }
            return await client.Translate(PackageSourceLanguage,
                PackageLanguage, text);
        }


        public ITranslator? GetTranslator()
        {
            return Option.Translator switch
            {
                "DeepL" => new DeepLTranslator()
                {
                    AuthKey = Option.TranslatorData["AuthKey"]
                },
                "Baidu" => new BaiduTranslator()
                {
                    AppId = Option.TranslatorData["AppId"],
                    Secret = Option.TranslatorData["Secret"],
                },
                "Tencent" => new TencentTranslator()
                {
                    ProjectId = Option.TranslatorData["ProjectId"],
                    Region = Option.TranslatorData["Region"],
                    SecretId = Option.TranslatorData["SecretId"],
                    SecretKey = Option.TranslatorData["SecretKey"],
                },
                "YouDao" => new YouDaoTranslator()
                {
                    AppKey = Option.TranslatorData["AppKey"],
                    Secret = Option.TranslatorData["Secret"],
                },
                "Azure" => new AzureTranslator()
                {
                    AuthKey = Option.TranslatorData["AuthKey"],
                    Region = Option.TranslatorData["Region"],
                },
                "Google" => new GoogleTranslator()
                {
                    Token = Option.TranslatorData["Token"],
                    Project = Option.TranslatorData["Project"],
                },
                _ => null,
            };
        }

        public static IReader? Reader(string name)
        {
            if (name[0] == '.')
            {
                name = name[1..];
            }
            return name.ToLower() switch
            {
                "xlf" => new Shared.Readers.Angular.XlfReader(),
                "mo" => new Shared.Readers.WordPress.MoReader(),
                "po" => new Shared.Readers.WordPress.PoReader(),
                "resx" => new Shared.Readers.CSharp.ResXReader(),
                "php" => new Shared.Readers.WordPress.PhpReader(),
                "json" => new Shared.Readers.Angular.JsonReader(),
                _ => null,
            };
        }
   
    }
}
