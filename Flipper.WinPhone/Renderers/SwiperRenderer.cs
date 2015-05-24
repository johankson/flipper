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
        private System.Windows.Controls.Image _image1 = new System.Windows.Controls.Image();
        private System.Windows.Controls.Image _image2 = new System.Windows.Controls.Image();
        private System.Windows.Controls.Image _image3 = new System.Windows.Controls.Image();
        private System.Windows.Controls.Image _image4 = new System.Windows.Controls.Image();
        private Panorama _root;
        private string _currentImageUrl;
        private bool _isInitializingImages;
        private string _lastImageUrl;
 
        protected override void OnElementChanged(ElementChangedEventArgs<Swiper> e)
        {
            base.OnElementChanged(e);

            _root = new Panorama();
            _root.SelectionChanged += panorama_SelectionChanged;

            _root.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));

            _root.Items.Add(new PanoramaItem() { Content = _image1 });
            _root.Items.Add(new PanoramaItem() { Content = _image2 });
            _root.Items.Add(new PanoramaItem() { Content = _image3 });
            _root.Items.Add(new PanoramaItem() { Content = _image4 });

            SetNativeControl(_root);

            InitializeImages();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Swiper.SourceProperty.PropertyName)
            {
                 InitializeImages();
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

                InitializeImages();
            }

            if(string.IsNullOrWhiteSpace(_lastImageUrl))
            {
                _lastImageUrl = _currentImageUrl;
            }
        }

        private void InitializeImages()
        {
            if (_isInitializingImages)
            {
                return;
            }

            try
            {
                _isInitializingImages = true;

                if (this.Element.Source == null)
                {
                    return;
                }

                if (!this.Element.Source.Any())
                {
                    _image1.Source = null;
                    _image2.Source = null;
                    _image3.Source = null;
                    _image4.Source = null;
                    return;
                }

                if (_currentImageUrl == null)
                {
                    _currentImageUrl = this.Element.Source.First();
                    _lastImageUrl = _currentImageUrl;
                }

                // Set some properties on the control
                var index = this.Element.Source.IndexOf(_currentImageUrl);
                this.Element.SelectedIndex = index;
                this.Element.SelectedUrl = _currentImageUrl;

                if (index > (this.Element.Source.Count - 1) - this.Element.NearEndTreshold && this.Element.IsNearEnd != null)
                {
                    if (this.Element.IsNearEnd.CanExecute(null))
                    {
                        this.Element.IsNearEnd.Execute(null);
                    }
                }

                // Set center image first
                LoadImage(1, _currentImageUrl);

                if (index > 0)
                {
                    LoadImage(0, this.Element.Source[index - 1]);
                }
                else
                {
                    LoadImage(0, string.Empty);
                }

                if (index < this.Element.Source.Count() - 1)
                {
                    LoadImage(2, this.Element.Source[index + 1]);
                }
                else
                {
                    LoadImage(2, string.Empty);
                }

                if (index < this.Element.Source.Count() - 2)
                {
                    LoadImage(3, this.Element.Source[index + 2]);
                }
                else
                {
                    LoadImage(3, string.Empty);
                }

                // TODO This will be the place to preload images into some sort of cache
            }
            finally
            {
                _isInitializingImages = false;
            }
        }


        /// <summary>
        /// Loads an image into a virtual slot
        /// </summary>
        /// <param name="virtualIndex">0 is the image to the left, 1 is the current 2 and 3 are on the right</param>
        /// <param name="url"></param>
        /// <remarks>
        /// Since the panorama have a rolling index from 0-3 we need to remap based on the current displaying index
        /// </remarks>
        private void LoadImage(int virtualIndex, string url)
        {
            // TODO add a reference collection instead to get rid of the lenghty switch?
            System.Windows.Controls.Image image;
            switch(CalculateIndex(virtualIndex))
            {
                case 0:
                    image = _image1;
                    break;
                case 1:
                    image = _image2;
                    break;
                case 2:
                    image = _image3;
                    break;
                case 3:
                    image = _image4;
                    break;
                default:
                    throw new Exception("This really shouldn't happen");
            }

            LoadImage(image, url);
        }

        private void LoadImage(System.Windows.Controls.Image image, string url)
        {
            if(string.IsNullOrWhiteSpace(url))
            {
                image.Source = null;
            }
            else
            {
                image.Source = new BitmapImage(new Uri(url));
            }

            // We use the tag to later identify what image we are showing.
            (image.Parent as PanoramaItem).Tag = url;
        }

        /// <summary>
        /// Calculates the actual image index based on the virtual index passed in
        /// </summary>
        /// <param name="virtualIndex">The virtual index</param>
        /// <returns>The actual index</returns>
        /// <remarks>
        /// Since we have four rotating PanoramaItems, each containing an Image control, the index 
        /// for the one showing differs from 0-3. The virtual index always represents the image with 1 in the middle
        /// to easily reference it.
        /// </remarks>
        private int CalculateIndex(int virtualIndex)
        {
            var selectedIndex = _root.SelectedIndex == -1 ? 0 : _root.SelectedIndex;
            var index = (virtualIndex - 1 + selectedIndex) % 4;
            if (index < 0)
            {
                index += 4;
            }

            return index;
        }

        void panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentImageUrl = (_root.SelectedItem as PanoramaItem).Tag as string;

            if (string.IsNullOrWhiteSpace(currentImageUrl))
            {
                var indexOfLast = Element.Source.IndexOf(_lastImageUrl);
                if (indexOfLast == 0)
                {
                    _currentImageUrl = Element.Source.First();
                }
                else
                {
                    _currentImageUrl = Element.Source.Last();
                }
            }
            else
            {
                _currentImageUrl = currentImageUrl;
            }

            _lastImageUrl = _currentImageUrl;
            InitializeImages();
        }
    }
}