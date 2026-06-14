using Ubad.Views;

namespace Ubad
{
    public partial class AppShell : Shell
    {
        public AppShell(
            HomePage      home,
            ProjectsPage  projects,
            FavoritesPage favorites,
            SettingsPage  settings)
        {
            InitializeComponent();
            Routing.RegisterRoute("projectdetail", typeof(ProjectDetailPage));
            Routing.RegisterRoute("browser",       typeof(BrowserPage));
        }
    }
}
