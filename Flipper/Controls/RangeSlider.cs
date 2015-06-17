using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Flipper.Controls
{
    public class RangeSlider : View
    {
        public static readonly BindableProperty CommandProperty =
                BindableProperty.Create<RangeSlider, ICommand>
            (p => p.Command, null);

        public static readonly BindableProperty MaxValueProperty =
                BindableProperty.Create<RangeSlider, double>
            (p => p.MaxValue, double.NaN, BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (RangeSlider)bindable;
                control.RightValue = newValue;
                control.LeftValue = control.MinValue;
            });

        public static readonly BindableProperty MinValueProperty =
               BindableProperty.Create<RangeSlider, double>
           (p => p.MinValue, double.NaN,BindingMode.TwoWay, propertyChanged: (bindable, oldValue, newValue) =>
            {
                var control = (RangeSlider)bindable;
                control.LeftValue = newValue;
                control.RightValue = control.MaxValue;
            });

        public static readonly BindableProperty LeftValueProperty =
               BindableProperty.Create<RangeSlider,double>
           (p => p.LeftValue, double.NaN, BindingMode.TwoWay);

        public static readonly BindableProperty RightValueProperty =
               BindableProperty.Create<RangeSlider, double>
           (p => p.RightValue, double.NaN, BindingMode.TwoWay);

        public new static readonly BindableProperty BackgroundColorProperty =
              BindableProperty.Create<RangeSlider, Color>
          (p => p.BackgroundColor, Color.Gray);

        public static readonly BindableProperty RangeColorProperty =
              BindableProperty.Create<RangeSlider, Color>
          (p => p.RangeColor, Color.Blue);

        public static readonly BindableProperty IndicatorColorProperty =
              BindableProperty.Create<RangeSlider, Color>
          (p => p.IndicatorColor, Color.Blue);

        public static readonly BindableProperty StepProperty =
              BindableProperty.Create<RangeSlider, double>
          (p => p.Step, 1);

        public ICommand Command
        {
            get { return GetValue(CommandProperty) as ICommand; }
            set { SetValue(CommandProperty, value); }
        }

        public event EventHandler ValueChanged;

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public double LeftValue
        {
            get { return (double)GetValue(LeftValueProperty); }
            set { SetValue(LeftValueProperty, value); }
        }

        public double RightValue
        {
            get { return (double)GetValue(RightValueProperty); }
            set { SetValue(RightValueProperty, value); }
        }

        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public new Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public Color RangeColor
        {
            get { return (Color)GetValue(RangeColorProperty); }
            set { SetValue(RangeColorProperty, value); }
        }

        public Color IndicatorColor
        {
            get { return (Color)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }
        }

        public void NotifyValueChanged()
        {
            if(ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
            }
        }
    }
}
