using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Common
{
    public class NavigatedPage : Page
    {
        protected NavigationHelper navigationHelper;
        
        public NavigatedPage()
            : base()
        {
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += OnNavigationHelperLoadState;
            this.navigationHelper.SaveState += OnNavigationHelperSaveState;
        }

        protected virtual void OnNavigationHelperSaveState(object sender, SaveStateEventArgs e)
        {
        }

        protected virtual void OnNavigationHelperLoadState(object sender, LoadStateEventArgs e)
        {
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
    }
}
