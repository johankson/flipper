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
    public partial class SwiperView : ContentPage
    {
        public SwiperView()
        {
            InitializeComponent();
            BindingContext = CreateViewModel();
        }

        private static SwiperViewModel CreateViewModel()
        {
            var items = new ObservableCollection<string>();

            // Create dummy data by randomly adding 5 images of different sizes
            AddFiveRandomImages(items);

            // Select the third image
           // var third = items.Skip(2).First();

            // At the momement we need to match both index and url
            var model = new SwiperViewModel()
            {
                Items = items,
                Index = 2
            };
            return model;
        }

        public static void AddFiveRandomImages(ObservableCollection<string> items)
        {
            // We need the guid to create a unique URL to ensure
            // that no caching is going on!
            var key = Guid.NewGuid().ToString();

            var r = new Random();
            for (int i = 0; i < 5; i++)
            {
             //   var url = String.Format("http://dummyimage.com/{0}x{1}/fff?a={2}",
             //                           r.Next(100, 1400), r.Next(100, 1000), key);
                var url = String.Format("http://lorempixel.com/{0}/{1}/?a={2}",
                                       r.Next(100, 1400), r.Next(100, 1000), key);
            
                if (!items.Contains(url))
                {
                    items.Add(url);
                }
            }
        }
    }
}