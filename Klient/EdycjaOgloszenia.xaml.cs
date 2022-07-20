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
    /// Logika interakcji dla klasy EdycjaOgloszenia.xaml
    /// </summary>
    public partial class EdycjaOgloszenia : Page
    {
        public static TextBox TextBoxTytulOgl;

        public static TextBox TextBoxTrescOgl;

        public static Button UsunOgloszenieButton;

        public static Button ZatwierdzEdycjeOgloszeniaButton;

        public static string SkadWchodze;

        public EdycjaOgloszenia()
        {
            InitializeComponent();

            TextBoxTytulOgl = TextBoxTytul;
            TextBoxTrescOgl = TextBoxTresc;

            UsunOgloszenieButton = UsunButton;
            ZatwierdzEdycjeOgloszeniaButton = ZatwierdzButton;

            // wyswietlanie dostepnych kategorii w listboxie
            OperacjeKlient.Wyslij("KATEGORIE");
            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);
            foreach (var kat in kategorie)
            {
                ListBoxKategorie.Items.Add(kat.Nazwa);
            }

            // zaznaczenie kategorii wybranego ogloszenia
            foreach (var nazwa in StronaOgloszenia.NazwyWybranychKategoriiDoListBoxa)
            {
                ListBoxKategorie.SelectedItems.Add(nazwa);
            }
        }

        private void PowrótButton_Click(object sender, RoutedEventArgs e)
        {
            if (SkadWchodze == "ze strony ogloszenia")
            {
                MainWindow.rama.Content = new StronaOgloszenia();
            }
            else if (SkadWchodze == "z moich ogloszen")
            {
                MainWindow.rama.Content = new MojeOgloszenia();
            }
        }

        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            OperacjeKlient.Wyslij("USUNIECIE OGLOSZENIA");
            OperacjeKlient.Wyslij(StronaOgloszenia.idWybranegoOgloszenia.ToString());
            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "Usunieto")
            {
                MessageBox.Show("Ogloszenie zostalo usuniete (ze wszystkich kategorii)!");
                if (SkadWchodze == "ze strony ogloszenia")
                {
                    MainWindow.rama.Content = new StronaOgloszenia();
                }
                else if (SkadWchodze == "z moich ogloszen")
                {
                    MainWindow.rama.Content = new MojeOgloszenia();
                }
            }
        }

        private void ZatwierdzButton_Click(object sender, RoutedEventArgs e)
        {
            // sprawdzam czy nastapila zmiana w wyborze kategorii
            bool zmianaWKategoriach = false;
            if (ListBoxKategorie.SelectedItems.Count != StronaOgloszenia.NazwyWybranychKategoriiDoListBoxa.Count)
            {
                zmianaWKategoriach = true;
            }
            else
            {
                for (int i = 0; i < ListBoxKategorie.SelectedItems.Count; i++)
                {
                    bool czyZawiera = ListBoxKategorie.SelectedItems.Contains(StronaOgloszenia.NazwyWybranychKategoriiDoListBoxa[i]);
                    if (!czyZawiera)
                    {
                        zmianaWKategoriach = true;
                    }
                }
            }    

            if (TextBoxTytulOgl.Text == string.Empty || TextBoxTrescOgl.Text == string.Empty)
            {
                MessageBox.Show("Uzupełnij wszystkie pola!");
                return;
            }
            else if (TextBoxTytulOgl.Text == StronaOgloszenia.TytulWybranegoOgloszenia &&
                TextBoxTrescOgl.Text == StronaOgloszenia.TrescWybranegoOgloszenia && zmianaWKategoriach == false)
            {
                MessageBox.Show("Nie zostały dokonane żadne zmiany!");
                return;
            }

            OperacjeKlient.Wyslij("EDYCJA OGLOSZENIA");
            // potrzebuje przeslac id, tytul i tresc ogloszenia, ale zeby nie wysylac trzech danych oddzielnie to wysylam jeden obiekt
            var ogloszenie = new Ogloszenie()
            {
                Id = StronaOgloszenia.idWybranegoOgloszenia,
                Tytul = TextBoxTytulOgl.Text,
                Data_utw = DateTime.Now,
                Data_ed = DateTime.Now,
                Tresc = TextBoxTrescOgl.Text,
            };
            string ogloszenieSerialized = JsonConvert.SerializeObject(ogloszenie, Formatting.Indented,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            OperacjeKlient.Wyslij(ogloszenieSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "zedytowano ogloszenie")
            {
                // wyslanie nowo wybranych kategorii z listboxa
                var nazwyWybranychKategorii = new List<string>();
                foreach (var nazwa in ListBoxKategorie.SelectedItems)
                {
                    nazwyWybranychKategorii.Add(nazwa.ToString());
                }
                string nazwyWybranychKategoriiSerialized = JsonConvert.SerializeObject(nazwyWybranychKategorii, Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
                OperacjeKlient.Wyslij(nazwyWybranychKategoriiSerialized);

                string odpowiedz2 = OperacjeKlient.Odbierz();
                if (odpowiedz2 == "zakonczono edycje")
                {
                    MessageBox.Show("Ogloszenie zostalo zedytowane! Znajdziesz je w kategorii: " + String.Join(", ", nazwyWybranychKategorii));
                    if (SkadWchodze == "ze strony ogloszenia")
                    {
                        MainWindow.rama.Content = new StronaOgloszenia();
                    }
                    else if (SkadWchodze == "z moich ogloszen")
                    {
                        MainWindow.rama.Content = new MojeOgloszenia();
                    }
                }    
            }
        }
    }
}
