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
            var key = Guid.NewGuid().ToString();
            var items = new ObservableCollection<string>();

            // Create dummy data
            items.Add("http://dummyimage.com/600x400/000/fff?a=" + key);
            items.Add("http://dummyimage.com/600x500/000/fff?a=" + key);
            items.Add("http://dummyimage.com/590x400/000/fff?a=" + key);
            items.Add("http://dummyimage.com/300x700/000/fff?a=" + key);
            items.Add("http://dummyimage.com/500x1400/000/fff?a=" + key);
            items.Add("http://dummyimage.com/600x450/000/fff?a=" + key);
            items.Add("http://dummyimage.com/2045x130/000/fff?a=" + key);
            items.Add("http://dummyimage.com/800x400/000/fff?a=" + key);

            var model = new MainViewModel()
            {
                Items = items
            };
            return model;
        }
    }
}
