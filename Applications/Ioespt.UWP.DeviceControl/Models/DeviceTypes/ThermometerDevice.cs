using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Ioespt.UWP.DeviceControl.Models.DeviceTypes
{
    public class TemperatureReading
    {
        public double Temperature { get; set; }
    }

    [ImplementPropertyChanged]
    public class ThermometerDevice : IDevice
    {
        public RegisteredDevice DeviceDetails { get; set; }

        public string GivenName => DeviceDetails.GivenName;


        [AlsoNotifyFor("TemperatureText")]
        public double CurrentTemperature { get; set; }

        public string TemperatureText => $"{CurrentTemperature}c";

        Controls.DeviceControls.ThermometerDevice _myControl = null;

        public FrameworkElement GeneralControl
        {
            get
            {
                if(_myControl == null)
                {
                    _myControl = new Controls.DeviceControls.ThermometerDevice();
                    _myControl.DataContext = this;
                }

                return _myControl;
            }
        }

        public async Task<bool> UpdateValue()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var stringRes = await httpClient.GetStringAsync($"http://{DeviceDetails.Ip}/gettemperature");
                var reading = JsonConvert.DeserializeObject<TemperatureReading>(stringRes);

                CurrentTemperature = reading.Temperature;

                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }

        public async void Refresh()
        {
            await UpdateValue();
        }



    }
}
