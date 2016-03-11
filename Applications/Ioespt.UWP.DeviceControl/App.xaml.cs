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
using Ioespt.UWP.DeviceControl.Models.DeviceTypes;

namespace Ioespt.UWP.DeviceControl
{
    // documentation on the bootstrapper
    // https://github.com/Windows-XAML/Template10/wiki/Docs-%7C-Bootstrapper

    sealed partial class App : Template10.Common.BootStrapper
    {

        private static ObservableCollection<IDevice> _devices = null;

        SSDPClient ssdpClient = null;
        WiFiClient wifiClient = null;

        DispatcherTimer searchTimer = new DispatcherTimer();

        DataService db = new DataService();


        public static ObservableCollection<IDevice> devices {
            get
            {
                if(_devices == null)
                    _devices = new ObservableCollection<IDevice>();

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
                IDevice dev = DeviceTypeFactory.MakeDevice(device.FirmwareName, device);

                dev.DeviceDetails.Status = DeviceStatus.Missing;

                devices.Add(dev);
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
                    devices.Add(DeviceTypeFactory.MakeDevice(e.Device.FirmwareName, e.Device));
                    db.InsertNewDevice(e.Device);
                }
                else
                {
                    device.DeviceDetails.Status = DeviceStatus.UnProvisioned;
                }
            });
        }

        private async void SsdpClient_DeviceFound(object sender, DeviceFoundEventArgs e)
        {
            if (e.Device.DeviceType.manufacturer == "IOESPT")
            {
                var foundDevice = devices.FirstOrDefault(D => D.DeviceDetails.ChipId == e.Device.DeviceType.serialNumber);

                Uri uri = new Uri(e.Device.URLBase);


                if (foundDevice != null)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => {
                        foundDevice.DeviceDetails.Status = DeviceStatus.Online;

                        foundDevice.DeviceDetails.Ip = uri.Host;
                    });
                }
                else
                {
                    //We have a new device online
                    var newDevice = new RegisteredDevice();

                    newDevice.ChipId = e.Device.DeviceType.serialNumber;
                    newDevice.FirmwareName = e.Device.DeviceType.modelName;
                    newDevice.ModuleType = e.Device.DeviceType.modelName;
                    newDevice.FirmwareVersion = e.Device.DeviceType.modelNumber;
                    newDevice.GivenName = e.Device.DeviceType.friendlyName;
                    newDevice.Status = DeviceStatus.Online;
                    newDevice.ConnectedTo = "None";
                    newDevice.Ip = uri.Host;

                    db.InsertNewDevice(newDevice);

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => {
                        devices.Add(DeviceTypeFactory.MakeDevice(newDevice.FirmwareName, newDevice));
                    });


                }
            }
        }
    }
}

