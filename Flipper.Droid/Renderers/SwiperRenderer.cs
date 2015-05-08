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

[assembly: ExportRenderer(typeof(Swiper), typeof(SwiperRenderer))]

namespace Flipper.Droid.Renderers
{
    public class SwiperRenderer : ViewRenderer<Swiper, View>
    {
        private View _rootView;

        private Bitmap _centerBitmap = null;
        private Bitmap _leftBitmap = null;
        private Bitmap _rightBitmap = null;
     
        private string _currentImageUrl;
        public SwiperRenderer()
        {
            this.SetWillNotDraw(false);
        }
        protected async override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);

         //   _centerImageView = CreateImageView();

            // UpdateSizes();

            //_rootView = new RelativeLayout(Context);
            _rootView = new View(Context);

            //RelativeLayout.LayoutParams prms = new RelativeLayout.LayoutParams(30, 40);
            //prms.LeftMargin = 50;
            //prms.TopMargin = 60;
            //_rootView.AddView(_centerImageView, prms);

            SetNativeControl(_rootView);
           

            await InitializeImages();
        }

        private async Task InitializeImages()
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
            /*     if (index > 0)
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
                 } */

                 _centerBitmap = await ResolveImage(_currentImageUrl);
                 Invalidate();
           
       //     _centerImageView.SetImageResource(Resource.Drawable.arrow);
           
        }

        /// <summary>
        /// Resolves the image from the given uri. Also handles
        /// caching and resizing... It's a magic function.
        /// </summary>
        /// <param name="url">The URL to the image</param>
        /// <returns>A resized, nice bitmap</returns>
        private async Task<Bitmap> ResolveImage(string url)
        {
            // Resize and assign the bitmap
            // TODO Cache here
            // TODO Figure out how to handle slow downloads
            using(var client = new HttpClient())
            {
                try
                {
                    var stream = await client.GetStreamAsync(new Uri(url));
                    var bitmap = await BitmapFactory.DecodeStreamAsync(stream);
                    var rect = CalculateLargestRect(bitmap);
                    return ResizeBitmap(bitmap, rect.Width(), rect.Height());
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
        }
        
        public override void Draw(Android.Graphics.Canvas canvas)
        {
            //if(_centerBitmap == null)
            //{
            //    // Resize and assign the bitmap
            //    // TODO Work in progress
            //    Bitmap bitmap = ((BitmapDrawable)_centerImageView.Drawable).Bitmap;
            //    var rect = CalculateLargestRect(bitmap);
            //    _centerBitmap = ResizeBitmap(bitmap, rect.Width(), rect.Height());
            //}

            if(_centerBitmap != null)
            {
                var dest = CalculateCentrationRect(_centerBitmap);
                canvas.DrawBitmap(_centerBitmap, dest.Left + _swipeCurrectXOffset, dest.Top, null);
            }
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

            if (bitmap.Width > this.Width)
            {
                widthfactor = (double)bitmap.Width / (double)this.Width;
            }
            if (bitmap.Height > this.Height)
            {
                heightfactor = (double)bitmap.Height / (double)this.Height;
            }

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

            if (heightfactor != 0 || widthfactor != 0)
            {
                double W = bitmap.Width / resizefactor;
                double H = bitmap.Height / resizefactor;

                return new Rect(0, 0, (int)W, (int)H);
            }

            throw new Exception("That's not right?");
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

        //private Android.Graphics.Bitmap ResolveImage(string source)
        //{
           
        //}

        private ImageView CreateImageView()
        {
            return new ImageView(Context)
                {
                     
                };
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
                    _swipeCurrectXOffset = 0f;
                    Invalidate();
                    return true;

                case MotionEventActions.Move:
                    _swipeCurrectXOffset = e.GetX() - _swipeStartX;
                    Invalidate();
                    return true;
            }

            return base.OnTouchEvent(e);
        }
    }
}