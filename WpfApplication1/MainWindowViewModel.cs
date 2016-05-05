namespace WpfApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using DynamicData;
    using DynamicData.Binding;

    public class MainWindowViewModel
    {
        private IEnumerable<Person> people;
        private readonly ReadOnlyObservableCollection<GroupedData> groupedByAge;

        public MainWindowViewModel()
        {
            people = new[]
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
                new Person("Oscar", 4),
            };

            var peopleObs = CreatePeopleObservable();

            var observableChangeSet = peopleObs.ToObservableChangeSet();

            var groupChangeSet  = observableChangeSet
                .Group(person => person.Age)
                .Transform((group, i) => new GroupedData(group));

            groupChangeSet
                .ObserveOnDispatcher()
                .Bind(out groupedByAge)                
                .Subscribe();
        }

        private IObservable<Person> CreatePeopleObservable()
        {
            var intervalObs = Observable.Interval(TimeSpan.FromSeconds(2));
            var peopleObs = people.ToObservable();
            var intervalPeopleObs = intervalObs.Zip(peopleObs, (_, person) => person);
            return intervalPeopleObs;
        }

        public ReadOnlyObservableCollection<GroupedData> GroupedByAge => groupedByAge;

        public IObservableCollection<Person> People { get; set; }
    }

    public class GroupedData : AbstractNotifyPropertyChanged
    {
        private readonly IGroup<Person, int, int> g;
        private IEnumerable<Person> people;

        public GroupedData(IGroup<Person, int, int> group)
        {
            group.Cache.Connect()
                .QueryWhenChanged(query => query)
                .Subscribe(query => People = query.Items);

            Age = group.Key;
        }

        public IEnumerable<Person> People
        {
            get { return people; }
            set { SetAndRaise(ref people, value); }
        }

        public int Age { get; set; }
    }
}