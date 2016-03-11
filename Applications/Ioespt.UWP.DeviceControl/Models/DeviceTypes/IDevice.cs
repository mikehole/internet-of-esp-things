using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Ioespt.UWP.DeviceControl.Models.DeviceTypes
{
    public interface IDevice
    {
        RegisteredDevice DeviceDetails { get; set; }

        string GivenName { get; }

        FrameworkElement GeneralControl { get; }
    }
}
