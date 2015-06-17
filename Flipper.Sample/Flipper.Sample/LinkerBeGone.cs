using Flipper.Controls;

namespace Flipper.Sample
{
    class LinkerBeGone
    {
        public LinkerBeGone()
        {
            // This is needed to ensure that the linker doesn't remove the control
            var a = new Swiper();
            var b = new CardContentView();
            var c = new RangeSlider();
        }
    }
}
