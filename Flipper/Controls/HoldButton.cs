using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Flipper.Controls
{
    /// <summary>
    /// A control that needs the user to hold for a specificed amount of time.
    /// </summary>
    public class HoldButton : View
    {
        public static readonly BindableProperty PressCanceledProperty =
            BindableProperty.Create<HoldButton, ICommand>((p) => p.PressCanceled, null);

        public static readonly BindableProperty PressResumedProperty =
            BindableProperty.Create<HoldButton, ICommand>((p) => p.PressResumed, null);

        public static readonly BindableProperty PressCompletedProperty =
            BindableProperty.Create<HoldButton, ICommand>((p) => p.PressCompleted, null);

        public static readonly BindableProperty TransitionValueProperty =
            BindableProperty.Create<HoldButton, float>((p) => p.TransitionValue, 0);

        public static readonly BindableProperty DelayProperty =
            BindableProperty.Create<HoldButton, float>((p) => p.Delay, 2);

        public static readonly BindableProperty ColorProperty =
            BindableProperty.Create<HoldButton, Color>((p) => p.Color, Color.Teal);

        public static readonly BindableProperty ProgressColorProperty =
            BindableProperty.Create<HoldButton, Color>((p) => p.ProgressColor, Color.FromRgb(0, 100, 100));

        /// <summary>
        /// The press was cancelled
        /// </summary>
        public ICommand PressCanceled
        {
            get { return (ICommand)GetValue(PressCanceledProperty); }
            set { SetValue(PressCanceledProperty, value); }
        }

        /// <summary>
        /// If the press was cancelled and the resumed, this is called.
        /// </summary>
        public ICommand PressResumed
        {
            get { return (ICommand)GetValue(PressResumedProperty); }
            set { SetValue(PressResumedProperty, value); }
        }

        /// <summary>
        /// Fires when the press has exceeded the specified delay. Default at two (2) seconds.
        /// </summary>
        public ICommand PressCompleted
        {
            get { return (ICommand)GetValue(PressCompletedProperty); }
            set { SetValue(PressCompletedProperty, value); }
        }

        /// <summary>
        /// A value that represents the transition between 0 and 1 to indicate how far the progress has gone.
        /// </summary>
        /// <remarks>
        /// Use this property to control other stuff that are dependent on the transition. Like changing
        /// the background color.
        /// </remarks>
        public float TransitionValue
        {
            get { return (float)GetValue(TransitionValueProperty); }
            set { SetValue(TransitionValueProperty, value); }
        }

        /// <summary>
        /// The number of seconds you need to hold the button before PressCompleted is called.
        /// </summary>
        public float Delay
        {
            get { return (float)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }

        /// <summary>
        /// The color of the circle.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// The color of the progress arc.
        /// </summary>
        public Color ProgressColor
        {
            get { return (Color)GetValue(ProgressColorProperty); }
            set { SetValue(ProgressColorProperty, value); }
        }
    }
}