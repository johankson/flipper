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

[assembly: ExportRenderer(typeof(Swiper), typeof(SwiperRenderer))]

namespace Flipper.Droid.Renderers
{
    public class SwiperRenderer : ViewRenderer<Swiper, View>
    {
        private View _rootView;
        private ImageView _centerImageView;
        private ImageView _leftImageView;
        private ImageView _rightImageView;
        private string _currentImageUrl;
        public SwiperRenderer()
        {
            this.SetWillNotDraw(false);
        }
        protected override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);

            _centerImageView = CreateImageView();

            // UpdateSizes();

            //_rootView = new RelativeLayout(Context);
            _rootView = new View(Context);

            //RelativeLayout.LayoutParams prms = new RelativeLayout.LayoutParams(30, 40);
            //prms.LeftMargin = 50;
            //prms.TopMargin = 60;
            //_rootView.AddView(_centerImageView, prms);

            SetNativeControl(_rootView);
           

            InitializeImages();
        }

        private void InitializeImages()
        {
            if (this.Element.Source == null)
            {
                return;
            }

            if (!this.Element.Source.Any())
            {
                _leftImageView.SetImageBitmap(null); // Image = null;
                _rightImageView.SetImageBitmap(null); //.Image = null;
                _centerImageView.SetImageBitmap(null); //.Image = null;
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

            // _centerImageView.SetImageBitmap(ResolveImage(_currentImageUrl)); 
            //Koush.UrlImageViewHelper.SetUrlDrawable(_centerImageView, _currentImageUrl, new cb(
            //    () =>
            //    {
            //        //Device.BeginInvokeOnMainThread(
            //        //    () =>
            //        //    {
            //        //        _rootView.Invalidate();
            //        //    });
            //        //  _rootView.PostInvalidate();
            //        this.Invalidate();
            //    }));

            //var r = Resources.GetDrawable("arrow");
            _centerImageView.SetImageResource(Resource.Drawable.arrow);
        }

        class cb : Java.Lang.Object, Koush.IUrlImageViewCallback
        {
            public cb(Action loadedAction)
            {
                ImageLoaded = loadedAction;
            }

            public void OnLoaded(ImageView p0, Android.Graphics.Bitmap p1, string p2, bool p3)
            {
                if(ImageLoaded!=null)
                {
                    ImageLoaded();
                }
            }

            public Action ImageLoaded { get; set; }
        }

        public override void Draw(Android.Graphics.Canvas canvas)
        {
            Bitmap bitmap = ((BitmapDrawable)_centerImageView.Drawable).Bitmap;

            var width = this.Width;
            var height = this.Height;
            var dstRect = new Rect(0, 0, width, height);

            // Calculate the new bounds
            var rect = CalculateLargestRect(bitmap);
            var dest = CalculateCentrationRect(rect);

            bitmap = ResizeBitmap(bitmap, rect.Width(), rect.Height());
            canvas.DrawBitmap(bitmap, dest.Left, dest.Top, null);
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
    }
}