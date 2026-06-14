using Ubad.ViewModels;

namespace Ubad.Views
{
    public partial class BrowserPage : ContentPage
    {
        private readonly BrowserViewModel _vm;

        public BrowserPage(BrowserViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
            _vm.WebViewRef = WebViewControl;
        }

        private void OnNavigating(object sender, WebNavigatingEventArgs e)
        {
            _vm.IsLoading      = true;
            _vm.LoadingProgress = 0.1;
        }

        private void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            _vm.IsLoading      = false;
            _vm.LoadingProgress = 1.0;
            _vm.CanGoBack       = WebViewControl.CanGoBack;
            _vm.CanGoForward    = WebViewControl.CanGoForward;

            // Try to extract page title via JS
            _ = TryGetTitleAsync();
        }

        private async Task TryGetTitleAsync()
        {
            try
            {
                var title = await WebViewControl.EvaluateJavaScriptAsync("document.title");
                if (!string.IsNullOrWhiteSpace(title))
                    _vm.PageTitle = title.Trim('"');
            }
            catch { /* ignore */ }
        }
    }
}