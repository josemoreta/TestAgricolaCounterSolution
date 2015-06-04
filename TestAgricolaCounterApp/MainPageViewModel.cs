using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace TestAgricolaCounterApp
{
    public class MainPageViewModel : ViewModel
    {
        public string GameCaption { get; set; }

        public RelayCommand SelectGame { get; set; }

        public MainPageViewModel(NavigationHelper navigationHelper)
        {
            this.GameCaption = "Agricola";
            this.SelectGame = new RelayCommand(this.SelectGameInternal);
        }

        private void SelectGameInternal()
        {
            //NavigationEventArgs e = new NavigationEventArgs();
           
            //this.NavigationHelper.OnNavigatedTo(e);
        }
        
    }
}
