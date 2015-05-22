using Flipper.Controls;
using Flipper.WinPhone.Renderers;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Panorama _root;

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

            _root = new Panorama();
            _root.SelectionChanged += panorama_SelectionChanged;
            _root.ManipulationCompleted += panorama_ManipulationCompleted;

            _root.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));

            if (Element.Source.Count > 1)
            {
                _root.Items.Add(new PanoramaItem() { Content = _prevImage });
            }

            _root.Items.Add(new PanoramaItem() { Content = _image });

            if (Element.Source.Count > 2)
            {
                _root.Items.Add(new PanoramaItem() { Content = _nextImage });
            }

            if (Element.Source.Count > 1)
            {
                _root.TabIndex = 1;
            }

            

            SetNativeControl(_root);
        }

        void panorama_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            var i = 42;
        }

        void panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = 42;
        }
    }
}
