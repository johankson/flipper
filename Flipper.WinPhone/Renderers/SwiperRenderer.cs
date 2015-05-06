using Flipper.Controls;
using Flipper.WinPhone.Renderers;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
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
           
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);

            if (_currentImage == 0)
            {
                _prevImage.Source = new BitmapImage(new Uri(Element.Source.Last()));
                _image.Source = new BitmapImage(new Uri(Element.Source.First()));
                _nextImage.Source = new BitmapImage(new Uri(Element.Source[1]));
            }

            var panorama = new Panorama();
            panorama.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));

            

            if (Element.Source.Count > 1)
            {
                panorama.Items.Add(_prevImage);
            }

            panorama.Items.Add(_image);

            if (Element.Source.Count > 2)
            {
                panorama.Items.Add(_nextImage);
            }

            if (Element.Source.Count > 1)
            {
                panorama.TabIndex = 1;
            }
            
            panorama.SelectionChanged += panorama_SelectionChanged;

            SetNativeControl(panorama);
        }

        void panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
