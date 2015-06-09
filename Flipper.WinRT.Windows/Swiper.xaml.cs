using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Flipper.WinRT
{
    public sealed partial class Swiper
    {
        private TranslateTransform transform0, transform2, transform;
        private int _previousIndex = -1;
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _previousIndex = _selectedIndex;
                _selectedIndex = value;

                if(SelectedIndexChanged != null)
                {
                    SelectedIndexChanged(this, new EventArgs());
                }
            }
        }
        public event EventHandler SelectedIndexChanged;

         public static readonly DependencyProperty ImagesProperty =
    DependencyProperty.Register(
    "Images", typeof(ObservableCollection<string>),
    typeof(Swiper), new PropertyMetadata(new ObservableCollection<string>(),new PropertyChangedCallback((DependencyObject sender, DependencyPropertyChangedEventArgs args) =>{
        var swiper = (Swiper)sender;
        
        if(swiper.Images.Any())
        {
            swiper.UpdateImages();
        }

        swiper.Images.CollectionChanged += swiper._images_CollectionChanged;  
    })));

        private ObservableCollection<string> _images;
        public ObservableCollection<string> Images
        {
            get { return GetValue(ImagesProperty) as ObservableCollection<string>;}
            set { SetValue(ImagesProperty, value); }

        }

        protected void _images_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateImages();
        }

        public Swiper()
        {
            this.InitializeComponent();

            transform = (Main.RenderTransform = (Main.RenderTransform as TranslateTransform) ?? new TranslateTransform()) as TranslateTransform;

            transform0 = new TranslateTransform();
            Image0.RenderTransform = transform0;
            Image0.SizeChanged += Image0_SizeChanged;
            

            transform2 = new TranslateTransform();
            Image2.RenderTransform = transform2;
            Image2.SizeChanged += Image2_SizeChanged;
            

            Panel.ManipulationMode = ManipulationModes.TranslateX;
            Panel.ManipulationDelta += Main_ManipulationDelta;
            Panel.ManipulationCompleted += Main_ManipulationCompleted;

            Images = new ObservableCollection<string>();
        }

        void Main_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X < -200 && SelectedIndex < Images.Count - 1)
            {
                var story = new Storyboard()
                {
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250)),
                };

                story.Children.Add(CreateAnimation(transform.X, -Panel.ActualWidth, transform));
                story.Children.Add(CreateAnimation(transform2.X, 0, transform2));

                story.Completed += story_Completed;

                story.Begin();

                SelectedIndex++;
            }

            else if (e.Cumulative.Translation.X > 200 && SelectedIndex > 0)
            {
                var story = new Storyboard()
                {
                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250)),
                };

                story.Children.Add(CreateAnimation(transform.X, +Panel.ActualWidth, transform));
                story.Children.Add(CreateAnimation(transform0.X, 0, transform0));

                story.Completed += story_Completed;

                story.Begin();

                SelectedIndex--;

            }
            else
            {
                UpdateTransform();
            }
        }

        void story_Completed(object sender, object e)
        {
            UpdateImages();
        }

        void Image2_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            transform2.X = (Panel.ActualWidth/2)+(Image2.ActualWidth/2);
        }

        void Image0_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            transform0.X = -(Panel.ActualWidth / 2) - (Image0.ActualWidth / 2);
        }

        void Main_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            transform.X += e.Delta.Translation.X;
            transform0.X += e.Delta.Translation.X;
            transform2.X += e.Delta.Translation.X;     
        }

        private DoubleAnimation CreateAnimation(double from, double to, TranslateTransform target)
        {
            var animation = new DoubleAnimation()
            {
                From = target.X,
                To = to,
                Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250))
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, "X");

            return animation;
        }

        internal void UpdateImages()
        {
            if (_previousIndex != SelectedIndex)
            {
                if(_previousIndex < _selectedIndex)
                {
                    if (Image2.Source != null)
                    {
                        Main.Source = Image2.Source;
                    }
                    else if(Images.Count > 0)
                    {
                        Main.Source = new BitmapImage(new Uri(Images[SelectedIndex]));
                    }
                }
                else if(_previousIndex > _selectedIndex)
                {
                    Main.Source = Image0.Source;
                }
                //Main.Source = new BitmapImage(new Uri(Images[SelectedIndex]));

                if (Images.Count > 0 && SelectedIndex > 0)
                {
                    Image0.Source = new BitmapImage(new Uri(Images[SelectedIndex - 1]));
                }

                if (SelectedIndex == 0)
                {
                    Image0.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    Image0.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                }

                if (SelectedIndex < Images.Count - 1)
                {
                    Image2.Source = new BitmapImage(new Uri(Images[SelectedIndex + 1]));
                }


                if (SelectedIndex == Images.Count - 1)
                {
                    Image2.Visibility = global::Windows.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    Image2.Visibility = global::Windows.UI.Xaml.Visibility.Visible;
                }

                UpdateTransform();
            }
        }

        private void UpdateTransform()
        {
            transform.X = 0;
            transform0.X = -(Panel.ActualWidth / 2) - (Image0.ActualWidth / 2);
            transform2.X = (Panel.ActualWidth / 2) + (Image2.ActualWidth / 2);
        }
    }
}
