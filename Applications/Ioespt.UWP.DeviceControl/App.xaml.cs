using System;
using Windows.UI.Xaml;
using System.Threading.Tasks;
using Ioespt.UWP.DeviceControl.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Template10.Common;
using System.Collections.ObjectModel;
using Ioespt.UWP.DeviceControl.Models;
using Ioespt.UWP.DeviceControl.Services.DataServices;
using System.Linq;
using Ioespt.UWP.DeviceControl.Services.DeviceDiscovery;
using Windows.ApplicationModel.Core;

namespace Ioespt.UWP.DeviceControl
{
    // documentation on the bootstrapper
    // https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-Bootstrapper

    sealed partial class App : Template10.Common.BootStrapper
    {

        private static ObservableCollection<RegisteredDevice> _devices = null;

        SSDPClient ssdpClient = null;
        WiFiClient wifiClient = null;

        DispatcherTimer searchTimer = new DispatcherTimer();

        DataService db = new DataService();


        public static ObservableCollection<RegisteredDevice> devices {
            get
            {
                if(_devices == null)
                    _devices = new ObservableCollection<RegisteredDevice>();

                return _devices;
            }
        }

        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            ssdpClient = new SSDPClient();
            ssdpClient.DeviceFound += SsdpClient_DeviceFound;


            wifiClient = new WiFiClient();
            wifiClient.DeviceFound += WifiClient_DeviceFound;

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public override Task OnInitializeAsync(IActivatedEventArgs args)
        {
            // content may already be shell when resuming
            if ((Window.Current.Content as ModalDialog) == null)
            {
                // setup hamburger shell, wrapped in a modal dialog
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);
                Window.Current.Content = new ModalDialog
                {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }
            return Task.CompletedTask;
        }

        public override Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // long-running startup tasks go here
            db = new DataService();

            db.createDB();

            foreach (var device in db.DevicesTable)
            {
                device.Status = DeviceStatus.Missing;
                devices.Add(device);
            }

            NavigationService.Navigate(typeof(Views.MainPage));

            ssdpClient.Search();
            wifiClient.Search();

            searchTimer.Tick += SearchTimer_Tick;
            searchTimer.Interval = new TimeSpan(0, 1, 0);
            searchTimer.Start();


            return Task.CompletedTask;
        }


        ///////////////////////////////////////////
        //Methods to manage the devies collection

        private void SearchTimer_Tick(object sender, object e)
        {
            wifiClient.Search();
            ssdpClient.Search();
        }

        private async void WifiClient_DeviceFound(object sender, WiFiClient.DeviceFoundEventArgs e)
        {
            var device = devices.FirstOrDefault(D => D.GivenName == e.Device.GivenName);

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {

                if (device == null)
                {
                    devices.Add(e.Device);
                    db.InsertNewDevice(e.Device);
                }
                else
                {
                    device.Status = DeviceStatus.UnProvisioned;
                }
            });
        }

        private async void SsdpClient_DeviceFound(object sender, DeviceFoundEventArgs e)
        {
            if (e.Device.DeviceType.manufacturer == "IOESPT")
            {
                var foundDevice = devices.FirstOrDefault(D => D.ChipId == e.Device.DeviceType.serialNumber);

                if (foundDevice != null)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => {
                        foundDevice.Status = DeviceStatus.Online;
                    });
                }
            }
        }
    }
}

