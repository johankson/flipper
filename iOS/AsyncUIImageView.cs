using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;

namespace Flipper.iOS
{
    class AsyncUIImageView : UIImageView
    {
        LoadingOverlay loadingOverlay;

        public AsyncUIImageView()
        {
        }

        public void IsLoading()
        {
            Image = null;

            if (loadingOverlay == null)
            {
                this.loadingOverlay = new LoadingOverlay(this.Frame);
                this.AddSubview(loadingOverlay);
            }
        }

        public void IsLoaded()
        {
            if(this.loadingOverlay==null)
            {
                return;
            }

            this.loadingOverlay.Hide();
            this.loadingOverlay = null;
        }
    }
}