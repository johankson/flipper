using Flipper.Sample.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Flipper.Sample.ViewModels
{
    class MainViewModel
    {
        private INavigation _navigation;

        public MainViewModel(INavigation navigation)
        {
            _navigation = navigation;
        }

        public ICommand NavigateToSwiper
        {
            get
            {
                return new Command(async () =>
                    {
                        await _navigation.PushAsync(new SwiperView());
                    });
            }
        }
    }
}
