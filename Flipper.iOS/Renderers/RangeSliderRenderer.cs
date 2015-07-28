using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Flipper.Controls;
using CoreGraphics;
using Flipper.iOS.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(RangeSlider), typeof(RangeSliderRenderer))]
namespace Flipper.iOS.Renderers
{
    public class RangeSliderRenderer : ViewRenderer<Flipper.Controls.RangeSlider, iOS.Controls.RangeSlider>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<RangeSlider> e)
        {
            base.OnElementChanged(e);
            
            if (e.NewElement != null)
            {
                var slider = new Controls.RangeSlider();
                slider.Frame = new CGRect((float)Frame.X, (float)Frame.Y, (float)Frame.Width, 200);
                

                slider.ValueChanging += slider_ValueChanging;
                slider.ValueChanged += slider_ValueChanged;

                SetNativeControl(slider); 
            }

        }

        void slider_ValueChanged(object sender, EventArgs e)
        {
           if(Element.Command != null && Element.Command.CanExecute(null))
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
                Control.LeftValue = Element.LeftValue;
                Control.RightValue = Element.RightValue;
                Control.Step = Element.Step;

                Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
                Control.RangeColor = Element.RangeColor.ToUIColor();
                Control.IndicatorColor = Element.IndicatorColor.ToUIColor();
            }
            else if(e.PropertyName == RangeSlider.MaxValueProperty.PropertyName)
            {
                Control.MaxValue = Element.MaxValue;
            }
            else if (e.PropertyName == RangeSlider.MinValueProperty.PropertyName)
            {
                Control.MinValue = Element.MinValue;
            }
            else if (e.PropertyName == RangeSlider.LeftValueProperty.PropertyName)
            {
                Control.LeftValue = Element.LeftValue;
            }
            else if (e.PropertyName == RangeSlider.RightValueProperty.PropertyName)
            {
                Control.LeftValue = Element.RightValue;
            }
            else if (e.PropertyName == RangeSlider.StepProperty.PropertyName)
            {
                Control.Step = Element.Step;
            }
            else if (e.PropertyName == RangeSlider.BackgroundColorProperty.PropertyName)
            {
                Control.BackgroundColor = Element.BackgroundColor.ToUIColor();
            }
            else if (e.PropertyName == RangeSlider.RangeColorProperty.PropertyName)
            {
                Control.RangeColor = Element.RangeColor.ToUIColor();
            }
            else if (e.PropertyName == RangeSlider.IndicatorColorProperty.PropertyName)
            {
                Control.IndicatorColor = Element.IndicatorColor.ToUIColor();
            }
        }
    }
}