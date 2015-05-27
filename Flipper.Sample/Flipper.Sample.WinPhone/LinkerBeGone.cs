using Flipper.WinPhone.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flipper.Sample.WinPhone
{
    class LinkerBeGone
    {
        /// <summary>
        /// We simply need to create a reference to the type to get it loaded
        /// </summary>
        public LinkerBeGone()
        {
            var a = new SwiperRenderer();
        }
    }
}
