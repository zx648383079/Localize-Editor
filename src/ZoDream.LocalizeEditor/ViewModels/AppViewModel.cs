using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Linq;
using ZoDream.LocalizeEditor.Pages;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Translators;

namespace ZoDream.LocalizeEditor.ViewModels
{
    public class AppViewModel : IDisposable
    {
        public const string FileFilters = ReaderFactory.FileFilters;

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

        private BrowserWindow? _translatorBrowser;

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
                var val = App.ResourceAssembly?.GetName()?.Version?.ToString();
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
        public Dispatcher DispatcherQueue => App.Current.Dispatcher;



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
            _translatorBrowser = null;
            Packages.Clear();
        }

        public void Exit()
        {
            _translatorBrowser?.Close();
            _baseWindow?.Close();
            App.Current.Shutdown();
        }

        public void AddPackage(LanguagePackage package)
        {
            CurrentPackage = package;
        }


        public BrowserWindow OpenBrowser()
        {
            if (_translatorBrowser is null)
            {
                _translatorBrowser = new BrowserWindow();
                _translatorBrowser.Closed += (s, _) => {
                    _translatorBrowser = null;
                };
            }
            _translatorBrowser.Show();
            return _translatorBrowser;
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
            if (!Option.UseBrowser || client is not IBrowserTranslator browserClient)
            {
                return await client.TranslateAsync(PackageSourceLanguage,
                PackageLanguage, text);
            }
            OpenBrowser();
            _translatorBrowser!.Translator = browserClient;
            await _translatorBrowser.NavigateAsync(browserClient.EntryURL);
            return await _translatorBrowser.ExecuteScriptAsync(browserClient.TranslateScript(PackageSourceLanguage,
                PackageLanguage, text));
        }


        public async Task<LanguagePackage?> TranslatePackageAsync(LanguagePackage package, 
            CancellationToken token = default)
        {
            var client = GetTranslator();
            if (client == null)
            {
                DispatcherQueue.Invoke(() => {
                    MessageBox.Show("翻译插件未配置");
                });
                return null;
            }
            if (!Option.UseBrowser || client is not IBrowserTranslator browserClient)
            {
                return await client.TranslateAsync(PackageSourceLanguage, package, token);
            }
            OpenBrowser();
            _translatorBrowser!.Translator = browserClient;
            await _translatorBrowser.NavigateAsync(browserClient.EntryURL);
            var translated = new Dictionary<string, string>();
            foreach (var item in package.Items)
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }
                if (!string.IsNullOrWhiteSpace(item.Target) || 
                    string.IsNullOrWhiteSpace(item.Source))
                {
                    continue;
                }
                var source = item.Source.Trim();
                if (translated.TryGetValue(source, out var target))
                {
                    item.Target = target;
                    continue;
                }
                item.Target = await _translatorBrowser.ExecuteScriptAsync(browserClient.TranslateScript(PackageSourceLanguage,
                PackageLanguage, source));
                translated.TryAdd(source, item.Target);
            }
            translated.Clear();
            return package;
        }


        public ITranslator? GetTranslator()
        {
            return TranslatorFactory.Translator(Option.Translator, Option.TranslatorData);
        }

        public static IReader? Reader(string name)
        {
            return ReaderFactory.Reader(name);
        }
    }
}
