namespace Grouping
{
    using System;
    using System.Collections.Generic;
    using DynamicData;
    using DynamicData.Binding;

    public class AgePersonPair : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IGroup<Person, int, int> g;
        private IEnumerable<Person> people;
        private readonly IDisposable cleanup;
        private PersonGroupViewModel personGroup;

        public AgePersonPair(IGroup<Person, int, int> group)
        {
            cleanup = group.Cache.Connect()
                .QueryWhenChanged(
                    query =>
                    {
                        var count = query.Count;
                        var items = query.Items;
                        return new PersonGroupViewModel(count, items);
                    })
                .Subscribe(model => PersonGroup = model);

            Age = group.Key;
        }

        public PersonGroupViewModel PersonGroup
        {
            get { return personGroup; }
            set { SetAndRaise(ref personGroup, value); }
        }    

        public int Age { get; set; }

        public int PeopleCount { get; set; }

        public void Dispose()
        {
            cleanup.Dispose();
        }
    }

    public class PersonGroupViewModel
    {
        public int Count { get; set; }
        public IEnumerable<Person> People { get; set; }

        public PersonGroupViewModel(int count, IEnumerable<Person> people)
        {
            Count = count;
            People = people;
        }
    }
}