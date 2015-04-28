using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using Flipper;
using Flipper.iOS;
using System.Drawing;
using Flipper.Controls;
using CoreGraphics;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Foundation;

[assembly: ExportRenderer (typeof(Swiper), typeof(SwiperRenderer))]

namespace Flipper.iOS
{

    public class SwiperRenderer : ViewRenderer<Swiper, UIView>
    {
        private UIView _rootView;
        private UIImageView _centerImageView;
        private UIImageView _leftImageView;
        private UIImageView _rightImageView;
        private UIViewAnimationOptions _animationOptions;
        private string _currentImageUrl;
        private nfloat dx = 0;
        private nfloat dy = 0;
        private nfloat startX;
        private nfloat _width = 320;
        private nfloat _height = 320f;
        private nfloat _halfWidth;
        private nfloat _halfHeight;

        public SwiperRenderer()
        {
            _animationOptions = UIViewAnimationOptions.TransitionNone;
        }

        private UIImageView CreateImageView()
        {
            return new UIImageView()
            {
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);

            _leftImageView = CreateImageView();
            _rightImageView = CreateImageView();

            _centerImageView = CreateImageView();
            _centerImageView.UserInteractionEnabled = true;
            _centerImageView.AddGestureRecognizer (new UIPanGestureRecognizer(OnPan));

            UpdateSizes();

            _rootView = new UIView();
            _rootView.ContentMode = UIViewContentMode.ScaleAspectFit;
            _rootView.AddSubview(_centerImageView);
            _rootView.AddSubview(_leftImageView);
            _rootView.AddSubview(_rightImageView);

            this.SetNativeControl(_rootView);

            InitializeImages();
        }

        /// <summary>
        /// Recalculates sizes
        /// </summary>
        private void UpdateSizes()
        {
            if (this.Element.Width > 0 &&
                this.Element.Height > 0)
            {
                _width = (nfloat)this.Element.Width;
                _halfWidth = _width / 2;

                _height = (nfloat)this.Element.Height;
                _halfHeight = _height / 2;

                _leftImageView.Frame = new CGRect(0, 0, _width, _height);
                _leftImageView.Center = new CGPoint(-_halfWidth, _halfHeight);

                _rightImageView.Frame = new CGRect(0, 0, _width, _height);
                _rightImageView.Center = new CGPoint(_width + _halfWidth, _halfHeight);
                _centerImageView.Frame = new CGRect(0, 0, _width, _height);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if(e.PropertyName == Swiper.SourceProperty.PropertyName)
            {
                InitializeImages();
            }

            if(e.PropertyName == Swiper.WidthProperty.PropertyName ||
               e.PropertyName == Swiper.HeightProperty.PropertyName)
            {
                UpdateSizes();
            }
        }

        private void InitializeImages()
        {
            if(this.Element.Source == null)
            {
                return;
            }

            if (!this.Element.Source.Any())
            {
                _leftImageView.Image = null;
                _rightImageView.Image = null;
                _centerImageView.Image = null;
                _currentImageUrl = null;
                return;
            }

            if (_currentImageUrl == null)
            {
                _currentImageUrl = this.Element.Source.First();
            }

            var index = this.Element.Source.IndexOf(_currentImageUrl);
            if (index > 0)
            {
                _leftImageView.Image = ResolveImage(this.Element.Source[index - 1]);
            }
            else
            {
                _leftImageView.Image = null;
            }

            if (index < this.Element.Source.Count() - 1)
            {
                _rightImageView.Image = ResolveImage(this.Element.Source[index + 1]);
            }
            else
            {
                _rightImageView.Image = null;
            }

            _centerImageView.Image = ResolveImage(_currentImageUrl);
        }

        /// <summary>
        /// Resolves the source into an UIImage
        /// </summary>
        /// <param name="source">An URL or a resource name</param>
        /// <returns>A UIImage</returns>
        private UIImage ResolveImage(string source)
        {
            if(source.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
            {
                using (var url = new NSUrl(source))
                {
                    using (var data = NSData.FromUrl(url))
                    {
                        return UIImage.LoadFromData(data);
                    }
                }
            }
            else
            {
                return UIImage.FromFile(source);
            }
        }

        private void OnPan(UIPanGestureRecognizer recognizer)
        {
            if ((recognizer.State == UIGestureRecognizerState.Began || 
                 recognizer.State == UIGestureRecognizerState.Changed) && (recognizer.NumberOfTouches == 1)) {

                var p0 = recognizer.LocationInView (this.NativeView);

                if (startX == 0)
                {
                    startX = p0.X;
                }

                if (dx == 0)
                {
                    dx = p0.X - _centerImageView.Center.X;
                }

                if (dy == 0)
                {
                    dy = p0.Y - _centerImageView.Center.Y;
                }

                var p1 = new CGPoint (p0.X - dx, _centerImageView.Center.Y);

                _centerImageView.Center = p1;

                _leftImageView.Center = new CGPoint(p1.X - _width, _halfHeight);
                _rightImageView.Center = new CGPoint(p1.X + _width, _halfHeight);

            } 
            else if (recognizer.State == UIGestureRecognizerState.Ended)
            {
                dx = 0;
                dy = 0;

                var p0 = recognizer.LocationInView (this.NativeView);
                var p1 = p0.X - startX;
                startX = 0;

                var index = this.Element.Source.IndexOf(_currentImageUrl);

                if (Math.Abs(p1) > 30)
                {
                    if (p1 > 0 && index > 0)
                    {
                        Animate(0.2, 0, _animationOptions,
                            () =>
                            {
                                _centerImageView.Center = new CGPoint(_width + _halfWidth, _halfHeight);
                                _leftImageView.Center = new CGPoint(_width - _halfWidth, _halfHeight);
                                _rightImageView.Center = new CGPoint(_width + _width + _halfWidth, _halfHeight);
                            }
                            , 
                            () =>
                            {
                                MoveImagesToOrigin();
                                _currentImageUrl = this.Element.Source[index - 1];
                                InitializeImages();
                            }
                        );

                    }
                    else if (p1 < 0 && index < this.Element.Source.Count() - 1)
                    {
                        Animate(0.2, 0, _animationOptions,
                            () =>
                            {
                                _centerImageView.Center = new CGPoint(_halfWidth - _width, _halfHeight);
                                _leftImageView.Center = new CGPoint(-_halfWidth - _width, _halfHeight);
                                _rightImageView.Center = new CGPoint(_halfWidth, _halfHeight);
                            }
                            , 
                            () =>
                            {
                                MoveImagesToOrigin();
                                _currentImageUrl = this.Element.Source[index + 1];
                                InitializeImages();
                            });
                    }
                    else
                    {
                        Animate(0.2, 0, _animationOptions,
                            MoveImagesToOrigin, 
                            null);
                    }
                }
                else
                {
                    Animate(0.2, 0, _animationOptions,
                        MoveImagesToOrigin, 
                        null);
                }
            }
        }

        private void MoveImagesToOrigin()
        {
            _centerImageView.Center = new CGPoint(_halfWidth, _halfHeight);
            _leftImageView.Center = new CGPoint(-_halfWidth, _halfHeight);
            _rightImageView.Center = new CGPoint(_width + _halfWidth, _halfHeight);
        }
    }
}