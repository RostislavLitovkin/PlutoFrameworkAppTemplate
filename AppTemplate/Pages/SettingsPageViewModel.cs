using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlutoFramework.Components.Account;
using PlutoFramework.Components.Credits;
using PlutoFramework.Components.Mnemonics;
using PlutoFramework.Components.Password;
using PlutoFramework.Components.Settings;
using PlutoFramework.Model;
using PlutoFramework.Model.SQLite;

namespace AppTemplate.Pages
{
    public partial class SettingsPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool hasAccount;
        public SettingsPageViewModel()
        {
            hasAccount = KeysModel.HasSubstrateKey();
        }

        [RelayCommand]
        public async Task LogOutAsync()
        {
            // Authenticate before logging out
            var account = await KeysModel.GetAccountAsync();

            if (account is null)
            {
                return;
            }

            KeysModel.RemoveAccount();
            KeysModel.RemoveAccount("kilt1");

            SecureStorage.Default.Remove(PreferencesModel.PASSWORD);
            Preferences.Remove(PreferencesModel.BIOMETRICS_ENABLED);
            Preferences.Remove(PreferencesModel.SHOW_WELCOME_SCREEN);

            await SQLiteModel.DeleteAllDatabasesAsync();

            Application.Current.MainPage = new SetupPasswordPage { Navigation = App.NewMainPageNavigationAsync };
        }

        [RelayCommand]
        public Task DeveloperSettingsAsync() => Shell.Current.Navigation.PushAsync(new DeveloperSettingsPage());

        [RelayCommand]
        public async Task ShowMnemonicsAsync()
        {
            if (!KeysModel.HasSubstrateKey())
            {
                var noAccountPopupViewModel = DependencyService.Get<NoAccountPopupViewModel>();

                noAccountPopupViewModel.IsVisible = true;

                return;
            }

            try
            {
                var secret = await KeysModel.GetMnemonicsOrPrivateKeyAsync();

                await Shell.Current.Navigation.PushAsync(new MnemonicsPage(secret));
            }
            catch
            {
                // Failed to authenticate
            }
        }

        [RelayCommand]
        public Task CreditsAsync() => Shell.Current.Navigation.PushAsync(new CreditsPage());
        
    }
}
