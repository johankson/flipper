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
using Xamarin.Forms;
using CardViewFormsAndroid;
using Flipper.Controls;
using Xamarin.Forms.Platform.Android;
using Android.Support.V7.Widget;

[assembly: ExportRendererAttribute(typeof(CardContentView), typeof(CardContentViewRenderer))]
namespace CardViewFormsAndroid
{
    public class CardContentViewRenderer : CardView,
        IVisualElementRenderer
    {
        public CardContentViewRenderer()
            : base(Forms.Context)
        {
        }

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;

        bool init;
        ViewGroup packed;
        public void SetElement(VisualElement element)
        {
            var oldElement = this.Element;

            if (oldElement != null)
                oldElement.PropertyChanged -= HandlePropertyChanged;

            this.Element = element;
            if (this.Element != null)
            {
                //UpdateContent ();
                this.Element.PropertyChanged += HandlePropertyChanged;
            }

            ViewGroup.RemoveAllViews();
            //sizes to match the forms view
            //updates properties, handles visual element properties
            Tracker = new VisualElementTracker(this);

            Packager = new VisualElementPackager(this);
            Packager.Load();

            UseCompatPadding = true;

            SetContentPadding((int)TheView.Padding.Left, (int)TheView.Padding.Top,
                   (int)TheView.Padding.Right, (int)TheView.Padding.Bottom);

            Radius = TheView.CornderRadius;
            SetCardBackgroundColor(TheView.BackgroundColor.ToAndroid());






            if (ElementChanged != null)
                ElementChanged(this, new VisualElementChangedEventArgs(oldElement, this.Element));
        }




        public CardContentView TheView
        {
            get { return this.Element == null ? null : (CardContentView)Element; }
        }


        void HandlePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
            {

                //Packager.Load();

                Tracker.UpdateLayout();
            }
            else if (e.PropertyName == CardContentView.PaddingProperty.PropertyName)
            {
                SetContentPadding((int)TheView.Padding.Left, (int)TheView.Padding.Top,
                    (int)TheView.Padding.Right, (int)TheView.Padding.Bottom);
            }
            else if (e.PropertyName == CardContentView.CornerRadiusProperty.PropertyName)
            {
                this.Radius = TheView.CornderRadius;
            }
            else if (e.PropertyName == CardContentView.BackgroundColorProperty.PropertyName)
            {
                if (TheView.BackgroundColor != null)
                    SetCardBackgroundColor(TheView.BackgroundColor.ToAndroid());

            }
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            packed.Measure(widthConstraint, heightConstraint);

            //Measure child here and determine size
            return new SizeRequest(new Size(packed.MeasuredWidth, packed.MeasuredHeight));
        }

        public void UpdateLayout()
        {
            if (Tracker == null)
                return;

            Tracker.UpdateLayout();
        }

        public VisualElementTracker Tracker
        {
            get;
            private set;
        }

        public VisualElementPackager Packager
        {
            get;
            private set;
        }

        public Android.Views.ViewGroup ViewGroup
        {
            get { return this; }
        }

        public VisualElement Element
        {
            get;
            private set;
        }


    }
}
