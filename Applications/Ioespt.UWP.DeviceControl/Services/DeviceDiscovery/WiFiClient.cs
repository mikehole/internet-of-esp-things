using Ioespt.UWP.DeviceControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.WiFi;

namespace Ioespt.UWP.DeviceControl.Services.DeviceDiscovery
{
    class WiFiClient
    {
        public event EventHandler<DeviceFoundEventArgs> DeviceFound;

        private WiFiAdapter firstAdapter;

        public WiFiClient()
        {
            Initialise().Wait();
        }

        public async Task<bool> Initialise()
        {
            var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(WiFiAdapter.GetDeviceSelector());

            if (result.Count >= 1)
            {
                firstAdapter = await WiFiAdapter.FromIdAsync(result[0].Id);
            }
            else
                return false;

            return true;
        }

        public async void Search()
        {
            if (firstAdapter == null)
                return;

            await firstAdapter.ScanAsync();

            //firstAdapter.AvailableNetworksChanged += FirstAdapter_AvailableNetworksChanged;

            var qualifyingWifi = firstAdapter.NetworkReport.AvailableNetworks.Where(N => N.Ssid.ToLower().StartsWith("ioespt-thing"));

            foreach (WiFiAvailableNetwork deviceWifi in qualifyingWifi)
            {
                RegisteredDevice FoundDevice = new RegisteredDevice()
                {
                    Status = DeviceStatus.UnProvisioned,
                    ConnectedTo = "None",
                    GivenName = deviceWifi.Ssid,
                    ChipId = "Unknown",
                    FirmwareName = "Unknown",
                    FirmwareVersion = "Unknown",
                    ModuleType = "Unknown"
                };

                if (DeviceFound != null)
                    DeviceFound(this, new DeviceFoundEventArgs(FoundDevice));
            }
        }

        public class DeviceFoundEventArgs : EventArgs
        {
            public DeviceFoundEventArgs(RegisteredDevice device)
            {
                Device = device;
            }

            public RegisteredDevice Device { get; set; }
        }
    }
}
