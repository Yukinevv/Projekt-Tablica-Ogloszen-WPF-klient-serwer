using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Klient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Frame rama;

        public MainWindow()
        {
            InitializeComponent();

            rama = Ramka;
            rama.Content = new Logowanie();

            OperacjeKlient.PolaczZSerwerem();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OperacjeKlient.clientSocket.Close();
        }
    }
}
