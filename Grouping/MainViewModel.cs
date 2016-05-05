namespace Grouping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using DynamicData;

    public class MainViewModel
    {
        private readonly ReadOnlyObservableCollection<AgePersonPair> groupedByAgeCollection;
        private readonly ReadOnlyObservableCollection<Person> peopleCollection;

        public MainViewModel()
        {
            var peopleObs = CreatePeopleObservable().Publish();

            var observableChangeSet = peopleObs.ToObservableChangeSet();

            var groupChangeSet = observableChangeSet
                .Group(person => person.Age)
                .Transform((group, i) => new AgePersonPair(group));

            groupChangeSet
                .ObserveOnDispatcher()
                .Bind(out groupedByAgeCollection)
                .Subscribe();

            var peopleChangeSet = observableChangeSet;

            peopleChangeSet
                .ObserveOnDispatcher()
                .Bind(out peopleCollection)
                .Subscribe();

            peopleObs.Connect();
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
    }
}