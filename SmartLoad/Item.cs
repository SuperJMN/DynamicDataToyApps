namespace WpfApplication7
{
    using System;
    using System.Windows.Media.Imaging;
    using ReactiveUI;

    public class Item : ReactiveObject, IVisibilityAware
    {
        private string title;
        private bool isVisible;
        private BitmapSource bitmap;

        public Item(string title)
        {
            this.title = title;
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                this.RaiseAndSetIfChanged(ref isVisible, value);
                if (value)
                {
                    var bitmapSource = new BitmapImage(new Uri("Bitmap.jpg", UriKind.Relative));
                    bitmapSource.Freeze();
                    Bitmap = bitmapSource;
                }
                else
                {
                    Bitmap = null;
                }
            }
        }

        public BitmapSource Bitmap
        {
            get { return bitmap; }
            set { this.RaiseAndSetIfChanged(ref bitmap, value); }
        }

        public string Title
        {
            get { return title; }
            set { this.RaiseAndSetIfChanged(ref title, value); }
        }
    }
}