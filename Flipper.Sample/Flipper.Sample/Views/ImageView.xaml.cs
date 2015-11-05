using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flipper.Sample.ViewModels;
using Xamarin.Forms;

namespace Flipper.Sample.Views
{
    public partial class ImageView : ContentPage
    {
        public ImageView(int index, string url)
        {
            InitializeComponent();

            BindingContext = new ImageViewModel()
            {
                Index = index,
                Url = url
            };
        }
    }
}
