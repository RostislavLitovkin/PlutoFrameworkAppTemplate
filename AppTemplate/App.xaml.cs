using PlutoFramework.Components.Account;
using PlutoFramework.Components.Password;
using PlutoFramework.Model;

namespace AppTemplate
{
    public partial class App : Application
    {
        public static Task NewMainPageNavigationAsync()
        {
            App.Current.MainPage = new AppShell();
            return Task.FromResult(0);
        }

        public async Task GenerateNewAccountAsync()
        {
            await KeysModel.GenerateNewAccountAsync();

            App.Current.MainPage = new AppShell();
        }

        public App()
        {
            var noAccountViewModel = DependencyService.Get<NoAccountPopupViewModel>();
            noAccountViewModel.AfterCreateAccountNavigation = NewMainPageNavigationAsync;

            InitializeComponent();

            if (!KeysModel.HasSubstrateKey())
            {
                MainPage = new SetupPasswordPage
                {
                    Navigation = GenerateNewAccountAsync
                };
            }
            else
            {
                MainPage = new AppShell();
            }
        }
    }
}