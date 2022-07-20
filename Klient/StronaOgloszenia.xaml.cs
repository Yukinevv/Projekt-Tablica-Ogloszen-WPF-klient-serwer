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

        public static string TytulWybranegoOgloszenia;

        public static string TrescWybranegoOgloszenia;

        public static int idWybranegoOgloszenia;

        private static List<Ogloszenie> OgloszeniaKopia;

        public static List<string> NazwyWybranychKategoriiDoListBoxa;

        private static bool posortowano = false;

        public StronaOgloszenia()
        {
            InitializeComponent();

            ListViewOgl = ListViewOgloszenia;

            OperacjeKlient.Wyslij("OGLOSZENIA");
            OperacjeKlient.Wyslij(StronaGlowna.idKategorii.ToString());
            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
            ListViewOgl.ItemsSource = ogloszenia;
            OgloszeniaKopia = ogloszenia;
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new StronaGlowna();
        }

        private void ListViewOgloszenia_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewOgl.SelectedItem == null)
            {
                return;
            }

            var ogloszenia = (List<Ogloszenie>)ListViewOgl.ItemsSource;

            // potrzebne do np. usuniecia wybranego ogloszenia
            idWybranegoOgloszenia = ogloszenia[ListViewOgl.SelectedIndex].Id;

            // potrzebne do edycji kategorii ogloszenia
            OperacjeKlient.Wyslij("WYBRANE NAZWY KATEGORII");
            OperacjeKlient.Wyslij(idWybranegoOgloszenia.ToString());
            string nazwyKategoriiSerialized = OperacjeKlient.Odbierz();
            NazwyWybranychKategoriiDoListBoxa = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);

            MainWindow.rama.Content = new EdycjaOgloszenia();
            EdycjaOgloszenia.SkadWchodze = "ze strony ogloszenia";               

            // uzupelnienie textboxow danymi ogloszenia      
            EdycjaOgloszenia.TextBoxTytulOgl.Text = ogloszenia[ListViewOgl.SelectedIndex].Tytul;
            EdycjaOgloszenia.TextBoxTrescOgl.Text = ogloszenia[ListViewOgl.SelectedIndex].Tresc;

            // potrzebne do weryfikacji zmian przy edycji ogloszenia
            TytulWybranegoOgloszenia = ogloszenia[ListViewOgl.SelectedIndex].Tytul;
            TrescWybranegoOgloszenia = ogloszenia[ListViewOgl.SelectedIndex].Tresc;            

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
                EdycjaOgloszenia.TextBoxTytulOgl.IsReadOnly = true;
                EdycjaOgloszenia.TextBoxTrescOgl.IsReadOnly = true;
            }
        }

        private void DodajOgloszenieButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new DodawanieOgloszen();
            DodawanieOgloszen.SkadWchodze = "ze strony ogloszenia";
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var kolumna = (sender as GridViewColumnHeader);
            var ogloszenia = (List<Ogloszenie>)ListViewOgl.ItemsSource;

            if (posortowano == false)
            {
                if (kolumna.Tag.ToString() == "Id")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Id).ToList();
                }
                else if (kolumna.Tag.ToString() == "UzytkownikId")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.UzytkownikId).ToList();
                }
                else if (kolumna.Tag.ToString() == "Tytul")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Tytul).ToList();
                }
                else if (kolumna.Tag.ToString() == "Data_utw")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Data_utw).ToList();
                }
                else if (kolumna.Tag.ToString() == "Data_ed")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Data_ed).ToList();
                }
                else if (kolumna.Tag.ToString() == "Tresc")
                {
                    ogloszenia = ogloszenia.OrderBy(o => o.Tresc).ToList();
                }
                ListViewOgl.ItemsSource = ogloszenia;
                OgloszeniaKopia = ogloszenia;
                posortowano = true;
            }
            else
            {
                if (kolumna.Tag.ToString() == "Id")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Id).ToList();
                }
                else if (kolumna.Tag.ToString() == "UzytkownikId")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.UzytkownikId).ToList();
                }
                else if (kolumna.Tag.ToString() == "Tytul")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Tytul).ToList();
                }
                else if (kolumna.Tag.ToString() == "Data_utw")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Data_utw).ToList();
                }
                else if (kolumna.Tag.ToString() == "Data_ed")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Data_ed).ToList();
                }
                else if (kolumna.Tag.ToString() == "Tresc")
                {
                    ogloszenia = ogloszenia.OrderByDescending(o => o.Tresc).ToList();
                }
                ListViewOgl.ItemsSource = ogloszenia;
                OgloszeniaKopia = ogloszenia;
                posortowano = false;
            }
        }

        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var ogloszenia = (List<Ogloszenie>)ListViewOgl.ItemsSource;
            if (TextBoxFilter.Text == string.Empty)
            {
                ogloszenia = OgloszeniaKopia;
            }
            else
            {
                ogloszenia = ogloszenia.Where(o => o.Tytul.Contains(TextBoxFilter.Text)).ToList();
            }
            ListViewOgl.ItemsSource = ogloszenia;
        }
    }
}
