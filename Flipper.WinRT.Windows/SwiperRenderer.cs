using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.WinRT;

[assembly: ExportRenderer(typeof(Flipper.Controls.Swiper), typeof(Flipper.WinRT.SwiperRenderer))]

namespace Flipper.WinRT
{
    public class SwiperRenderer : ViewRenderer<Flipper.Controls.Swiper, Swiper>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Controls.Swiper> e)
        {
            base.OnElementChanged(e);

            var swiper = new Swiper();
            swiper.SelectedIndexChanged += swiper_SelectedIndexChanged;

            SetNativeControl(swiper);
        }

        void swiper_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Element.SelectedIndex != Control.SelectedIndex)
            {
                Element.SelectedIndex = Control.SelectedIndex;
                Element.SelectedUrl = Control.Images[Control.SelectedIndex];
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if(e.PropertyName == "Renderer")
            {
                Control.Images = Element.Source;
                Control.SelectedIndex = Element.SelectedIndex;
                
            }
            else if(e.PropertyName == Controls.Swiper.SelectedIndexProperty.PropertyName)
            {
                Control.SelectedIndex = Element.SelectedIndex;
            }
            else if (e.PropertyName == Controls.Swiper.SelectedIndexProperty.PropertyName)
            {
                Control.Images = Element.Source;
            }

            Control.UpdateImages();
        }
    }
}
