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
    /// 
    /// </summary>
    public class HoldButton : View
    {
        public static readonly BindableProperty PressCanceledProperty =
           BindableProperty.Create<HoldButton, ICommand>((p) => p.PressCanceled, null);

        public static readonly BindableProperty PressCompletedProperty =
            BindableProperty.Create<HoldButton, ICommand>((p) => p.PressCompleted, null);

        public static readonly BindableProperty TransitionValueProperty =
            BindableProperty.Create<HoldButton, float>((p) => p.TransitionValue, 0);

        public static readonly BindableProperty DelayProperty =
            BindableProperty.Create<HoldButton, float>((p) => p.Delay, 2);

        public ICommand PressCanceled
        {
            get { return (ICommand)GetValue(PressCanceledProperty); }
            set { SetValue(PressCanceledProperty, value); }
        }

        public ICommand PressCompleted
        {
            get { return (ICommand)GetValue(PressCompletedProperty); }
            set { SetValue(PressCompletedProperty, value); }
        }

        public float TransitionValue
        {
            get { return (float)GetValue(TransitionValueProperty); }
            set { SetValue(TransitionValueProperty, value); }
        }

        public float Delay
        {
            get { return (float)GetValue(DelayProperty); }
            set { SetValue(DelayProperty, value); }
        }
    }
}
