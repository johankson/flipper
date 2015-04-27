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

[assembly: ExportRenderer (typeof(Swiper), typeof(SwiperRenderer))]

namespace Flipper.iOS
{

    public class SwiperRenderer : ViewRenderer<Swiper, UIView>
    {
        UIPanGestureRecognizer panGesture;
        UIView _rootView;
        UIImageView _centerImageView;
        UIImageView _leftImageView;
        UIImageView _rightImageView;
        UIViewAnimationOptions _animationOptions;

        private List<string> _imageUrls;
        private string _currentImageUrl;

        public SwiperRenderer()
        {
            _halfWidth = _width / 2;
            _halfHeight = _height / 2;
            _imageUrls = new List<string>();

            _imageUrls.Add("arrow.png");
            _imageUrls.Add("camel.jpg");
            _imageUrls.Add("plumber.jpg");

            _animationOptions = UIViewAnimationOptions.TransitionCrossDissolve;

            _currentImageUrl = _imageUrls.First();
        }

        nfloat dx = 0;
        nfloat dy = 0;
        nfloat startX;
        nfloat _width = 320f;
        nfloat _height = 320f;
        nfloat _halfWidth;
        nfloat _halfHeight;

        protected override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);

            _centerImageView = new UIImageView();
            _centerImageView.UserInteractionEnabled = true;
            _leftImageView = new UIImageView();
            _rightImageView = new UIImageView();

            panGesture = new UIPanGestureRecognizer(OnPan);
            _centerImageView.Frame = new CGRect(0, 0, _width, _height);
            _centerImageView.AddGestureRecognizer (panGesture);

            _leftImageView.Frame = new CGRect(0, 0, _width, _height);
            _leftImageView.Center = new CGPoint(-_halfWidth, _halfHeight);

            _rightImageView.Frame = new CGRect(0, 0, _width, _height);
            _rightImageView.Center = new CGPoint(_width + _halfWidth, _halfHeight);

            _rootView = new UIView();
            _rootView.ContentMode = UIViewContentMode.ScaleAspectFit;
            _rootView.AddSubview(_centerImageView);
            _rootView.AddSubview(_leftImageView);
            _rootView.AddSubview(_rightImageView);

            this.SetNativeControl(_rootView);

            InitializeImages();
        }

        private void InitializeImages()
        {
            if (!_imageUrls.Any())
            {
                _leftImageView.Image = null;
                _rightImageView.Image = null;
                _centerImageView.Image = null;
                _currentImageUrl = null;
                return;
            }

            if (_currentImageUrl == null)
            {
                _imageUrls.First();
            }

            var index = _imageUrls.IndexOf(_currentImageUrl);
            if (index > 0)
            {
                _leftImageView.Image = UIImage.FromFile(_imageUrls[index - 1]);
            }
            else
            {
                _leftImageView.Image = null;
            }

            if (index < _imageUrls.Count() - 1)
            {
                _rightImageView.Image = UIImage.FromFile(_imageUrls[index + 1]);
            }
            else
            {
                _rightImageView.Image = null;
            }

            _centerImageView.Image = UIImage.FromFile(_currentImageUrl);
        }

        private void OnPan(UIPanGestureRecognizer recognizer)
        {
            if ((recognizer.State == UIGestureRecognizerState.Began || 
                 recognizer.State == UIGestureRecognizerState.Changed) && (recognizer.NumberOfTouches == 1)) {

                var p0 = recognizer.LocationInView (this.NativeView);

                if(startX == 0)
                    startX = p0.X;

                if (dx == 0)
                    dx = p0.X - _centerImageView.Center.X;

                if (dy == 0)
                    dy = p0.Y - _centerImageView.Center.Y;

                var p1 = new CGPoint (p0.X - dx, _centerImageView.Center.Y);

                _centerImageView.Center = p1;

                _leftImageView.Center = new CGPoint(p1.X - _width, _halfHeight);
                _rightImageView.Center = new CGPoint(p1.X + _width, _halfHeight);

            } else if (recognizer.State == UIGestureRecognizerState.Ended) {
                dx = 0;
                dy = 0;

                var p0 = recognizer.LocationInView (this.NativeView);
                var p1 = p0.X - startX;
                startX = 0;

                var index = _imageUrls.IndexOf(_currentImageUrl);

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
                                _currentImageUrl = _imageUrls[index - 1];
                                InitializeImages();
                            }
                        );

                    }
                    else if (p1 < 0 && index < _imageUrls.Count() - 1)
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
                                _currentImageUrl = _imageUrls[index + 1];
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