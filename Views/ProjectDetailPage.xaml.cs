using Ubad.ViewModels;

namespace Ubad.Views
{
    public partial class ProjectDetailPage : ContentPage
    {
        public ProjectDetailPage(ProjectDetailViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}