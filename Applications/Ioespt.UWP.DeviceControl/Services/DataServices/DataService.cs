using Ioespt.UWP.DeviceControl.Models;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ioespt.UWP.DeviceControl.Services.DataServices
{
    class DataService
    {
        SQLiteConnection db;

        public DataService()
        {
            var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.sqlite");

            db = new SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
        }

        public void createDB()
        {
            db.CreateTable<RegisteredDevice>();
        }

        public TableQuery<RegisteredDevice> DevicesTable
        {
            get
            {
                return db.Table<RegisteredDevice>();
            }
        }

        public void InsertNewDevice(RegisteredDevice device)
        {
            db.Insert(device);
        }

        public void DeleteDevice(RegisteredDevice device)
        {
            db.Table<RegisteredDevice>().Delete( C=> C.ChipId == device.ChipId);
        }

    }
}
