namespace TextFileLoader
{
    using System.Windows;
    using Cinch;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel(new WPFOpenFileService());
        }
    }
}
