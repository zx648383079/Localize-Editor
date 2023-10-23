using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ZoDream.Shared.Models;
using ZoDream.Shared.Readers;
using ZoDream.Shared.Storage;

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

        public readonly Dictionary<string, LanguagePackage> Packages = new();

        public string PackageLanguage { get; set; } = string.Empty;

        public LanguagePackage? CurrentPackage 
        {
            get {
                if (Packages.TryGetValue(PackageLanguage, out var package))
                {
                    return package;
                }
                return null;
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
            var reader = Reader(Path.GetExtension(path));
            if (reader is null)
            {
                return false;
            }
            var package = await reader.ReadAsync(path);
            package.FileName = path;
            PackageLanguage = package.TargetLanguage?.Code ?? "en";
            Packages.Add(PackageLanguage, package);
            return true;
        }

        public Task<bool> CreatePackageAsync()
        {
            var package = new LanguagePackage("en", "en");
            PackageLanguage = package.TargetLanguage!.Code;
            Packages.Add(PackageLanguage, package);
            return Task.FromResult(true);
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
            PackageLanguage = package.TargetLanguage?.Code ?? "new";
            Packages.Add(PackageLanguage, package);
            
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
