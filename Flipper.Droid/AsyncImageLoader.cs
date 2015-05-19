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
using System.Threading.Tasks;
using System.Net.Http;
using ModernHttpClient;
using Android.Util;

namespace Flipper.Droid
{
    /// <summary>
    /// Represents a bitmap that can load an image async
    /// </summary>
    class AsyncImageLoader 
    {
        private string _url;
        private Bitmap _bitmap;

        public Bitmap Bitmap
        {
            get { return _bitmap; }
            set { _bitmap = value; }
        }

        public AsyncImageLoader(string url)
        {
            _url = url;
            Task.Run(async () => await Load());
        }

        public Action<AsyncImageLoader> Completed { get; set; }

        public async Task Load()
        {
            using (var client = new HttpClient(new NativeMessageHandler()))
            {
                try
                {
                    Log.Debug("SwipeRenderer", "Begin loading image '{0}'", _url);
                    var stream = await client.GetStreamAsync(new Uri(_url));
                    _bitmap = await BitmapFactory.DecodeStreamAsync(stream);
                    Log.Debug("SwipeRenderer", "Done loading image '{0}'", _url);

                    if (Completed != null && _bitmap != null)
                    {
                        Completed(this);
                    }
                }
                catch (Exception ex)
                {
                    // TODO Log
                    Log.Debug("SwipeRenderer", "Exception loading image '{0}'", _url);
                }
            }
        }

    }
}