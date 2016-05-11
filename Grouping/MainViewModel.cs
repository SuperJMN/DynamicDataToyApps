using System.Reactive.Disposables;

namespace Grouping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Windows;
    using DynamicData;
    using DynamicData.Aggregation;

    public class MainViewModel: IDisposable
    {
        private readonly ReadOnlyObservableCollection<AgePersonPair> groupedByAgeCollection;
        private readonly ReadOnlyObservableCollection<Person> peopleCollection;
        private readonly IDisposable cleanUp;

        public MainViewModel()
        {
            var dispatcher = Application.Current.Dispatcher;

            var localList = CreatePeopleObservable()
                        .ToObservableChangeSet()
                        .AsObservableList();

            //var count changed observable 
            //var countChanged = localCache.CountChanged;
            
            //alternatively include 'DynamicData.Aggregation' namespace and use dd specific aggregations 
            var countChanged = localList.Connect().Count();

            var groupLoader = localList.Connect()
                .GroupOn(person => person.Age)
                .Transform(group => new AgePersonPair(group, countChanged, dispatcher))
                .ObserveOn(dispatcher)
                .Bind(out groupedByAgeCollection)
                .DisposeMany()
                .Subscribe();

            var peopleLoader = localList.Connect()
                .ObserveOn(dispatcher)
                .Bind(out peopleCollection)
                .Subscribe();

            cleanUp = new CompositeDisposable(peopleLoader, groupLoader, groupLoader);
        }

        public ReadOnlyObservableCollection<AgePersonPair> GroupedByAgeCollection => groupedByAgeCollection;

        public ReadOnlyObservableCollection<Person> People => peopleCollection;

        private IObservable<Person> CreatePeopleObservable()
        {
            var people = new[]
            {
                new Person("JMN", 1),
                new Person("Andrea", 1),
                new Person("Ana", 1),
                new Person("Pepito", 2),
                new Person("Johnny", 4),
                new Person("Mary", 1),
                new Person("Rose", 4),
                new Person("Anthony", 3),
                new Person("David", 2),
                new Person("Joanna", 5),
                new Person("Oscar", 4)
            };

            var intervalObs = Observable.Interval(TimeSpan.FromSeconds(2));
            var peopleObs = people.ToObservable();
            var intervalPeopleObs = intervalObs.Zip(peopleObs, (_, person) => person);
            return intervalPeopleObs;
        }

        public void Dispose()
        {
            cleanUp.Dispose();
        }
    }
}