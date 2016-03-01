using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Ioespt.UWP.DeviceControl.Services.DeviceDiscovery
{
    public class SSDPClient
    {
        public event EventHandler<DeviceFoundEventArgs> DeviceFound;

        bool isSearching = false;

        public async void SearchForDevices()
        {
            if (isSearching)
                return;

            isSearching = true;

            var socket = new DatagramSocket();
            socket.MessageReceived += async (sender, args) =>
            {
                DataReader reader = args.GetDataReader();

                uint count = reader.UnconsumedBufferLength;
                string data = reader.ReadString(count);
                var response = new Dictionary<string, string>();
                foreach (
                    string x in
                        data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
                {
                    if (x.Contains(":"))
                    {
                        string[] strings = x.Split(':');
                        response.Add(strings[0].ToLower(), x.Remove(0, strings[0].Length + 1));
                    }
                }

                Device device = await GetXml(response);
                Debug.WriteLine($"Device found : {device.DeviceType.friendlyName}");

                if (DeviceFound != null)
                    DeviceFound(this, new DeviceFoundEventArgs(device));
            };

            IOutputStream stream = await socket.GetOutputStreamAsync(new HostName("239.255.255.250"), "1900");


            const string message = "M-SEARCH * HTTP/1.1\r\n" +
                                   "HOST: 239.255.255.250:1900\r\n" +
                                   "ST:upnp:rootdevice\r\n" +
                                   "MAN:\"ssdp:discover\"\r\n" +
                                   "MX:3\r\n\r\n";

            var writer = new DataWriter(stream) { UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8 };
            writer.WriteString(message);
            await writer.StoreAsync();

            isSearching = false;
        }


        private async Task<Device> GetXml(Dictionary<string, string> headers)
        {
            if (headers.ContainsKey("location"))
            {
                WebRequest request = WebRequest.Create(new Uri(headers["location"]));

                var r = (HttpWebResponse)await request.GetResponseAsync();
                if (r.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        var ser = new XmlSerializer(typeof(Device));
                        return (Device)ser.Deserialize(r.GetResponseStream());
                    }
                    catch (InvalidOperationException ex)
                    {
                        Debug.WriteLine(ex.Message);
                        return null;
                    }
                }

                throw new Exception("Cannot connect to service");
            }

            throw new Exception("No service Uri defined");
        }
    }

    public class DeviceFoundEventArgs : EventArgs
    {
        public DeviceFoundEventArgs(Device device)
        {
            Device = device;
        }

        public Device Device { get; set; }
    }
}
