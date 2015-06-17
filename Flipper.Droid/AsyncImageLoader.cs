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
using System.Net;

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

        private async Task Load()
        {
            try
            {
                var webClient = new WebClient();
                webClient.DownloadProgressChanged += webClient_DownloadProgressChanged;

                var bytes = await webClient.DownloadDataTaskAsync(new Uri(_url));
                _bitmap = await BitmapFactory.DecodeByteArrayAsync(bytes, 0, bytes.Length);

                if (Completed != null && _bitmap != null)
                {
                    Completed(this);
                }
            }
            catch (Exception ex)
            {
                Log.Debug("SwiperRenderer", "Exception loading image '{0}' using WebClient '{1}'", _url, ex.ToString());
            }
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // TODO Visualize this
            Log.Debug("SwiperRenderer", "DownloadProgress changed for '{0}' to {1}%", _url, e.ProgressPercentage);
        }
    }
}