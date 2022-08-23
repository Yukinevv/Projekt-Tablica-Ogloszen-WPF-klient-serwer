using System.Windows;
using System.Windows.Controls;

namespace Serwer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TextBox TextBoxLogs;
        public MainWindow()
        {
            InitializeComponent();

            TextBoxLogs = TextBoxLog;

            DataContext = new MainWindowModelWidoku();
        }
    }
}
