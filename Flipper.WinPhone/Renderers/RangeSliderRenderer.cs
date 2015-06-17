using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Flipper.Controls;
using Xamarin.Forms;
using Flipper.WinPhone.Renderers;
using Xamarin.Forms.Platform.WinPhone;
using System.Windows.Media;

[assembly: ExportRenderer(typeof(RangeSlider), typeof(RangeSliderRenderer))]
namespace Flipper.WinPhone.Renderers
{
    public class RangeSliderRenderer : ViewRenderer<Flipper.Controls.RangeSlider, Controls.RangeSlider>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<RangeSlider> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var slider = new Controls.RangeSlider();

                slider.ValueChanging += slider_ValueChanging;
                slider.ValueChanged += slider_ValueChanged;

                SetNativeControl(slider);
            }

        }

        void slider_ValueChanged(object sender, EventArgs e)
        {
            if (Element.Command != null && Element.Command.CanExecute(null))
            {
                Element.Command.Execute(null);
            }

            Element.NotifyValueChanged();
        }

        void slider_ValueChanging(object sender, EventArgs e)
        {
            Element.LeftValue = (float)Control.LeftValue;
            Element.RightValue = (float)Control.RightValue;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Renderer")
            {
                Control.MaxValue = Element.MaxValue;
                Control.MinValue = Element.MinValue;
                Control.Step = Element.Step;

                Control.Background = ToSolidColorBrush(Element.BackgroundColor);
                Control.Foreground = ToSolidColorBrush(Element.RangeColor);
                Control.IndicatorColor = ToSolidColorBrush(Element.IndicatorColor);
            }
            else if (e.PropertyName == RangeSlider.MaxValueProperty.PropertyName)
            {
                Control.MaxValue = Element.MaxValue;
            }
            else if (e.PropertyName == RangeSlider.MinValueProperty.PropertyName)
            {
                Control.MinValue = Element.MinValue;
            }
            else if (e.PropertyName == RangeSlider.StepProperty.PropertyName)
            {
                Control.Step = Element.Step;
            }
            else if (e.PropertyName == RangeSlider.BackgroundColorProperty.PropertyName)
            {
                Control.Background = ToSolidColorBrush(Element.BackgroundColor);
            }
            else if (e.PropertyName == RangeSlider.RangeColorProperty.PropertyName)
            {
                Control.Foreground = ToSolidColorBrush(Element.RangeColor);
            }
            else if (e.PropertyName == RangeSlider.IndicatorColorProperty.PropertyName)
            {
                Control.IndicatorColor = ToSolidColorBrush(Element.IndicatorColor);
            }
        }

        private SolidColorBrush ToSolidColorBrush(Xamarin.Forms.Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)(color.A*255), (byte)(color.R*255), (byte)(color.G*255), (byte)(color.B*255)));
        }
    }
}