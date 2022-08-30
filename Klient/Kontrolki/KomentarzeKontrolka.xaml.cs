using Newtonsoft.Json;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy KomentarzeKontroka.xaml
    /// </summary>
    public partial class KomentarzeKontrolka : UserControl
    {
        public KomentarzeKontrolka()
        {
            InitializeComponent();
        }

        private void EdytujKomentarzButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!OperacjeKlient.SocketConnected(OperacjeKlient.clientSocket))
            {
                MessageBox.Show("Utracono polaczenie z serwerem! Aplikacja zostanie zamknieta.");
                OperacjeKlient.clientSocket.Close();
                Application.Current.Shutdown();
                return;
            }

            if (!KomentarzeOgloszeniaModelWidoku.KomentarzeLista.Any(k => k.CzyZaznaczony == true))
            {
                MessageBox.Show("Nie zostal zaznaczony zaden komentarz!");
                return;
            }
            if (KomentarzeOgloszeniaModelWidoku.KomentarzeLista.Where(k => k.CzyZaznaczony == true).ToList().Count >= 2)
            {
                MessageBox.Show("Nie mozesz edytowac wiecej niz jeden komentarz jednoczesnie!");
                return;
            }
            if (KomentarzeOgloszeniaModelWidoku.TextBoxTrescModelWidoku == string.Empty)
            {
                MessageBox.Show("Komentarz nie moze byc pusty!");
                return;
            }

            var wybranyKomentarz = KomentarzeOgloszeniaModelWidoku.KomentarzeLista.FirstOrDefault(k => k.CzyZaznaczony == true);

            if (KomentarzeOgloszeniaModelWidoku.TextBoxTrescModelWidoku == wybranyKomentarz.Tresc)
            {
                MessageBox.Show("Aby zedytowac komentarz musisz zmienic jego tresc!");
                return;
            }

            var result = MessageBox.Show("Czy na pewno chcesz edytowac wybrany komentarz?", "Edycja komentarza",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            OperacjeKlient.Wyslij("EDYCJA KOMENTARZA");

            string[] dane = new string[2] { wybranyKomentarz.Id.ToString(), KomentarzeOgloszeniaModelWidoku.TextBoxTrescModelWidoku };
            string daneSerialized = JsonConvert.SerializeObject(dane, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(daneSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "zedytowano")
            {
                MessageBox.Show("Twoj komentarz zostal zedytowany!");
                KomentarzeOgloszeniaModelWidoku.TextBoxTrescModelWidoku = string.Empty;
                MainWindow.Rama.Content = new KomentarzeOgloszenia();
            }
        }
    }
}
