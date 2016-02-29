using Ioespt.UWP.Devices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Ioespt.UWP.DeviceControl.Models
{
    public enum DeviceStatus
    {
        Offline,
        Online,
        UnProvisioned
    }


    [JsonObject(MemberSerialization.OptIn)]
    public class RegisteredDevice
    {
        [JsonProperty]
        public DeviceStatus Status { get; set; }

        [JsonProperty]
        public string GivenName { get; set;  }

        [JsonProperty]
        public string ModuleType { get; set; }

        [JsonProperty]
        public string FirmwareName { get; set; }

        [JsonProperty]
        public string FirmwareVersion { get; set; }

        [JsonProperty]
        public string ChipId { get; set; }

        [JsonProperty]
        public string Ip { get; set; }

        [JsonProperty]
        public string ConnectedTo { get; set; }

        public string StatusText
        {
            get {
                switch (Status)
                {
                    case DeviceStatus.Offline:
                        return "offline";
                    case DeviceStatus.Online:
                        return "online";
                    default:
                        return "unprovisioned";
                }
            }
        }

        public SolidColorBrush StatusColor
        {
            get
            {
                switch (Status)
                {
                    case DeviceStatus.Offline:
                        return new SolidColorBrush(Color.FromArgb(255,255,0,0));
                    case DeviceStatus.Online:
                        return new SolidColorBrush(Color.FromArgb(255,0, 255, 0));
                    default:
                        return new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                }

            }
        }

        public string DeviceIcon
        {
            get
            {
                switch( FirmwareName )
                {
                    case "IOESPT-Slave":
                        return "ms-appx:///Assets/DeviceTypes/SlaveDevice.png";
                    default:
                        return "ms-appx:///Assets/DeviceTypes/MasterDevice.png";
                }
            }
                

        }

    }
}
