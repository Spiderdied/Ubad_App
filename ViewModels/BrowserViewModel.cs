using System.Windows.Input;

namespace Ubad.ViewModels
{
    // ✅ IQueryAttributable بدلاً من [QueryProperty]
    public class BrowserViewModel : BaseViewModel, IQueryAttributable
    {
        private string _url = string.Empty;
        public string Url
        {
            get => _url;
            set => SetProperty(ref _url, value);
        }

        private string _pageTitle = "Loading…";
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        private double _loadingProgress;
        public double LoadingProgress
        {
            get => _loadingProgress;
            set => SetProperty(ref _loadingProgress, value);
        }

        private bool _canGoBack;
        public bool CanGoBack
        {
            get => _canGoBack;
            set => SetProperty(ref _canGoBack, value);
        }

        private bool _canGoForward;
        public bool CanGoForward
        {
            get => _canGoForward;
            set => SetProperty(ref _canGoForward, value);
        }

        // ✅ IQueryAttributable
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("Url", out var val) && val is string url
                && !string.IsNullOrWhiteSpace(url))
            {
                Url       = url;
                PageTitle = url;
            }
        }

        // WebView reference — يُحقن من code-behind
        public WebView? WebViewRef { get; set; }

        public ICommand GoBackCommand       { get; }
        public ICommand GoForwardCommand    { get; }
        public ICommand RefreshCommand      { get; }
        public ICommand ShareCommand        { get; }
        public ICommand OpenExternalCommand { get; }
        public ICommand CloseCommand        { get; }

        public BrowserViewModel()
        {
            GoBackCommand       = CreateCommand(GoBackAsync);
            GoForwardCommand    = CreateCommand(GoForwardAsync);
            RefreshCommand      = CreateCommand(RefreshAsync);
            ShareCommand        = CreateCommand(ShareAsync);
            OpenExternalCommand = CreateCommand(OpenExternalAsync);
            CloseCommand        = CreateCommand(NavigateBackAsync);
        }

        private Task GoBackAsync()
        {
            if (WebViewRef?.CanGoBack == true)
                WebViewRef.GoBack();
            return Task.CompletedTask;
        }

        private Task GoForwardAsync()
        {
            if (WebViewRef?.CanGoForward == true)
                WebViewRef.GoForward();
            return Task.CompletedTask;
        }

        private Task RefreshAsync()
        {
            WebViewRef?.Reload();
            return Task.CompletedTask;
        }

        private async Task ShareAsync() =>
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = PageTitle,
                Text  = Url
            });

        private async Task OpenExternalAsync() =>
            await Browser.OpenAsync(Url, BrowserLaunchMode.SystemPreferred);
    }
}
