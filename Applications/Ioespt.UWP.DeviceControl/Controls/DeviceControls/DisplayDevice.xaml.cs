﻿using Ioespt.UWP.DeviceControl.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Ioespt.UWP.DeviceControl.Controls.DeviceControls
{
    public sealed partial class DisplayDevice : UserControl
    {
        private byte[] bitmap = {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0xF0, 0xF0, 0xF0, 0x00, 0x00, 0x00, 0x00, 0xF0, 0xF0, 0xF0, 0x00, 0x00, 0x00, 0x00, 0xF0,
            0xF0, 0xF0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0xF8,
            0xFC, 0xFE, 0x0E, 0x0E, 0x0E, 0x0F, 0x0F, 0x0F, 0x0E, 0x0E, 0x0E, 0x0E, 0x0F, 0x0F, 0x0F, 0x8E,
            0x8E, 0x8E, 0x8E, 0x8F, 0x8F, 0x8F, 0x8E, 0x8E, 0x0E, 0x0E, 0xFE, 0xFC, 0xF8, 0x80, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1C, 0x1C, 0x1C, 0x1C,
            0x0E, 0x87, 0xC7, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0xFF, 0xFF,
            0xFF, 0xC7, 0x87, 0x0E, 0x1C, 0x1C, 0x1C, 0x1C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x0E, 0x0E, 0x0E, 0x0E, 0x87, 0xC3, 0xE3, 0xFF, 0xFF, 0xFF, 0x00, 0xF0, 0x90, 0x90, 0x90, 0x00,
            0x20, 0x50, 0x90, 0x10, 0x00, 0x00, 0xF0, 0x90, 0x90, 0x60, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xE3, 0xC3, 0x87, 0x0E, 0x0E, 0x0E, 0x0E, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x07, 0x07, 0x07, 0x07, 0x03, 0x01, 0x01, 0x3F, 0x7F, 0xFF, 0xE0, 0xE7,
            0xE4, 0xE4, 0xE4, 0xE0, 0xE4, 0xE4, 0xE4, 0xE3, 0xE0, 0xE0, 0xE7, 0xE0, 0xE0, 0xE0, 0xE0, 0xE0,
            0xE0, 0xE0, 0xE0, 0xE0, 0xE0, 0xE0, 0xFF, 0x7F, 0x3F, 0x01, 0x01, 0x03, 0x07, 0x07, 0x07, 0x07,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x00,
            0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

        public DisplayDevice()
        {
            this.InitializeComponent();

            for (int x = 0; x < 84; x++)
                displayGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int y = 0; y < 48; y++)
                displayGrid.RowDefinitions.Add(new RowDefinition());

            //int col = 0;
            //int row = 0;

            //var brush = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));

            //foreach (var b in bitmap)
            //{
            //    for(int cbit = 0; cbit <= 8; cbit++)
            //    {
            //        if ((b & (1 << cbit - 1)) != 0)
            //        {
            //            var rect = new Rectangle();

            //            rect.Fill = brush;

            //            Grid.SetColumn(rect, col);

            //            Grid.SetRow(rect, (row * 7) + cbit);

            //            displayGrid.Children.Add(rect);
            //        }
            //    }

            //    col = col + 1;
            //    if (col > 83)
            //    {
            //        row = row + 1;
            //        col = 0;
            //    }
            //}
        }

        //private async void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    HttpClient httpClient = new HttpClient();

        //    //for (int x = 0; x < bitmap.Length; x++)
        //    //    bitmap[x] = 0xFF;

        //    //ByteArrayContent sc = new ByteArrayContent(bitmap);

        //    //await httpClient.PutAsync("http://192.168.1.103/setbitmap", sc);

        //    byte[] ddd = { 0xBB };

        //    ByteArrayContent sc = new ByteArrayContent(ddd);

        //    await httpClient.PutAsync("http://192.168.1.103/setcontrast", sc);

        //}
        public Models.DeviceTypes.DisplayDevice ViewModel => DataContext as Models.DeviceTypes.DisplayDevice;
    }



}
