using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Flipper.Controls
{
    public class Swiper : View
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create<Swiper, ObservableCollection<string>>(
            (p) => p.Source, null);

        public Swiper()
        {
        }

        public ObservableCollection<string> Source
        {

            get { return (ObservableCollection<string>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
    }
}

