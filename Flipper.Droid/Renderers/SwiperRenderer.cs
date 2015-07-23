using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Flipper.Controls;
using Flipper.Droid.Renderers;
using Xamarin.Forms.Platform.Android;
using RelativeLayout = Android.Widget.RelativeLayout;
using View = Android.Views.View;
using Xamarin.Forms;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using System.Net.Http;
using System.Threading.Tasks;
using Java.IO;
using ModernHttpClient;
using Android.Animation;

[assembly: ExportRenderer(typeof(Swiper), typeof(SwiperRenderer))]

namespace Flipper.Droid.Renderers
{
    public class SwiperRenderer : ViewRenderer<Swiper, View>
    {
        private View _rootView;
        private AsyncImageLoader _centerBitmap = null;
        private AsyncImageLoader _leftBitmap = null;
        private AsyncImageLoader _rightBitmap = null;
       
        private string _currentImageUrl;
        private float _width;
        private float _halfWidth;
        private float _height;
        private float _halfHeight;
        
        public SwiperRenderer()
        {
            this.SetWillNotDraw(false);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);
            
            UpdateSizes();

            _rootView = new View(Context);
            SetNativeControl(_rootView);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Swiper.SourceProperty.PropertyName)
            {
                InitializeImages();
            }

            if (e.PropertyName == Swiper.WidthProperty.PropertyName || e.PropertyName == Swiper.HeightProperty.PropertyName)
            {
                UpdateSizes();
            }

            if (e.PropertyName == Swiper.SelectedIndexProperty.PropertyName)
            {
                // TODO Check for index overrun
                if (this.Element.SelectedIndex > 0 &&
                    _currentImageUrl != this.Element.Source[this.Element.SelectedIndex])
                {
                    _currentImageUrl = this.Element.Source[this.Element.SelectedIndex];
                    InitializeImages();
                }
            }

            if (e.PropertyName == Swiper.SelectedUrlProperty.PropertyName)
            {
                if (!string.IsNullOrWhiteSpace(this.Element.SelectedUrl) &&
                    _currentImageUrl != this.Element.SelectedUrl)
                {
                    _currentImageUrl = this.Element.SelectedUrl;
                    InitializeImages();
                }
            }

