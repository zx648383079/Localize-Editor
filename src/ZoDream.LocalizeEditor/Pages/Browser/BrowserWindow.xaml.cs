using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZoDream.Shared.Translators;

namespace ZoDream.LocalizeEditor.Pages
{
    /// <summary>
    /// BrowserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BrowserWindow : Window
    {
        public BrowserWindow()
        {
            InitializeComponent();
        }

        private readonly BrowserBridge Bridge = new();
        private IBrowserTranslator? Provider;
        private bool isLoading = false;

        public void NavigateUrl(string url)
        {
            Browser.Source = new Uri(url);
            IsLoading = true;
        }

        public async Task NavigateAsync(string url)
        {
            await Browser.EnsureCoreWebView2Async();
            Dispatcher.Invoke(() => {
                if (UrlTb.Text.StartsWith(url))
                {
                    return;
                }
                NavigateUrl(url);
            });
            await Task.Factory.StartNew(() => {
                while (IsLoading)
                {
                    Thread.Sleep(100);
                }
            });
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            var isCallback = script.Contains("zreBridger", StringComparison.CurrentCulture);
            await Browser.EnsureCoreWebView2Async();
            if (!isCallback)
            {
                return await Browser.ExecuteScriptAsync(script);
            }
            var completer = new TaskCompletionSource<string>();
            void func(string res)
            {
                completer.SetResult(res);
            }
            Bridge.ContentReady = func;
            var res = await Browser.ExecuteScriptAsync(script);
            return await completer.Task;
        }

        public bool IsLoading {
            get { return isLoading; }
            set {
                isLoading = value;
                StopBtn.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                RefreshBtn.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                ConfirmBtn.Visibility = Provider is not null && !isLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public IBrowserTranslator? Translator {
            set {
                Provider = value;
                ConfirmBtn.Visibility = value is not null && !isLoading ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl(UrlTb.Text);
        }

        private void UrlTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NavigateUrl(UrlTb.Text);
            }
        }


        private void Browser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Browser.Url = new Uri(((System.Windows.Forms.WebBrowser)sender).StatusText);
            e.Cancel = true;
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateUrl("https://www.baidu.com");
        }

        private void BeforeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
            {
                Browser.GoBack();
            }
        }

        private void ForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
            {
                Browser.GoForward();
            }
        }

        private void Browser_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            IsLoading = true;
        }

        private void Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            Title = Browser.CoreWebView2.DocumentTitle;
            BeforeBtn.Visibility = Browser.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
            ForwardBtn.Visibility = Browser.CanGoForward ? Visibility.Visible : Visibility.Collapsed;
            IsLoading = false;
        }

        private void Browser_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            var uri = Browser.Source;
            UrlTb.Text = uri.ToString();
        }

        private void Browser_CoreWebView2InitializationCompleted(object sender,
           CoreWebView2InitializationCompletedEventArgs e)
        {
            var coreWebView = Browser.CoreWebView2;
            coreWebView.NewWindowRequested += CoreWebView2_NewWindowRequested;
            coreWebView.DocumentTitleChanged += CoreWebView2_DocumentTitleChanged;
            coreWebView.AddHostObjectToScript("zreBridger", Bridge);
            coreWebView.AddScriptToExecuteOnDocumentCreatedAsync("var zreBridger = window.chrome.webview.hostObjects.zreBridger;");
        }

        private void CoreWebView2_DocumentTitleChanged(object? sender, object e)
        {
            Title = Browser.CoreWebView2.DocumentTitle;
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            Browser.Source = new Uri(e.Uri);
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            Browser.Stop();
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Provider is null)
            {
                return;
            }
            Browser.ExecuteScriptAsync(Provider.GetScript());
        }
    }
}
