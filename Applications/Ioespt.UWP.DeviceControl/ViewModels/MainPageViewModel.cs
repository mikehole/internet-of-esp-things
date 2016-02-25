using Template10.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Ioespt.UWP.DeviceControl.Models;

namespace Ioespt.UWP.DeviceControl.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ObservableCollection<RegisteredDevice> devices { get; set; }


        public MainPageViewModel()
        {
            devices = new ObservableCollection<RegisteredDevice>();

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {

            }

            devices.Add(new RegisteredDevice()
            {
                details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Slave", FirmwareVersion="0,1,0", ModuleType = "ESP8266-01" },
                GivenName = "Lamp 1",
                Status = DeviceStatus.Offline,
                ConnectedTo = "Adafruit IO (MQTT)"
            });
            devices.Add(new RegisteredDevice()
            {
                details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Master", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                GivenName = "Socket 1",
                Status = DeviceStatus.Online,
                ConnectedTo = "Amazon AWS"
            });
            devices.Add(new RegisteredDevice()
            {
                details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Slave", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-12" },
                GivenName = "Weather Station",
                Status = DeviceStatus.Online,
                ConnectedTo = "Azure IoT Hub"
            });

            devices.Add(new RegisteredDevice()
            {
                details = new Devices.DeviceDetails() { FirmwareName = "IOESPT-Master", FirmwareVersion = "0,1,0", ModuleType = "ESP8266-01" },
                GivenName = "Doorbell",
                Status = DeviceStatus.UnProvisioned,
                ConnectedTo = "N/A"
            });

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

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), null);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
}

