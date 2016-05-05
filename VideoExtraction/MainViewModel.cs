namespace VideoExtraction
{
    using System.Collections.ObjectModel;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using DynamicData;
    using System;

    public class MainViewModel
    {
        private ReadOnlyObservableCollection<BitmapSource> bitmaps;

        public MainViewModel()
        {
            var bitmapSource = new VideoBitmapProvider(@"Resources\\Sample.mp4", 0);
            var source = bitmapSource.Frames.ToObservable(new TaskPoolScheduler(new TaskFactory()));
            var changeSet = source.ToObservableChangeSet();

            changeSet
                .ObserveOnDispatcher()
                .Bind(out bitmaps)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<BitmapSource> Bitmaps => bitmaps;
    }
}