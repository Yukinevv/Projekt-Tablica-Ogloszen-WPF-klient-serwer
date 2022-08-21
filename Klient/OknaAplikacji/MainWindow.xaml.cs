using System.Windows;
using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Frame Rama;

        public MainWindow()
        {
            InitializeComponent();

            Rama = Ramka;

            DataContext = new MainWindowModelWidoku();
        }
    }
}
