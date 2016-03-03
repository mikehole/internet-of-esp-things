using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ioespt.UWP.DeviceControl.Models.DeviceTypes
{
    public class TempretureReading
    {
        public int Tempreture { get; set; }
    }



    [ImplementPropertyChanged]
    class ThermometerDevice : RegisteredDevice
    {
        [AlsoNotifyFor("TempretureText")]
        public int CurrentTempreture { get; set; }

        public string TempretureText => $"{CurrentTempreture}c";

        public async Task<bool> UpdateValue()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var stringRes = await httpClient.GetStringAsync($"http://{Ip}/gettempreture");
                var reading = JsonConvert.DeserializeObject<TempretureReading>(stringRes);

                CurrentTempreture = reading.Tempreture;

                return true;
            }
            catch (Exception E)
            {
                return false;
            }
        }
    }
}
