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

    public class MainViewModel : ReactiveObject
    {
        private readonly IOpenFileService openFileService;
        
        private readonly ISubject<IObservable<FileViewModel>> files = new Subject<IObservable<FileViewModel>>();
        private readonly ObservableAsPropertyHelper<FileViewModel> file;


        public MainViewModel(IOpenFileService openFileService)
        {
            this.openFileService = openFileService;
            OpenFileCommand = ReactiveCommand.Create();
            OpenFileCommand.Subscribe(_ => OpenFromFile());

            var fileObs = files
                .Switch()
                .Publish();
            
            fileObs.ToProperty(this, model => model.File, out file);

            fileObs.Connect();
        }

        public FileViewModel File => file.Value;

        private void OpenFromFile()
        {
            var dialogResult = openFileService.ShowDialog(null);
            if (dialogResult == true)
            {
                var path = openFileService.FileName;
                var observable = Observable.Using(() => new StreamReader(path, Encoding.Default), Observable.Return);
                files.OnNext(observable);
            }
        }
       
        public ReactiveCommand<object> OpenFileCommand { get; }        
    }
}