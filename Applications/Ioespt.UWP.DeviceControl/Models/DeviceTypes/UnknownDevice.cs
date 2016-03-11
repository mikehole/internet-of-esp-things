using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Ioespt.UWP.DeviceControl.Models.DeviceTypes
{
    public class UnknownDevice : IDevice
    {
        public RegisteredDevice DeviceDetails { get; set; }

        private FrameworkElement _myControl = null;
        public FrameworkElement GeneralControl
        {
            get
            {
                return _myControl;
            }
        }

        public string GivenName => DeviceDetails.GivenName;

    }
}
