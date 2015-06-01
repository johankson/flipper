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

        public async Task Load()
        {
            await LoadWithProgress();
            return;

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
                    Log.Debug("SwipeRenderer", "Exception loading image '{0}'", _url);
                }
            }
        }

        private async Task LoadWithProgress()
        {
            try
            {
                var webClient = new WebClient();

                string proxyHost = Java.Lang.JavaSystem.GetProperty("http.proxyHost");
                string proxyPort = Java.Lang.JavaSystem.GetProperty("http.proxyPort"); 

                if (string.IsNullOrEmpty(proxyHost) == false && string.IsNullOrEmpty(proxyPort) == false)
                {
                    Log.Debug("SwipeRenderer", "proxy host:" + proxyHost + ":" + proxyPort);
                    WebProxy proxy = new WebProxy(proxyHost, int.Parse(proxyPort));
                    webClient.Proxy = proxy;
                }

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
                Log.Debug("SwipeRenderer", "Exception loading image '{0}' using WebClient", _url);
            }
        }

        void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int i = 42;
        }

       

    }
}