using System;
using System.Collections.Generic;
using Windows.Phone.UI.Input;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Common
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationHelper
    {
        private RelayCommand _goBackCommand;

        private RelayCommand _goForwardCommand;

        private string _pageKey;

        public event EventHandler<LoadStateEventArgs> LoadState;

        public event EventHandler<SaveStateEventArgs> SaveState;

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

        public RelayCommand GoBackCommand
        {
            get
            {
                if (_goBackCommand == null)
                {
                    _goBackCommand = new RelayCommand(
                        () => this.GoBack(),
                        () => this.CanGoBack());
                }

                return _goBackCommand;
            }
            set
            {
                _goBackCommand = value;
            }
        }

        public RelayCommand GoForwardCommand
        {
            get
            {
                if (_goForwardCommand == null)
                {
                    _goForwardCommand = new RelayCommand(
                        () => this.GoForward(),
                        () => this.CanGoForward());
                }

                return _goForwardCommand;
            }
            set
            {
                _goForwardCommand = value;
            }
        }

        private Frame Frame { get { return this.Page.Frame; } }

        private Page Page { get; set; }

        public virtual bool CanGoBack()
        {
            return this.Frame != null && this.Frame.CanGoBack;
        }

        public virtual bool CanGoForward()
        {
            return this.Frame != null && this.Frame.CanGoForward;
        }

        public virtual void GoBack()
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        public virtual void GoForward()
        {
            if (this.Frame != null && this.Frame.CanGoForward)
            {
                this.Frame.GoForward();
            }
        }

        public void OnNavigatedTo(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if(e.NavigationMode == NavigationMode.New)
            {
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;

                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                if(this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, null));
                }
            }
            else
            {
                if(this.LoadState != null)
                {
                    this.LoadState(this, new LoadStateEventArgs(e.Parameter, (Dictionary<string, object>)frameState[this._pageKey]));
                }
            }
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<string, object>();

            if(this.SaveState != null)
            {
                this.SaveState(this, new SaveStateEventArgs(pageState));
            }

            frameState[this._pageKey] = pageState;
        }

#if WINDOWS_PHONE_APP
        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if(this.GoBackCommand.CanExecute(null))
            {
                e.Handled = true;
                this.GoBackCommand.Execute(null);
            }
        }
#else
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            throw new NotImplementedException();
        }
#endif
    }

    public class LoadStateEventArgs : EventArgs
    {
        public object NavigationParameter { get; private set; }

        public Dictionary<string, object> PageState { get; private set; }

        public LoadStateEventArgs(object navigationParameter, Dictionary<string, object> pageState) : base()
        {
            this.NavigationParameter = navigationParameter;
            this.PageState = pageState;
        }
    }

    public class SaveStateEventArgs : EventArgs
    {
        public Dictionary<string, object> PageState { get; private set; }

        public SaveStateEventArgs(Dictionary<string, object> pageState)
        {
            this.PageState = pageState;
        }
    }
}