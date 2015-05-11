using Flipper.Controls;
using Flipper.Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Flipper.Sample.Views
{
    public partial class MainView : ContentPage
    {
        public MainView()
        {
            InitializeComponent();

            var model = CreateViewModel();

            BindingContext = model;
        }

        private static MainViewModel CreateViewModel()
        {
            // We need the guid to create a unique URL to ensure
            // that no caching is going on!
            var key = Guid.NewGuid().ToString();
            var items = new ObservableCollection<string>();

            // Create dummy data by randomly adding 50 images of different sizes
            var r = new Random();
            for(int i=0;i<50;i++)
            {
                var url = String.Format("http://dummyimage.com/{0}x{1}/fff?a={2}",
                                        r.Next(100, 1400), r.Next(100, 1000), key);

                if(!items.Contains(url))
                {
                    items.Add(url);
                }
            }

            var model = new MainViewModel()
            {
                Items = items
            };
            return model;
        }
    }
}
