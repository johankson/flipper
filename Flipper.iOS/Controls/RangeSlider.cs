using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using System.Drawing;
using CoreGraphics;

namespace Flipper.iOS.Controls
{
    public class RangeSlider : UIView
    {
        private UIView _background, _leftIndicator, _rightIndicator, _range, _leftTouchArea, _rightTouchArena;
        private UIPanGestureRecognizer _leftIndicatorGesture, _rightIndicatorGesture;

        public event EventHandler ValueChanging, ValueChanged;
        private bool _isInitialized;

        private double _leftValue;
        public double LeftValue
        {
            get
            {
                return _leftValue;
            }
            set
            {
                if (!_isInitialized)
                {

                    UpdateValue(_leftIndicator, _leftTouchArea, value); 
                }

                _leftValue = value;
                
            }
        }

        private double _rightValue { get; set; }
        public double RightValue
        {
            get
            {
                return _rightValue;
            }
            set
            {
                if (!_isInitialized)
                {
                    if(MaxValue < value)
                    {
                        MaxValue = value;
                    }

                    UpdateValue(_rightIndicator, _rightTouchArena, value); 
                }

                _rightValue = value;
                
            }
        }

        private void UpdateValue(UIView indicator, UIView touchArea, double value)
        {
            var percent = value / (MaxValue - MinValue);

            var position = (double)(_background.Frame.Width * percent);

            if (!double.IsNaN(position))
            {
                indicator.Center = new CGPoint(position, indicator.Center.Y);
                touchArea.Center = new CGPoint(position, indicator.Center.Y);

                var width = _rightIndicator.Center.X - _leftIndicator.Center.X;
                _range.Frame = new CoreGraphics.CGRect(_leftIndicator.Center.X, _range.Frame.Y, width, _range.Frame.Height);

                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs()); 
                }
            }
        }

        private double _minValue;
        public double MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                _minValue = value;
                
            }
        }

        private double _maxValue;
        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                _maxValue = value;

                UpdateValue(_rightIndicator, _rightTouchArena, RightValue);
            }
        }


        public double Step
        {
            get;
            set;
        }

        public new UIColor BackgroundColor
        {
            get { return _background.BackgroundColor; }
            set { _background.BackgroundColor = value; }
        }

        public UIColor IndicatorColor
        {
            get { return _leftIndicator.BackgroundColor; }
            set
            {
                _leftIndicator.BackgroundColor = value;
                _rightIndicator.BackgroundColor = value;
            }
        }

        public UIColor RangeColor
        {
            get { return _range.BackgroundColor; }
            set { _range.BackgroundColor = value; }
        }


        public RangeSlider()
        {
            _background = new UIView();
            _background.BackgroundColor = UIColor.LightGray;

            _range = new UIView();
            _range.BackgroundColor = UIColor.Blue;

            _leftIndicator = CreateIndicator();
            _leftIndicatorGesture = new UIPanGestureRecognizer(OnPan);

            _rightIndicator = CreateIndicator();
            _rightIndicatorGesture = new UIPanGestureRecognizer(OnPan);

            _leftTouchArea = new UIView();
            _leftTouchArea.BackgroundColor = UIColor.Clear;
            _leftTouchArea.AddGestureRecognizer(_leftIndicatorGesture);

            _rightTouchArena = new UIView();
            _rightTouchArena.BackgroundColor = UIColor.Clear;
            _rightTouchArena.AddGestureRecognizer(_rightIndicatorGesture);

            AddSubview(_background);
            AddSubview(_range);
            AddSubview(_leftIndicator);
            AddSubview(_rightIndicator);
            AddSubview(_leftTouchArea);
            AddSubview(_rightTouchArena);
        }

        private UIView CreateIndicator()
        {
            var indicator = new UIView()
            {
                BackgroundColor = UIColor.Gray
            };

            indicator.Layer.CornerRadius = 10;
            indicator.Layer.MasksToBounds = true;

            return indicator;
        }

        private bool _layouted;
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (!_layouted)
            {
                _background.Frame = new RectangleF(0, 19, (float)Frame.Width-20, 2);
                _range.Frame = new RectangleF(0, 19, (float)Frame.Width-20, 2);
                _leftIndicator.Frame = new RectangleF(0, 10, 20, 20);
                _rightIndicator.Frame = new RectangleF((float)Frame.Width - 40, 10, 20, 20);

                _leftTouchArea.Frame = new RectangleF(0, 0, 40, 40);
                _rightTouchArena.Frame = new RectangleF((float)Frame.Width - 60, 0, 40, 40);

                UpdateValue(_leftIndicator, _leftTouchArea, LeftValue);
                UpdateValue(_rightIndicator, _rightTouchArena, RightValue);

                _layouted = true;
            }

        }


        private float _startX;
        private float _lastStep;
        private void OnPan(UIPanGestureRecognizer recognizer)
        {
            if (recognizer.State == UIGestureRecognizerState.Began || recognizer.State == UIGestureRecognizerState.Changed)
            {
                _isInitialized = true;

                var stepLength = _background.Frame.Width / ((MaxValue - MinValue) / Step);

                var touchPoint = recognizer.LocationInView(this);

                UIView indicator = null;
                UIView touchArea = null;

                //Is this a slide to left or right?
                if (recognizer == _leftIndicatorGesture)
                {
                    indicator = _leftIndicator;
                    touchArea = _leftTouchArea;
                }
                else if (recognizer == _rightIndicatorGesture)
                {
                    indicator = _rightIndicator;
                    touchArea = _rightTouchArena;
                }

                if (recognizer.State == UIGestureRecognizerState.Began)
                {
                    _startX = (float)indicator.Center.X;
                }


                var cumulativeManipulation = touchPoint.X - _startX;
                var deltaManipulation = touchPoint.X - indicator.Center.X;

                if (deltaManipulation > 0 && cumulativeManipulation / stepLength > _lastStep ||
                    deltaManipulation < 0 && cumulativeManipulation / stepLength < _lastStep)
                {
                    if (deltaManipulation > 0)
                    {
                        _lastStep++;
                    }
                    else
                    {
                        _lastStep--;
                    }

                    var numberOfSteps = Math.Ceiling(deltaManipulation / stepLength);
                    var newPosition = new CGPoint(indicator.Center.X + stepLength * numberOfSteps, indicator.Center.Y);

                    var pixelStep = (MaxValue - MinValue) / Frame.Width;

                    if (touchPoint.X >= 0 && touchPoint.X <= _background.Frame.Width-10)
                    {


                        if (recognizer == _leftIndicatorGesture)
                        {
                            
                            var newLeftValue = Round(MinValue + (pixelStep * newPosition.X));

                            if (newLeftValue >= RightValue)
                            {
                                return;
                            }
                        }
                        else if (recognizer == _rightIndicatorGesture)
                        {
                            var newRightValue = Round(MinValue + (pixelStep * newPosition.X));

                            if (newRightValue <= LeftValue)
                            {
                                return;
                            }
                        }


                        if (recognizer == _leftIndicatorGesture)
                        {
                            indicator.Center = newPosition;
                            touchArea.Center = newPosition;
                            var width = _rightIndicator.Center.X - _leftIndicator.Center.X;
                            _range.Frame = new CoreGraphics.CGRect(newPosition.X, _range.Frame.Y, width, _range.Frame.Height);
                        }
                        else if (recognizer == _rightIndicatorGesture)
                        {
                            indicator.Center = newPosition;
                            touchArea.Center = newPosition;
                            var width = _rightIndicator.Center.X - _leftIndicator.Center.X;
                            _range.Frame = new CoreGraphics.CGRect(_range.Frame.X, _range.Frame.Y, width, _range.Frame.Height);
                        }

                        

                        LeftValue = Round(MinValue + (pixelStep * _leftIndicator.Center.X));
                        RightValue = Round(MinValue + (pixelStep * _rightIndicator.Center.X));

                        if (ValueChanging != null)
                        {
                            ValueChanging(this, new EventArgs());
                        }
                    }
                }
            }
            else if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                if (ValueChanged != null)
                {
                    ValueChanged(this, new EventArgs());
                }

                _lastStep = 0;
            }
        }

        private double Round(double value)
        {
            var result = 0.0;

            var overflow = value % Step;


                result = value + Step - overflow;


            return result;
        }
    }
}