            if (e.PropertyName == "Renderer")
            {
                if (!String.IsNullOrWhiteSpace(this.Element.SelectedUrl))
                {
                    _currentImageUrl = this.Element.SelectedUrl;
                }
                else if (this.Element.SelectedIndex > 0)
                {
                    _currentImageUrl = this.Element.Source[this.Element.SelectedIndex];
                }
            }
        }

        private void InitializeImages()
        {
            if (this.Element.Source == null)
            {
                return;
            }

            if (!this.Element.Source.Any())
            {
                // TODO Add a placeholder bitmap for empty ones?
                _leftBitmap = null;
                _rightBitmap = null;
                _centerBitmap = null;

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
                _leftBitmap = ResolveImage(this.Element.Source[index - 1]);
            }
            else
            {
                _leftBitmap = null;
            }

            if (index < this.Element.Source.Count() - 1)
            {
                _rightBitmap = ResolveImage(this.Element.Source[index + 1]);
            }
            else
            {
                _rightBitmap = null;
            }

            _centerBitmap = ResolveImage(_currentImageUrl);
            Invalidate();

            this.Element.SelectedIndex = index;
            this.Element.SelectedUrl = _currentImageUrl;

            if (index > (this.Element.Source.Count - 1) - this.Element.NearEndThreshold && this.Element.IsNearEnd != null)
            {
                if (this.Element.IsNearEnd.CanExecute(null))
                {
                    this.Element.IsNearEnd.Execute(null);
                }
            }
        }

        /// <summary>
        /// Resolves the image from the given uri. Also handles
        /// caching and resizing... It's a magic function.
        /// </summary>
        /// <param name="url">The URL to the image</param>
        /// <returns>A resized, nice bitmap</returns>
        private AsyncImageLoader ResolveImage(string url)
        {
            if (_cache.ContainsKey(url))
            {
                return _cache[url];
            }

            // Resize and assign the bitmap
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                var loader = new AsyncImageLoader(url);
                loader.Completed = (a) =>
                {
                    var rect = CalculateLargestRect(a.Bitmap);
                    loader.Bitmap = ResizeBitmap(a.Bitmap, rect.Width(), rect.Height());
                    Device.BeginInvokeOnMainThread(() => Invalidate());
                };

                lock (_cache)
                {
                    if (!_cache.ContainsKey(url))
                    {
                        _cache.Add(url, loader);
                    }
                }

                return loader;
            }
        }

        bool _reinitializeImages = true;

        /// <summary>
        /// Quick fix cache (replace with something better if needed)
        /// </summary>
        private Dictionary<string, AsyncImageLoader> _cache = new Dictionary<string, AsyncImageLoader>();

        public override void Draw(Android.Graphics.Canvas canvas)
        {
            if(_reinitializeImages)
            {
                InitializeImages();
                _reinitializeImages = false;
            }

            UpdateSizes();

            // Clear the canvas
            canvas.DrawARGB(255, 255, 255, 255);

            if(_centerBitmap != null && _centerBitmap.Bitmap != null)
            {
                var dest = CalculateCentrationRect(_centerBitmap.Bitmap);
                 canvas.DrawBitmap(_centerBitmap.Bitmap, dest.Left + _swipeCurrectXOffset, dest.Top, null);
            }
            else if (_centerBitmap != null)
            {
                DrawLoadingText(canvas, 0);
            }

            if (_leftBitmap != null && _leftBitmap.Bitmap != null)
            {
                var dest = CalculateCentrationRect(_leftBitmap.Bitmap);
                canvas.DrawBitmap(_leftBitmap.Bitmap, dest.Left + _swipeCurrectXOffset - this.Width, dest.Top, null);
            }
            else if (_leftBitmap != null)
            {
                DrawLoadingText(canvas, -Width);
            }

            if (_rightBitmap != null && _rightBitmap.Bitmap != null)
            {
                var dest = CalculateCentrationRect(_rightBitmap.Bitmap);
                canvas.DrawBitmap(_rightBitmap.Bitmap, dest.Left + _swipeCurrectXOffset + this.Width, dest.Top, null);
            }
            else if (_rightBitmap != null)
            {
                DrawLoadingText(canvas, Width);
            }
        }

        private void DrawLoadingText(Android.Graphics.Canvas canvas, float offset)
        {
            // TODO Perhaps replace with an animation later on?
            Paint paint = new Paint();
            paint.Color = Xamarin.Forms.Color.Black.ToAndroid();
            paint.SetStyle(Paint.Style.Fill);
            paint.TextSize = 50;
            var textWidth = paint.MeasureText(Element.ImageLoadingText);

            canvas.DrawText(Element.ImageLoadingText, this._halfWidth - (textWidth / 2) + _swipeCurrectXOffset + offset, this._halfHeight, paint);
        }

        /// <summary>
        /// Calculates the destination rect to center the image in 
        /// the available space.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private Rect CalculateCentrationRect(Rect src)
        {
            if(src.Width() > src.Height())
            {
                return new Rect(0, this.Height / 2 - src.Height() / 2, src.Width(), this.Height / 2 + src.Height() / 2);
            }
            else
            {
                return new Rect(this.Width / 2 - src.Width() / 2, 0, this.Width / 2 + src.Width() / 2, src.Height());
            }
        }

        private Rect CalculateCentrationRect(Bitmap bitmap)
        {
            var rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            return CalculateCentrationRect(rect);
        }

        /// <summary>
        /// Calculates the largest rect possible to scale the image to for it
        /// to be totally visible on screen
        /// </summary>
        /// <param name="bitmap">The bitmap to fit</param>
        /// <returns></returns>
        private Rect CalculateLargestRect(Bitmap bitmap)
        {
            var bitmapRatio = (float)bitmap.Width / (float)bitmap.Height;
            var canvasRatio = (float)this.Width / (float)this.Height;

            double widthfactor = 0, heightfactor = 0;
            int scaleFactor = 1;
            double resizefactor = 1;

            widthfactor = (double)bitmap.Width / (double)this.Width;
            heightfactor = (double)bitmap.Height / (double)this.Height;

            if (widthfactor > heightfactor)
            {
                resizefactor = widthfactor;
                scaleFactor = (int)resizefactor;
            }
            else if (heightfactor > widthfactor)
            {
                resizefactor = heightfactor;
                scaleFactor = (int)resizefactor;
            }

            double W = bitmap.Width / resizefactor;
            double H = bitmap.Height / resizefactor;

            return new Rect(0, 0, (int)W, (int)H);
        }

        /// <summary>
        /// Creates a new sized bitmap 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        /// <returns></returns>
        public Bitmap ResizeBitmap(Bitmap bitmap, int newWidth, int newHeight)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;
            
            Matrix matrix = new Matrix();
            matrix.PostScale(scaleWidth, scaleHeight);
            
            Bitmap resizedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, width, height, matrix, false);
            return resizedBitmap;
        }

        private float _swipeStartX = 0f;
        private float _swipeCurrectXOffset = 0f;

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch(e.Action)
            {
                case MotionEventActions.Down:
                    _swipeStartX = e.GetX();
                    return true;

                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                  //  _swipeCurrectXOffset = 0f;

                    var index = this.Element.Source.IndexOf(_currentImageUrl);
                    
                    if(Math.Abs(_swipeCurrectXOffset)>30) // TODO Add a variable for the trigger offset?
                    {
                        if(_swipeCurrectXOffset > 0 && index > 0)
                        {
                            // Left swipe
                            AnimateLeft(index);
                        }
                        else if (_swipeCurrectXOffset < 0 && index < this.Element.Source.Count() -1)
                        {
                            // Right swipe
                            AnimateRight(index);
                        }
                        else
                        {
                            AnimateBackToStart();
                        }
                    }
                    else
                    {
                        AnimateBackToStart();
                    }
                    
                    return true;

                case MotionEventActions.Move:
                    _swipeCurrectXOffset = e.GetX() - _swipeStartX;
                    Invalidate();
                    return true;
            }

            return base.OnTouchEvent(e);
        }

        private void AnimateLeft(int index)
        {
            var animator = ValueAnimator.OfFloat(_swipeCurrectXOffset, this.Width);
            animator.Start();

            animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs args) =>
            {
                _swipeCurrectXOffset = (float)args.Animation.AnimatedValue;
                Invalidate();
            };
            animator.AnimationEnd += (object sender, EventArgs args) =>
            {
                _swipeCurrectXOffset = 0f;
                _currentImageUrl = this.Element.Source[index - 1];
                InitializeImages();
            };
        }

        private void AnimateRight(int index)
        {
            var animator = ValueAnimator.OfFloat(_swipeCurrectXOffset, -(this.Width));
            animator.SetDuration(200);
            animator.Start();

            animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs args) =>
            {
                _swipeCurrectXOffset = (float)args.Animation.AnimatedValue;
                Invalidate();
            };
            animator.AnimationEnd += (object sender, EventArgs args) =>
            {
                _swipeCurrectXOffset = 0f;
                _currentImageUrl = this.Element.Source[index + 1];
                InitializeImages();
            };
        }

        private void AnimateBackToStart()
        {
            var animator = ValueAnimator.OfFloat(_swipeCurrectXOffset, 0);
            animator.SetDuration(200);
            animator.Start();

            animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs args) =>
            {
                float newValue = (float)args.Animation.AnimatedValue;
                _swipeCurrectXOffset = newValue;
                Invalidate();
            };
        }

        private void UpdateSizes()
        {
            if (this.Element == null)
            {
                return;
            }

            if (this.Width > 0 && this.Height > 0)
            {
                _width = this.Width;
                _halfWidth = _width / 2;

                _height = this.Height;
                _halfHeight = _height / 2;
            }
        }
    }
}