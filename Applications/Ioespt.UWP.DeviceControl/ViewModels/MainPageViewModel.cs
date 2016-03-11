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
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using Ioespt.UWP.DeviceControl.Models.DeviceTypes;

namespace Ioespt.UWP.DeviceControl.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<IDevice> devices 
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

            TestSerial();

        }

        private async void TestSerial()
        {
            var aqsFilter = SerialDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(aqsFilter);
            if (devices.Any())
            {
                //var deviceId = devices.First().Id;
                //this.device = await SerialDevice.FromIdAsync(deviceId);

                //if (this.device != null)
                //{
                //    this.device.BaudRate = 57600;
                //    this.device.StopBits = SerialStopBitCount.One;
                //    this.device.DataBits = 8;
                //    this.device.Parity = SerialParity.None;
                //    this.device.Handshake = SerialHandshake.None;

                //    this.reader = new DataReader(this.device.InputStream);
                //}
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


        private RelayCommand<IDevice> _GotoDetailsPage;
        public RelayCommand<IDevice> GotoDetailsPage
        {
            get
            {
                if (_GotoDetailsPage == null)
                {
                    _GotoDetailsPage = new RelayCommand<IDevice>((selectedDevice) =>
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

