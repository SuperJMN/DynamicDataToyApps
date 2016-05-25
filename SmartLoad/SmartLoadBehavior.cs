namespace WpfApplication7
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    public class SmartLoadBehavior : Behavior<ItemsControl>
    {
        private IEnumerable<IVisibilityAware> oldItems = new List<IVisibilityAware>();
        private ScrollViewer scrollViewer;
        private IDisposable loaded;
        private IDisposable updater;
        
        protected override void OnAttached()
        {
            base.OnAttached();

            loaded = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
                handler => AssociatedObject.Loaded += handler,
                handler => AssociatedObject.Loaded -= handler)
                .Subscribe(_ => OnLoaded());            
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            updater?.Dispose();
        }

        private void OnLoaded()
        {
            loaded.Dispose();

            scrollViewer = VisualTreeExtensions.GetVisualChild<ScrollViewer>(AssociatedObject);
            var scrollBar = scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer) as ScrollBar;

            var valueChanged = Observable.FromEventPattern<RoutedPropertyChangedEventHandler<double>, RoutedEventArgs>(
                handler => scrollBar.ValueChanged += handler,
                handler => scrollBar.ValueChanged -= handler)
                .Select(_ => Unit.Default);

            var layoutUpdated = Observable.FromEventPattern<EventHandler, EventArgs>(
                handler => AssociatedObject.LayoutUpdated += handler,
                handler => AssociatedObject.LayoutUpdated -= handler)
                .Select(_ => Unit.Default);

            var update = layoutUpdated
                .Merge(valueChanged)
                .Sample(TimeSpan.FromMilliseconds(100))
                .StartWith(Unit.Default);

            updater = update
                .ObserveOnDispatcher()
                .Subscribe(_ => RefreshState());
        }

        private void RefreshState()
        {
            var index = (int)scrollViewer.VerticalOffset;
            var count = (int)scrollViewer.ViewportHeight;

            if (index + count < AssociatedObject.Items.Count)
            {
                count++;
            }

            var newItems = GetItems(index, count).ToList();
            SetVisibilityValues(oldItems, newItems);
            oldItems = newItems;
        }

        private void SetVisibilityValues(IEnumerable<IVisibilityAware> oldItems, IEnumerable<IVisibilityAware> visible)
        {
            var noLonger = oldItems.Except(visible);
            var newer = visible.Except(oldItems);

            foreach (var visibility in noLonger)
            {
                visibility.IsVisible = false;
            }

            foreach (var visibility in newer)
            {
                visibility.IsVisible = true;
            }
        }

        private IEnumerable<IVisibilityAware> GetItems(int index, int count)
        {
            for (var i = index; i < index + count; i++)
            {
                var container = AssociatedObject.ItemContainerGenerator.ContainerFromIndex(i);
                var item = AssociatedObject.ItemContainerGenerator.ItemFromContainer(container);
                yield return (IVisibilityAware)item;
            }
        }
    }
}