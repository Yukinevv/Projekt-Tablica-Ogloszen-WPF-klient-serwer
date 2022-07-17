using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Logika interakcji dla klasy StronaOgloszenia.xaml
    /// </summary>
    public partial class StronaOgloszenia : Page
    {
        public static ListView ListViewOgl;

        public static int idWybranegoOgloszenia;

        public StronaOgloszenia()
        {
            InitializeComponent();

            ListViewOgl = ListViewOgloszenia;
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new StronaGlowna();

            OperacjeKlient.Wyslij("KATEGORIE");
            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);
            StronaGlowna.ListViewKat.ItemsSource = kategorie;

            // sprawdzenie czy uzytkownik ma uprawnienia administratora
            if (Logowanie.czyAdmin == "nie admin")
            {
                StronaGlowna.UsunKatButton.Visibility = Visibility.Hidden;
            }
        }

        private void ListViewOgloszenia_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow.rama.Content = new EdycjaOgloszenia();

            // uzupelnienie textboxow danymi ogloszenia z bazy
            OperacjeKlient.Wyslij("OGLOSZENIA");
            OperacjeKlient.Wyslij(StronaGlowna.idKategorii.ToString());

            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
            EdycjaOgloszenia.TextBoxTytulOgl.Text = ogloszenia[ListViewOgl.SelectedIndex].Tytul;
            EdycjaOgloszenia.TextBoxTrescOgl.Text = ogloszenia[ListViewOgl.SelectedIndex].Tresc;

            // potrzebne do usuniecia wybranego ogloszenia
            idWybranegoOgloszenia = ogloszenia[ListViewOgl.SelectedIndex].Id;

            // sprawdzenie czy uzytkownik jest wlascicielem wybranego ogloszenia lub czy jest adminem
            // jezeli NIE to ukrywam przyciski odpowiadajace za edycje i usuniecie ogloszenia
            int idUzytkownika = ogloszenia[ListViewOgl.SelectedIndex].UzytkownikId;
            OperacjeKlient.Wyslij("CZY MOZE EDYTOWAC");
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);
            OperacjeKlient.Odbierz();
            OperacjeKlient.Wyslij(idUzytkownika.ToString());

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "NIE")
            {
                EdycjaOgloszenia.ZatwierdzEdycjeOgloszeniaButton.Visibility = Visibility.Hidden;
                EdycjaOgloszenia.UsunOgloszenieButton.Visibility = Visibility.Hidden;
            }
        }

        private void DodajOgloszenieButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new DodawanieOgloszen();

            // wyswietlanie dostepnych kategorii w listboxie
            OperacjeKlient.Wyslij("KATEGORIE");
            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);

            foreach (var kat in kategorie)
            {
                DodawanieOgloszen.ListBoxKat.Items.Add(kat.Nazwa);
            }
        }
    }
}
