using System.Reactive.Disposables;

namespace Grouping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Windows.Threading;
    using DynamicData;
    using DynamicData.Binding;
    using ReactiveUI;

    public class AgePersonPair : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable cleanup;
        private readonly ReadOnlyObservableCollection<Person> people;
        private  int count;
        private decimal _percent;

        public AgePersonPair(IGroup<Person, int> group, IObservable<int> totalCountChanged, Dispatcher dispatcher)
        {
             var peopleLoader = group.List.Connect()
                .ObserveOn(dispatcher)
                .Bind(out people)
                .Subscribe();

            Age = group.GroupKey;

            var countChangedShared = group.List.CountChanged.Publish();

            var percentChaged = totalCountChanged.CombineLatest(countChangedShared, (overallCount, groupCount) =>
            {
                return overallCount == 0 ?  0 : Math.Round(groupCount / (decimal)overallCount,2);
            }).Subscribe(pc => Percent = pc);

            var countChanged = countChangedShared.Subscribe(num => Count = num);

            cleanup = new CompositeDisposable(percentChaged, peopleLoader, countChanged, countChangedShared.Connect());


        }

        public int Count
        {
            get { return count; }
            set { SetAndRaise(ref count, value); }
        }

        public decimal Percent
        {
            get { return _percent; }
            set { SetAndRaise(ref _percent, value);  }
        }

        public ReadOnlyObservableCollection<Person> People => people;

        public int Age { get; set; }

        public void Dispose()
        {
            cleanup.Dispose();
        }
    }    
}