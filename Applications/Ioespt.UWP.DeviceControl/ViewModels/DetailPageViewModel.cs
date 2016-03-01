using Ioespt.UWP.DeviceControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Common;
using Template10.Mvvm;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;

namespace Ioespt.UWP.DeviceControl.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        public RegisteredDevice selectedDevice { get; set; }

        public DetailPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
            }
        }

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            selectedDevice = (suspensionState.ContainsKey(nameof(selectedDevice))) ? suspensionState[nameof(selectedDevice)] as RegisteredDevice : parameter as RegisteredDevice;
            return Task.CompletedTask;
        }

        public override Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(selectedDevice)] = selectedDevice;
            }
            return Task.CompletedTask;
        }

        public override Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            return Task.CompletedTask;
        }
    }
}

