namespace TextFileLoader
{
    using System;
    using System.Reactive.Linq;

    public static class ObservableExtensions
    {
        public static IObservable<T> PushEvery<T>(this IObservable<T> first, TimeSpan timeSpan)
        {
            return first.Zip(Observable.Interval(timeSpan), (arg1, l) => arg1);
        }            
    }
}