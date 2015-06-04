using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace Flipper.Controls
{
    public class Swiper : View
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create<Swiper, ObservableCollection<string>>(
            (p) => p.Source, null);

        public static readonly BindableProperty IsNearEndProperty =
            BindableProperty.Create<Swiper, ICommand>(
            (p) => p.IsNearEnd, null);

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create<Swiper, int>(
            (p) => p.SelectedIndex, -1);

        public static readonly BindableProperty SelectedUrlProperty =
            BindableProperty.Create<Swiper, string>(
            (p) => p.SelectedUrl, null);

        public static readonly BindableProperty NearEndThresholdProperty =
            BindableProperty.Create<Swiper, int>(
            (p) => p.NearEndThreshold, 3);

        public static readonly BindableProperty ImageLoadingTextProperty =
            BindableProperty.Create<Swiper, string>(
            (p) => p.ImageLoadingText, "Loading image...");

        /// <summary>
        /// The source of the images represented by a list of URLs.
        /// </summary>
        public ObservableCollection<string> Source
        {
            get { return (ObservableCollection<string>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Fires when the current displayed image is near the end of the list, giving you 
        /// a chance to load more images.
        /// </summary>
        public ICommand IsNearEnd
        {
            get { return (ICommand)GetValue(IsNearEndProperty); }
            set { SetValue(IsNearEndProperty, value); }
        }

        /// <summary>
        /// Gets or sets the index of the currently selected image. 
        /// </summary>
        /// <remarks>Returns -1 if not set</remarks>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the url of the currently selected image.
        /// </summary>
        /// <remarks>Must be a url present in the Source property or an exception will be thrown</remarks>
        public string SelectedUrl
        {
            get { return (string)GetValue(SelectedUrlProperty); }
            set { SetValue(SelectedUrlProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value of how near the end of the collection that the IsNearEnd command
        /// will fire.
        /// </summary>
        public int NearEndThreshold
        {
            get { return (int)GetValue(NearEndThresholdProperty); }
            set { 
                if(value<0)
                {
                    throw new ArgumentException("NearEndThreshold must be 0 or higher");
                }
                
                SetValue(NearEndThresholdProperty, value); }
        }

        public string ImageLoadingText
        {
            get { return (string)GetValue(ImageLoadingTextProperty); }
            set { SetValue(ImageLoadingTextProperty, value); }
        }
    }
}