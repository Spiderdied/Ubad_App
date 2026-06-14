using Ubad.ViewModels;

namespace Ubad.Views
{
    public partial class ProjectsPage : ContentPage
    {
        private readonly ProjectsViewModel _vm;

        public ProjectsPage(ProjectsViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (_vm.FilteredRepos.Count == 0)
                await _vm.LoadAsync();
        }
    }
}