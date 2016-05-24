namespace WpfApplication7
{
    using System.Collections.ObjectModel;
    using System.Linq;

    public class MainViewModel
    {
        public MainViewModel()
        {
            Contacts = new ObservableCollection<Item>(Enumerable.Range(1, 4000).Select(i => new Item(i.ToString())) );
        }

        public ObservableCollection<Item> Contacts { get; set; }
    }
}