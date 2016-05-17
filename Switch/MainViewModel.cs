namespace ReactiveLocura
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Text;
    using Cinch;
    using DynamicData;
    using ReactiveUI;

    public class MainViewModel
    {
        private readonly IOpenFileService openFileService;
        private readonly ISubject<IObservable<string>> workUnits = new Subject<IObservable<string>>();
        private readonly ReadOnlyObservableCollection<string> lines;
        private readonly IConnectableObservable<string> observableLines;

        public MainViewModel(IOpenFileService openFileService)
        {
            this.openFileService = openFileService;
            OpenFileCommand = ReactiveCommand.Create();
            OpenFileCommand.Subscribe(_ => OpenFromFile());

            observableLines = workUnits                
                .Switch()
                .Publish();
            
            observableLines
                .ToObservableChangeSet()
                .ObserveOnDispatcher()
                .Bind(out lines)
                .Subscribe();
        }

        public IEnumerable<string> Lines => lines;

        private void OpenFromFile()
        {
            var dialogResult = openFileService.ShowDialog(null);
            if (dialogResult == true)
            {
                var path = openFileService.FileName;
                observableLines.Connect();

                var linesFromFile = ToLines(new StreamReader(path, Encoding.Default));

                var observableLinesFromFile = linesFromFile
                    .ToObservable()
                    .PushEvery(TimeSpan.FromSeconds(1));

                workUnits.OnNext(observableLinesFromFile);
            }
        }

        private static IEnumerable<string> ToLines(StreamReader streamReader)
        {
            while (!streamReader.EndOfStream)
            {
                yield return streamReader.ReadLine();
            }
        }

        public ReactiveCommand<object> OpenFileCommand { get; }        
    }
}