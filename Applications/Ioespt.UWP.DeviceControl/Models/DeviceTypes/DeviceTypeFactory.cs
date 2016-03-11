using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ioespt.UWP.DeviceControl.Models.DeviceTypes
{
    class DeviceTypeFactory
    {
        public static IDevice MakeDevice(string DeviceType, RegisteredDevice DeviceDetails)
        {
            switch(DeviceType)
            {
                case "IOESPT-Display":
                    return new DisplayDevice() { DeviceDetails = DeviceDetails };
                case "IOESPT-Thermometer":
                    return new ThermometerDevice() { DeviceDetails = DeviceDetails };
                default:
                    return new UnknownDevice() { DeviceDetails = DeviceDetails };
            }
        }
    }
}
