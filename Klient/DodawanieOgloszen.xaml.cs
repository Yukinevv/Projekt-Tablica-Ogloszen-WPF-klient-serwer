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
    /// Logika interakcji dla klasy DodawanieOgloszen.xaml
    /// </summary>
    public partial class DodawanieOgloszen : Page
    {
        public static ListBox ListBoxKat;

        public DodawanieOgloszen()
        {
            InitializeComponent();

            ListBoxKat = ListBoxKategorie;
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new StronaOgloszenia();

            OperacjeKlient.Wyslij("OGLOSZENIA");
            OperacjeKlient.Wyslij(StronaGlowna.idKategorii.ToString());

            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
            StronaOgloszenia.ListViewOgl.ItemsSource = ogloszenia;
        }

        private void ZatwierdzButton_Click(object sender, RoutedEventArgs e)
        {
            // wyslanie ogloszenia do dodania
            OperacjeKlient.Wyslij("DODANIE OGLOSZENIA");
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);
            string wiadomosc = OperacjeKlient.Odbierz();
            int idUzytkownika = int.Parse(wiadomosc);
            var ogloszenie = new Ogloszenie()
            {
                Tytul = TextBoxTytul.Text,
                Data_utw = DateTime.Now,
                Data_ed = DateTime.Now,
                Tresc = TextBoxTresc.Text,
                UzytkownikId = idUzytkownika
            };
            string oglSerialized = JsonConvert.SerializeObject(ogloszenie, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(oglSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "Dodano")
            {
                // wyslanie wybranych kategorii z listboxa
                var kategorie = ListBoxKat.SelectedItems;
                var nazwyKategorii = new List<string>();
                foreach (var item in kategorie)
                {
                    nazwyKategorii.Add(item.ToString());
                }
                string nazwyKategoriiSerialized = JsonConvert.SerializeObject(nazwyKategorii, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                OperacjeKlient.Wyslij(nazwyKategoriiSerialized);

                string drugaOdpowiedz = OperacjeKlient.Odbierz();
                if (drugaOdpowiedz == "zakonczono dodawanie")
                {
                    MessageBox.Show("Ogłoszenie zostało dodane! Znajdziesz je w kategorii: " + String.Join(", ", nazwyKategorii));
                    MainWindow.rama.Content = new StronaOgloszenia();

                    OperacjeKlient.Wyslij("OGLOSZENIA");
                    OperacjeKlient.Wyslij(StronaGlowna.idKategorii.ToString());

                    string oglSerialized2 = OperacjeKlient.Odbierz();
                    var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized2);
                    StronaOgloszenia.ListViewOgl.ItemsSource = ogloszenia;
                }
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }
    }
}
