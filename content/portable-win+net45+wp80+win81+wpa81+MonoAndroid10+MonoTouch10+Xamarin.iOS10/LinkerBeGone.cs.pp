using Flipper.Controls;

namespace $rootnamespace$
{
    class LinkerBeGone
    {
        public LinkerBeGone()
        {
            // This is needed to ensure that the linker doesn't remove the control
            var a = new Swiper();
            var b = new CardContentView();
        }
    }
}