using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlutoFramework.Components.MessagePopup;
using PlutoFramework.Components.TransferView;
using PlutoFramework.Components.UniversalScannerView;
using PlutoFramework.Components.Vault;
using PlutoFramework.Model;
using Plutonication;

namespace AppTemplate.Pages
{
    public partial class MainPageViewModel : ObservableObject
    {
        [RelayCommand]
        public Task QrAsync() => Shell.Current.Navigation.PushAsync(new UniversalScannerPage { OnScannedMethod = OnScanned });

        [RelayCommand]
        public Task Settings() => Shell.Current.Navigation.PushAsync(new SettingsPage());

        [ObservableProperty]
        private bool isRefreshing = false;

        [RelayCommand]
        public async Task RefreshAsync()
        {
            IsRefreshing = true;

            _ = SubstrateClientModel.ChangeConnectedClientsAsync(EndpointsModel.GetSelectedEndpointKeys(), CancellationToken.None);

            await Task.Delay(5000);

            IsRefreshing = false;
        }


        public static void OnScanned(object? sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
#pragma warning disable VSTHRD101 // Avoid unsupported async delegates
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (e.Results.Length <= 0)
                {
                    return;
                }

                try
                {
                    var scannedValue = e.Results[0].Value;

                    // trying to connect to a dApp
                    if (scannedValue.Length > 14 && scannedValue.Substring(0, 14) == "plutonication:")
                    {
                        AccessCredentials ac = new AccessCredentials(new Uri(scannedValue));

                        PlutonicationModel.ProcessAccessCredentials(ac);
                    }
                    else if (scannedValue.Length > 13 && scannedValue.Substring(0, 13) == "plutolayout: ")
                    {
                        // LATER: check validity

                        CustomLayoutModel.SaveLayout(scannedValue);
                    }
                    else if (scannedValue.Length > 10 && scannedValue.Substring(0, 10) == "substrate:")
                    {
                        var viewModel = DependencyService.Get<TransferViewModel>();

                        viewModel.GetFeeAsync();

                        viewModel.IsVisible = true;

                        var scannedAddress = e.Results[e.Results.Length - 1].Value;

                        if (scannedAddress.Substring(10).IndexOf(":") != -1)
                        {
                            viewModel.Address = scannedAddress.Substring(10, scannedAddress.Substring(10).IndexOf(":"));
                        }
                        else
                        {
                            viewModel.Address = scannedAddress.Substring(10);
                        }
                    }
                    else if (Substrate.NetApi.Utils.Bytes2HexString(e.Results[0].Raw).IndexOf("530102") != -1)
                    {
                        var vaultSign = DependencyService.Get<VaultSignViewModel>();

                        await vaultSign.SignExtrinsicAsync("0x" + Substrate.NetApi.Utils.Bytes2HexString(e.Results[0].Raw).Substring(Substrate.NetApi.Utils.Bytes2HexString(e.Results[0].Raw).IndexOf("530102") + 6));
                    }
                    else
                    {
                        var messagePopup = DependencyService.Get<MessagePopupViewModel>();

                        messagePopup.Title = "Unable to read QR code";
                        messagePopup.Text = "The QR code was in incorrect format.";

                        messagePopup.IsVisible = true;
                    }

                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                catch (Exception ex)
                {

                    // Does not make much sense now...
                    return;

                    var messagePopup = DependencyService.Get<MessagePopupViewModel>();

                    messagePopup.Title = "BasePage Error";
                    messagePopup.Text = ex.Message;

                    messagePopup.IsVisible = true;
                }
            });
#pragma warning restore VSTHRD101 // Avoid unsupported async delegates
        }
    }
}
