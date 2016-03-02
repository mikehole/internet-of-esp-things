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
        public ObservableCollection<RegisteredDevice> devices 
        {
            get {
                return App.devices;
            }
        }

        public MainPageViewModel()
        {
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
                    _GotoDetailsPage = new RelayCommand<RegisteredDevice>((selectedDevice) =>
                    {
                        string deviceName = selectedDevice.GivenName;
                        NavigationService.Navigate(typeof(Views.DetailPage), deviceName);
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


        public void Refresh()
        {
            //TODO
        }
    }
    }

