namespace ReactiveLocura
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reactive.Linq;
    using System.Text;
    using DynamicData;
    using DynamicData.Binding;
    using ReactiveUI;
    public class FileViewModel: ReactiveObject, IDisposable
    {
        private readonly ReadOnlyObservableCollection<string> lines;

        public FileViewModel(StreamReader stream)
        {
            var obs = CreateObservableFromReader(stream);

            obs
                .ToObservableChangeSet()
                .ObserveOnDispatcher()
                .Bind(out lines)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<string> Lines => lines;

        private static IObservable<string> CreateObservableFromReader(StreamReader reader)
        {
            var linesFromFile = ToLines(reader);

            var observableLinesFromFile = linesFromFile
                .ToObservable()
                .PushEvery(TimeSpan.FromSeconds(1));

            return observableLinesFromFile;
        }

        private static IEnumerable<string> ToLines(StreamReader streamReader)
        {
            while (!streamReader.EndOfStream)
            {
                yield return streamReader.ReadLine();
            }
        }


        public void Dispose()
        {
            //stream.Close();
        }
    }
}