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
using Android.Graphics;

namespace Flipper.Droid
{
    /// <summary>
    /// Represents a bitmap that can load an image async
    /// </summary>
    class AsyncImageView : ImageView
    {

        public AsyncImageView(Context context) : base(context)
        {
        }
    }
}