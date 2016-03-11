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
    [ImplementPropertyChanged]
    public class DisplayDevice : IDevice
    {
        public DisplayDevice()
        {
            Contrast = 0xBB;
        }

        public RegisteredDevice DeviceDetails { get; set; }

        public string GivenName => DeviceDetails.GivenName;

        private Controls.DeviceControls.DisplayDevice _myControl = null;

        public FrameworkElement GeneralControl
        {
            get
            {
                if(_myControl == null)
                {
                    _myControl = new Controls.DeviceControls.DisplayDevice();

                    _myControl.DataContext = this;
                }
                return _myControl;
            }
        }

        [AlsoNotifyFor("ContrastString")]
        public byte Contrast { get; set; }

        public string ContrastString => string.Format("0x{0:X}", Contrast);

        public async void UpContrast()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                Contrast += 5;
                byte[] ddd = { Contrast };
                ByteArrayContent sc = new ByteArrayContent(ddd);
                await httpClient.PutAsync("http://192.168.1.103/setcontrast", sc);
            }
            catch(Exception)
            {
            }
        }

        public async void DnContrast()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                Contrast -= 5;
                byte[] ddd = { Contrast };
                ByteArrayContent sc = new ByteArrayContent(ddd);
                await httpClient.PutAsync("http://192.168.1.103/setcontrast", sc);
            }
            catch (Exception)
            {
            }
        }


    }
}
