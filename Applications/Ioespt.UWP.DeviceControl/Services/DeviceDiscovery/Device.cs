using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Ioespt.UWP.DeviceControl.Services.DeviceDiscovery
{
    [XmlRoot("root", Namespace = "urn:schemas-upnp-org:device-1-0")]
    public class Device
    {
        public Device()
        {

            this.DeviceType = new DeviceType();
            this.SpecVersion = new SpecVersionType();
        }

        [XmlElement("specVersion")]
        public SpecVersionType SpecVersion { get; set; }

        public string URLBase { get; set; }

        [XmlElement("device")]
        public DeviceType DeviceType { get; set; }

    }

    public class SpecVersionType
    {

        public SpecVersionType()
        {
        }

        private int majorField;

        private int minorField;

        public int major
        {
            get
            {
                return this.majorField;
            }
            set
            {
                this.majorField = value;
            }
        }

        public int minor
        {
            get
            {
                return this.minorField;
            }
            set
            {
                this.minorField = value;
            }
        }
    }

    public class DeviceType
    {
        public DeviceType()
        {
            this.Any = new List<XmlElement>();
            this.deviceList = new List<DeviceType>();
            this.serviceList = new List<ServiceListTypeService>();
            this.iconList = new List<IconListTypeIcon>();
        }

        public string deviceType { get; set; }

        public string friendlyName { get; set; }

        public string manufacturer { get; set; }

        public string manufacturerURL { get; set; }

        public string modelDescription { get; set; }

        public string modelName { get; set; }

        public string modelNumber { get; set; }

        public string modelURL { get; set; }

        public string serialNumber { get; set; }

        public string UDN { get; set; }

        public string UPC { get; set; }


        public List<IconListTypeIcon> iconList { get; set; }


        public List<ServiceListTypeService> serviceList { get; set; }

        public List<DeviceType> deviceList { get; set; }

        public string presentationURL { get; set; }

        [XmlIgnore]
        public List<XmlElement> Any { get; set; }
    }

    public partial class IconListTypeIcon
    {

        public IconListTypeIcon()
        {
        }

        private string mimetypeField;

        private int widthField;

        private int heightField;

        private int depthField;

        private string urlField;

        public string mimetype
        {
            get
            {
                return this.mimetypeField;
            }
            set
            {
                this.mimetypeField = value;
            }
        }

        public int width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        public int height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        public int depth
        {
            get
            {
                return this.depthField;
            }
            set
            {
                this.depthField = value;
            }
        }

        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }
    }

    public partial class ServiceListTypeService
    {

        public ServiceListTypeService()
        {
        }

        private string serviceTypeField;

        private string serviceIdField;

        private string sCPDURLField;

        private string controlURLField;

        private string eventSubURLField;

        public string serviceType
        {
            get
            {
                return this.serviceTypeField;
            }
            set
            {
                this.serviceTypeField = value;
            }
        }

        public string serviceId
        {
            get
            {
                return this.serviceIdField;
            }
            set
            {
                this.serviceIdField = value;
            }
        }

        public string SCPDURL
        {
            get
            {
                return this.sCPDURLField;
            }
            set
            {
                this.sCPDURLField = value;
            }
        }

        public string controlURL
        {
            get
            {
                return this.controlURLField;
            }
            set
            {
                this.controlURLField = value;
            }
        }

        public string eventSubURL
        {
            get
            {
                return this.eventSubURLField;
            }
            set
            {
                this.eventSubURLField = value;
            }
        }
    }

    public partial class IconListType
    {


        private List<IconListTypeIcon> iconField;

        public IconListType()
        {
            this.iconField = new List<IconListTypeIcon>();
        }

        public List<IconListTypeIcon> icon
        {
            get
            {
                return this.iconField;
            }
            set
            {
                this.iconField = value;
            }
        }
    }

    public partial class ServiceListType
    {


        private List<ServiceListTypeService> serviceField;

        public ServiceListType()
        {
            this.serviceField = new List<ServiceListTypeService>();
        }

        public List<ServiceListTypeService> service
        {
            get
            {
                return this.serviceField;
            }
            set
            {
                this.serviceField = value;
            }
        }
    }

    public partial class DeviceListType
    {

        private List<DeviceType> deviceField;

        public DeviceListType()
        {
            this.deviceField = new List<DeviceType>();
        }

        public List<DeviceType> device
        {
            get
            {
                return this.deviceField;
            }
            set
            {
                this.deviceField = value;
            }
        }
    }
}
