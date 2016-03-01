using Template10.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Ioespt.UWP.DeviceControl.Models;
using Ioespt.UWP.DeviceControl.Services.DataServices;
using GalaSoft.MvvmLight.Command;
using Windows.Devices.WiFi;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using Ioespt.UWP.Devices;
using Ioespt.UWP.DeviceControl.Services.DeviceDiscovery;
using Windows.UI.Xaml;

namespace Ioespt.UWP.DeviceControl.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<RegisteredDevice> devices { get; set; }

        SSDPClient ssdpClient = null;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private WiFiAdapter firstAdapter = null;

        public MainPageViewModel()
        {
            devices = new ObservableCollection<RegisteredDevice>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                #region Design Time Info

                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Slave", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                //    GivenName = "Lamp 1",
                //    Status = DeviceStatus.Offline,
                //    ConnectedTo = "Adafruit IO (MQTT)"
                //});
                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Master", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                //    GivenName = "Socket 1",
                //    Status = DeviceStatus.Online,
                //    ConnectedTo = "Amazon AWS"
                //});
                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Slave", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-12" },
                //    GivenName = "Weather Station",
                //    Status = DeviceStatus.Online,
                //    ConnectedTo = "Azure IoT Hub"
                //});

                //devices.Add(new RegisteredDevice()
                //{
                //    details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Master", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                //    GivenName = "Doorbell",
                //    Status = DeviceStatus.UnProvisioned,
                //    ConnectedTo = "N/A"
                //});

                #endregion /Design Time Info
            }
            else
            {
                DataService db = new DataService();

                db.createDB();

                foreach (var device in db.DevicesTable)
                {
                    devices.Add(device);
                }

                ssdpClient = new SSDPClient();

                ssdpClient.DeviceFound += SsdpClinet_DeviceFound;

                ssdpClient.SearchForDevices();
                SearchWifi();

                dispatcherTimer.Tick += DispatcherTimer_Tick;

                dispatcherTimer.Interval = new TimeSpan(0, 2, 0);

            }
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            ssdpClient.SearchForDevices();
            SearchWifi();
        }

        private void SsdpClinet_DeviceFound(object sender, DeviceFoundEventArgs e)
        {
            if(e.Device.DeviceType.manufacturer == "IOESPT")
            {
                var foundDevice = devices.FirstOrDefault(D => D.ChipId == e.Device.DeviceType.serialNumber);

                if(foundDevice != null)
                {
                    Dispatcher.Dispatch(() => {
                        foundDevice.Status = DeviceStatus.Online;
                    });
                }
            }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {

            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;

            return Task.CompletedTask;
        }


        private RelayCommand<RegisteredDevice> _GotoDetailsPage;
        public RelayCommand<RegisteredDevice> GotoDetailsPage
        {
            get
            {
                if (_GotoDetailsPage == null)
                {
                    _GotoDetailsPage = new RelayCommand<RegisteredDevice>(
                        (selectedDevice) =>
                    {
                        NavigationService.Navigate(typeof(Views.DetailPage), selectedDevice);
                    });
                }

                return _GotoDetailsPage;
            }
        }

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

        public async void SearchWifi()
        {
            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

            if (result.Count >= 1)
            {

                firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);

                await firstAdapter.ScanAsync();

                //firstAdapter.AvailableNetworksChanged += FirstAdapter_AvailableNetworksChanged;

                var qualifyingWifi = firstAdapter.NetworkReport.AvailableNetworks.Where(N => N.Ssid.ToLower().StartsWith("ioespt-thing"));


                foreach (WiFiAvailableNetwork deviceWifi in qualifyingWifi)
                {
                    RegisteredDevice newRegisteredDevice = new RegisteredDevice()
                    {
                        Status = DeviceStatus.UnProvisioned,
                        ConnectedTo = "None",
                        GivenName = deviceWifi.Ssid,
                        ChipId = "Unknown",
                        FirmwareName = "Unknown",
                        FirmwareVersion = "Unknown",
                        ModuleType = "Unknown"
                    };

                    devices.Add(newRegisteredDevice);
                }
            }
            else
            {

            }
        }

        //private void FirstAdapter_AvailableNetworksChanged(WiFiAdapter sender, object args)
        //{
        //    var qualifyingWifi = firstAdapter.NetworkReport.AvailableNetworks.Where(N => N.Ssid.ToLower().StartsWith("ioespt-thing"));

        //    foreach (WiFiAvailableNetwork deviceWifi in qualifyingWifi)
        //    {
        //        if (!devices.Any(D => D.GivenName.ToLower() == deviceWifi.Ssid.ToLower()))
        //        {
        //            RegisteredDevice newRegisteredDevice = new RegisteredDevice()
        //            {
        //                Status = DeviceStatus.UnProvisioned,
        //                ConnectedTo = "None",
        //                GivenName = deviceWifi.Ssid,
        //                ChipId = "Unknown",
        //                FirmwareName = "Unknown",
        //                FirmwareVersion = "Unknown",
        //                ModuleType = "Unknown"
        //            };


        //            Dispatcher.Dispatch(()=>{
        //                devices.Add(newRegisteredDevice);
        //            });
                    
        //        }
        //    }
        // }

        public void Refresh()
        {
            ssdpClient.SearchForDevices();
            SearchWifi();
        }
    }
    }

