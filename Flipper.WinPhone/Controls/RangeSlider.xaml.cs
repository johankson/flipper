using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Flipper.WinPhone.Controls
{
    public partial class RangeSlider : UserControl
    {
        private TranslateTransform _leftTransform, _rightTransform, _rangeTransform, _leftTouchTransform, _rightTouchTransform;
        protected double _lastLeftValue, _lastRightValue;

        public event EventHandler ValueChanged;
        public event EventHandler ValueChanging;

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double),
            typeof(RangeSlider), new PropertyMetadata(double.NaN, new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) =>{
                var slider = (RangeSlider)sender;
                slider.RightValue = (double)args.NewValue;
                slider._lastLeftValue = slider.RightValue;
            })));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double),
            typeof(RangeSlider), new PropertyMetadata(double.NaN, new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            {
                var slider = (RangeSlider)sender;
                slider.LeftValue = (double)args.NewValue;
                slider._lastLeftValue = slider.LeftValue;
            })));

        public static readonly DependencyProperty LeftValueProperty =
            DependencyProperty.Register("LeftValue", typeof(double),
            typeof(RangeSlider), new PropertyMetadata(double.NaN, new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            {
                var slider = (RangeSlider)sender;
                slider.OnValueChanging();
            })));

        public static readonly DependencyProperty RightValueProperty =
            DependencyProperty.Register("RightValue", typeof(double),
            typeof(RangeSlider), new PropertyMetadata(double.NaN, new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) => {
                var slider = (RangeSlider)sender;
                slider.OnValueChanging();
            })));

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(double),
            typeof(RangeSlider), new PropertyMetadata(1.0));


        public new static readonly DependencyProperty ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(SolidColorBrush),
            typeof(RangeSlider), new PropertyMetadata(new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]), new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) => {
                var slider = (RangeSlider)sender;

                if (slider.Range != null)
                {
                    slider.Range.Fill = (SolidColorBrush)args.NewValue; 
                }
            })));

        public new static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(SolidColorBrush),
            typeof(RangeSlider), new PropertyMetadata(new SolidColorBrush(Colors.LightGray), new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            {
                var slider = (RangeSlider)sender;

                if (slider.BackgroundRectangle != null)
                {
                    slider.BackgroundRectangle.Fill = (SolidColorBrush)args.NewValue; 
                }
            })));

        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(SolidColorBrush),
            typeof(RangeSlider), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray), new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) =>
            {
                var slider = (RangeSlider)sender;

                if (slider.Left != null && slider.Right != null)
                {
                    slider.Left.Fill = (SolidColorBrush)args.NewValue;
                    slider.Right.Fill = (SolidColorBrush)args.NewValue; 
                }
            })));


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

        public new SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        public new SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }
        public SolidColorBrush IndicatorColor
        {
            get { return (SolidColorBrush)GetValue(IndicatorColorProperty); }
            set { SetValue(IndicatorColorProperty, value); }
        }


        public RangeSlider()
        {
            InitializeComponent();

            LeftTouch.ManipulationDelta += ManipulationDelta;
            LeftTouch.ManipulationStarted += Touch_ManipulationStarted;
            LeftTouch.ManipulationCompleted += Touch_ManipulationCompleted;
            RightTouch.ManipulationDelta += ManipulationDelta;
            RightTouch.ManipulationStarted += Touch_ManipulationStarted;
            RightTouch.ManipulationCompleted += Touch_ManipulationCompleted;

            SizeChanged += RangeSlider_SizeChanged;

           _leftTransform = new TranslateTransform();
           Left.RenderTransform = _leftTransform;

           _rightTransform = new TranslateTransform();
           Right.RenderTransform = _rightTransform;

           _rangeTransform = new TranslateTransform();
           Range.RenderTransform = _rangeTransform;

           _leftTouchTransform = new TranslateTransform();
           LeftTouch.RenderTransform = _leftTransform;

           _rightTouchTransform = new TranslateTransform();
           RightTouch.RenderTransform = _rightTransform;

           Range.Fill = Foreground;
           BackgroundRectangle.Fill = Background;
           Left.Fill = IndicatorColor;
           Right.Fill = IndicatorColor;
        }

        protected void OnValueChanging()
        {
            if(ValueChanging != null)
            {
                ValueChanging(this, new EventArgs());
            }
        }
        void Touch_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if(ValueChanged != null && (_lastLeftValue != LeftValue || _lastRightValue != RightValue))
            {
                _lastLeftValue = LeftValue;
                _lastRightValue = RightValue;

                ValueChanged(this, new EventArgs());
            }
        }

        void Touch_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _lastStep = 0;
        }

        void RangeSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _leftTransform.X = -(this.RenderSize.Width/2-10);
            _rightTransform.X = (this.RenderSize.Width/2 -10);

            SetTouchAreaTransform();

            Range.Width = this.RenderSize.Width;
        }
        private int _lastStep;

        void ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            TranslateTransform transform = null;            

            if(sender == LeftTouch)
            {
                transform = _leftTransform;                
            }
            else if(sender == RightTouch)
            {
                transform = _rightTransform;
            }

            if (transform != null)
            {
                var newTransform = transform.X + e.DeltaManipulation.Translation.X;

                    var stepLength = RenderSize.Width / ((MaxValue - MinValue) / Step);

                    var translation = 0.0; 
                    
                    if(e.DeltaManipulation.Translation.X < 0)
                    {
                        translation = Math.Ceiling(e.DeltaManipulation.Translation.X*-1 / stepLength);
                        translation *= -1;
                    }
                    else
                    {
                        translation = Math.Ceiling(e.DeltaManipulation.Translation.X / stepLength);
                    }

                    var cumulativeTranslation = e.CumulativeManipulation.Translation.X;

                    if ((translation > 0 && cumulativeTranslation/stepLength > _lastStep) ||
                        translation < 0 && cumulativeTranslation/stepLength < _lastStep)
                    {
                        if (translation > 0)
                        {
                            _lastStep++;
                        }
                        else
                        {
                            _lastStep--;
                        }

                        transform.X += translation * stepLength;

                        if (transform.X > (this.RenderSize.Width / 2 - 10))
                        {
                            transform.X = this.RenderSize.Width / 2 - 10;
                        }
                        else if (transform.X < -(this.RenderSize.Width / 2 - 10))
                        {
                            transform.X = -(this.RenderSize.Width / 2 - 10);
                        }

                        
                         var leftPosition = Left.TransformToVisual(this).Transform(new Point(0, 0));
                         var rightPosition = Right.TransformToVisual(this).Transform(new Point(0, 0));

                        if(rightPosition.X <= leftPosition.X)
                        {
                            if (sender == RightTouch)
                            {
                                _rightTransform.X = _leftTransform.X + stepLength;
                            }
                            else
                            {
                                _leftTransform.X = _rightTransform.X - stepLength;
                            }

                            leftPosition = Left.TransformToVisual(this).Transform(new Point(0, 0));
                            rightPosition = Right.TransformToVisual(this).Transform(new Point(0, 0));
                        }

                        if (sender == LeftTouch)
                        {
                            _rangeTransform.X = leftPosition.X;

                            if (_rangeTransform.X < 0)
                            {
                                _rangeTransform.X = 0;
                            }
                        }

                        var width = rightPosition.X - leftPosition.X;
                        if (width >= 0)
                        {
                            Range.Width = width; 
                        }
                    }
	            

                SetTouchAreaTransform();
            }

            CaluclateValues();
        }

        private void CaluclateValues()
        {
            var pixelStep = (MaxValue - MinValue) /RenderSize.Width;

            var leftPosition = Left.TransformToVisual(this).Transform(new Point(0, 0));
            var rightPosition = Right.TransformToVisual(this).Transform(new Point(0, 0));

            LeftValue = Round(MinValue + (pixelStep * leftPosition.X));
            RightValue = Round(MinValue + (pixelStep * (rightPosition.X+20)));         
        }

        private double Round(double value)
        {
            var result = 0.0;

            var overflow = value % Step;
            
            if(overflow > Step/2)
            {
                result = value + Step - overflow;
            }

            result = value - overflow;

            return result;
        }

        private void SetTouchAreaTransform()
        {
            _rightTouchTransform.X = _rightTransform.X - 10;
            _leftTouchTransform.X = _leftTransform.X - 10;
        }
    }
}
