namespace VideoExtraction
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    public interface IBitmapProvider
    {
        IEnumerable<BitmapSource> Frames { get; }
    }
}