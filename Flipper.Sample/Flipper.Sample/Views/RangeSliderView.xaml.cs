using Flipper.Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Flipper.Sample.Views
{
    public partial class RangeSliderView : ContentPage
    {
        public RangeSliderView()
        {
            InitializeComponent();

            BindingContext = new RangeSliderViewModel();
        }
    }
}
