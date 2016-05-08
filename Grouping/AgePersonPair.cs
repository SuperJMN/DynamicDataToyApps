namespace Grouping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Windows.Threading;
    using DynamicData;
    using DynamicData.Binding;
    using ReactiveUI;

    public class AgePersonPair : ReactiveObject, IDisposable
    {
        private readonly ReadOnlyObservableCollection<Person> people;
        private readonly IDisposable cleanup;
        private readonly ObservableAsPropertyHelper<int> count;

        public AgePersonPair(IGroup<Person, int> group, Dispatcher dispatcher)
        {
            cleanup = group.List.Connect()
                .ObserveOn(dispatcher)
                .Bind(out people)
                .Subscribe();

            Age = group.GroupKey;

            group.List.CountChanged.ToProperty(this, pair => pair.Count, out count);

        }

        public int Count => count.Value;

        public ReadOnlyObservableCollection<Person> People => people;

        public int Age { get; set; }

        public void Dispose()
        {
            cleanup.Dispose();
        }
    }    
}