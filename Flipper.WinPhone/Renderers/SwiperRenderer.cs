using Flipper.Controls;
using Flipper.WinPhone.Renderers;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(Swiper), typeof(SwiperRenderer))]
namespace Flipper.WinPhone.Renderers
{
    public class SwiperRenderer : ViewRenderer<Swiper, Panorama>
    {
        private int _currentImage = 0;

        private System.Windows.Controls.Image _prevImage = new System.Windows.Controls.Image();
        private System.Windows.Controls.Image _image = new System.Windows.Controls.Image();
        private System.Windows.Controls.Image _nextImage = new System.Windows.Controls.Image();

        public SwiperRenderer()
        {
            if(_currentImage == 0)
            {
                _prevImage.Source = new BitmapImage( new Uri(Element.Source.Last()));
                _image.Source = new BitmapImage(new Uri(Element.Source.First()));
                _nextImage.Source = new BitmapImage(new Uri(Element.Source[1]));
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);
            var panorama = new Panorama();
            panorama.Items.Add(_prevImage);
            panorama.Items.Add(_image);
            panorama.Items.Add(_nextImage);

            SetNativeControl(panorama);
        }
    }
}
