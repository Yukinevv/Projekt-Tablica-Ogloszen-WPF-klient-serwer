using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    /// Logika interakcji dla klasy StronaGlowna.xaml
    /// </summary>
    public partial class StronaGlowna : Page
    {
        public static ListView ListViewKat;

        public static int idKategorii;

        public static Button UsunKatButton;

        public StronaGlowna()
        {
            InitializeComponent();

            ListViewKat = ListViewKategorie;
            UsunKatButton = UsunKategorieButton;
        }

        private void ListViewKategorie_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow.rama.Content = new StronaOgloszenia();

            OperacjeKlient.Wyslij("KATEGORIE");
            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);

            idKategorii = kategorie[ListViewKat.SelectedIndex].Id;
            OperacjeKlient.Wyslij("OGLOSZENIA");
            OperacjeKlient.Wyslij(idKategorii.ToString());

            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);

            StronaOgloszenia.ListViewOgl.ItemsSource = ogloszenia;
        }

        private void WylogujButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new Logowanie();
        }

        private void DodajKategorieButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxNazwaNowejKategorii.Text == string.Empty)
            {
                MessageBox.Show("Nie wpisales nazwy kategorii!");
                return;
            }

            OperacjeKlient.Wyslij("DODANIE KATEGORII");
            var nowaKategoria = new Kategoria()
            {
                Id = 9999, // wlasciwe Id zostanie utworzone przy insercie do bazy
                Nazwa = TextBoxNazwaNowejKategorii.Text,
                Data_utw = DateTime.Now,
                UzytkownikId = 9999 // serwer pobierze sobie wlasciwe na podstawie loginu zalogowanego uzytkownika, ktory wysylam
            };
            string kategoriaSerialized = JsonConvert.SerializeObject(nowaKategoria, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(kategoriaSerialized);
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "Dodano")
            {
                MessageBox.Show("Dodano nowa kategorie o nazwie: " + TextBoxNazwaNowejKategorii.Text);

                OperacjeKlient.Wyslij("KATEGORIE");
                string katSerialized = OperacjeKlient.Odbierz();
                var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);
                ListViewKat.ItemsSource = kategorie;
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }

        private void UsunKategorieButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxNazwaNowejKategorii.Text == string.Empty)
            {
                MessageBox.Show("Nie wpisales nazwy kategorii!");
                return;
            }

            OperacjeKlient.Wyslij("USUNIECIE KATEGORII");
            OperacjeKlient.Wyslij(TextBoxNazwaNowejKategorii.Text);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "usunieto")
            {
                MessageBox.Show("Kategoria zostala usunieta!");

                OperacjeKlient.Wyslij("KATEGORIE");
                string katSerialized = OperacjeKlient.Odbierz();
                var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);
                ListViewKat.ItemsSource = kategorie;
            }
            else if (odpowiedz == "nie usunieto")
            {
                MessageBox.Show("W kategorii znajduja sie ogloszenia. Nie moze zostac usunieta!");
            }
            else
            {
                MessageBox.Show("Taka kategoria nie istnieje! Prosze sprawdzic poprawnosc wpisanej nazwy.");
            }
        }
    }
}
