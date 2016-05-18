namespace TextFileLoader
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

    public class MainViewModel : ReactiveObject
    {
        private readonly IOpenFileService openFileService;

        private readonly ISubject<IObservable<string>> lines = new Subject<IObservable<string>>();
        private readonly ReadOnlyObservableCollection<string> linesCollection;


        public MainViewModel(IOpenFileService openFileService)
        {
            this.openFileService = openFileService;
            OpenFileCommand = ReactiveCommand.Create();
            OpenFileCommand.Subscribe(_ => OpenFromFile());

            var fileObs = lines
                .Switch();

            fileObs
                .ToObservableChangeSet()
                .ObserveOnDispatcher()
                .Bind(out linesCollection)
                .Subscribe();
        }

        public IEnumerable<string> LinesCollection => linesCollection;

        private void OpenFromFile()
        {
            var dialogResult = openFileService.ShowDialog(null);
            if (dialogResult == true)
            {
                var path = openFileService.FileName;

                var observable = Observable.Using(() => new StreamReader(path, Encoding.Default), CreateObservableLines);

                lines.OnNext(observable);
            }
        }

        private static IObservable<string> CreateObservableLines(StreamReader reader)
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

        public ReactiveCommand<object> OpenFileCommand { get; }
    }
}