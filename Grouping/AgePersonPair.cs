using System.Reactive.Disposables;

namespace Grouping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reactive.Linq;
    using System.Windows.Threading;
    using DynamicData;
    using DynamicData.Binding;

    public class AgePersonPair : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable cleanup;
        private readonly ReadOnlyObservableCollection<Person> people;
        private  int count;
        private double percent;

        public AgePersonPair(IGroup<Person, int> group, IObservable<int> totalCountChanged, Dispatcher dispatcher)
        {
             var peopleLoader = group.List.Connect()
                .ObserveOn(dispatcher)
                .Bind(out people)
                .Subscribe();

            Age = group.GroupKey;

            var countChangedShared = group.List.CountChanged.Publish();

            var percentChaged = totalCountChanged
                .CombineLatest(countChangedShared, (overallCount, groupCount) => overallCount == 0 ?  0 : groupCount / (double)overallCount)
                .Subscribe(pc => Percent = pc);

            var countChanged = countChangedShared.Subscribe(num => Count = num);

            cleanup = new CompositeDisposable(percentChaged, peopleLoader, countChanged, countChangedShared.Connect());


        }

        public int Count
        {
            get { return count; }
            set { SetAndRaise(ref count, value); }
        }

        public double Percent
        {
            get { return percent; }
            set { SetAndRaise(ref percent, value);  }
        }

        public ReadOnlyObservableCollection<Person> People => people;

        public int Age { get; set; }

        public void Dispose()
        {
            cleanup.Dispose();
        }
    }    
}