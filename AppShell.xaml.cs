using Ubad.Views;

namespace Ubad
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register detail routes
            Routing.RegisterRoute("projectdetail", typeof(ProjectDetailPage));
            Routing.RegisterRoute("browser",       typeof(BrowserPage));
        }
    }
}