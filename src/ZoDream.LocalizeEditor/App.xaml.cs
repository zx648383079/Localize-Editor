using System.Windows;
using ZoDream.LocalizeEditor.Pages;
using ZoDream.LocalizeEditor.ViewModels;
using ZoDream.Shared.Routes;

namespace ZoDream.LocalizeEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            RegisterRoute();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length > 0)
            {
                await ViewModel.OpenPackageAsync(e.Args[0]);
                ShellManager.GoToAsync("home");
                return;
            }
            ShellManager.GoToAsync("startup");
        }

        private void RegisterRoute()
        {
            ShellManager.RegisterRoute("home", typeof(HomePage));
            ShellManager.RegisterRoute("startup", typeof(StartupPage));
            ShellManager.RegisterRoute("setting", typeof(SettingPage));
        }

        public static AppViewModel ViewModel { get; } = new();
    }
}
