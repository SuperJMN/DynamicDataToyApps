namespace VideoExtraction
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using DotImaging;

    public class VideoBitmapProvider : IBitmapProvider
    {
        private readonly string path;
        private readonly long skipStart;
        private readonly long? length;
        private readonly int skip;

        public VideoBitmapProvider(string path, long skipStart, long? length = null, int skip = 1)
        {
            this.path = path;
            this.skipStart = skipStart;
            this.length = length;
            this.skip = skip;

            Frames = GetEnumerable().Select(ConvertToBitmap);
        }

        private IEnumerable<IImage> GetEnumerable()
        {
            using (var capture = new FileCapture(path))
            {
                capture.Seek(skipStart, SeekOrigin.Begin);

                var l = length ?? capture.Length;
                for (int i = 0; i < l; i++)
                {
                    var enumerable = capture.Read();
                    yield return enumerable;
                    capture.Seek(skip);
                }
            }
        }

        public IEnumerable<BitmapSource> Frames { get; }

        private static BitmapSource ConvertToBitmap(IImage image)
        {
            var bgrs = image.ToBgr();
            var convertToBitmap = bgrs.ToBitmapSource();
            convertToBitmap.Freeze();
            return convertToBitmap;
        }
    }
}