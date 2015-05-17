using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Common
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationHelper
    {
        private Page Page { get; set; }

        private Frame Frame { get { return this.Page.Frame; } }

        public NavigationHelper(Page page)
        {
            this.Page = page;

            this.Page.Loaded += (sender, e) =>
                {
#if WINDOWS_PHONE_APP
                    HardwareButtons.BackPressed += HardwareButtons_BackPressed;
#else
                    if (this.Page.ActualHeight == Window.Current.Bounds.Height && this.Page.ActualWidth == Window.Current.Bounds.Width)
                    {
                        Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
                        Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                    }
#endif
                };

            this.Page.Unloaded += (sender, e) =>
                {
#if WINDOWS_PHONE_APP
                    Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
#else
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
#endif
                };
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            throw new NotImplementedException();
        }
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
