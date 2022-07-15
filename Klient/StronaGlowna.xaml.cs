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
    /// Logika interakcji dla klasy StronaGlowna.xaml
    /// </summary>
    public partial class StronaGlowna : Page
    {
        public static TextBox textBoxInfo;

        public StronaGlowna()
        {
            InitializeComponent();

            textBoxInfo = TextBoxInformacyjny;
        }

        private void RejestracjaButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new Rejestracja();
        }

        private void LogowanieButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxLogin.Text == string.Empty || TextBoxHaslo.Text == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }

            OperacjeKlient.Wyslij("LOGOWANIE");
            OperacjeKlient.Wyslij(TextBoxLogin.Text);
            OperacjeKlient.Odbierz();
            OperacjeKlient.Wyslij(TextBoxHaslo.Text);
            string czyZalogowac = OperacjeKlient.Odbierz();

            if (czyZalogowac == "true")
            {
                MessageBox.Show("Logowanie powiodlo sie!");
            }
            else
            {
                MessageBox.Show("Logowanie nieudane!");
            }
        }
    }
}
