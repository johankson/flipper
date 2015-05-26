using Flipper.Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Flipper.Sample.Views
{
    public partial class HoldButtonView : ContentPage
    {
        public HoldButtonView()
        {
            InitializeComponent();
            BindingContext = CreateViewModel();
        }

        /// <summary>
        /// This is just a sample, so we create the view model here
        /// </summary>
        /// <returns></returns>
        private HoldButtonViewModel CreateViewModel()
        {
            return new HoldButtonViewModel();
        }
    }
}
