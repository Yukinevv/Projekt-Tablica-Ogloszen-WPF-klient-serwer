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
    /// Logika interakcji dla klasy MojeOgloszenia.xaml
    /// </summary>
    public partial class MojeOgloszenia : Page
    {
        private static List<Ogloszenie> OgloszeniaKopia;

        private static bool posortowano = false;

        public MojeOgloszenia()
        {
            InitializeComponent();

            OperacjeKlient.Wyslij("MOJE OGLOSZENIA");
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);
            string oglSerialized = OperacjeKlient.Odbierz();
            var ogloszenia = JsonConvert.DeserializeObject<List<Ogloszenie>>(oglSerialized);
            ListViewOgloszenia.ItemsSource = ogloszenia;

            OgloszeniaKopia = ogloszenia;
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new MojProfil();
        }

        private void ListViewOgloszenia_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewOgloszenia.SelectedItem == null)
            {
                return;
            }

            var ogloszenia = (List<Ogloszenie>)ListViewOgloszenia.ItemsSource;

            // potrzebne do np. usuniecia wybranego ogloszenia
            StronaOgloszenia.idWybranegoOgloszenia = ogloszenia[ListViewOgloszenia.SelectedIndex].Id;

            // potrzebne do edycji kategorii ogloszenia
            OperacjeKlient.Wyslij("WYBRANE NAZWY KATEGORII");
            OperacjeKlient.Wyslij(StronaOgloszenia.idWybranegoOgloszenia.ToString());
            string nazwyKategoriiSerialized = OperacjeKlient.Odbierz();
            StronaOgloszenia.NazwyWybranychKategoriiDoListBoxa = JsonConvert.DeserializeObject<List<string>>(nazwyKategoriiSerialized);

            MainWindow.rama.Content = new EdycjaOgloszenia();
            EdycjaOgloszenia.SkadWchodze = "z moich ogloszen";

            // uzupelnienie textboxow danymi ogloszenia       
            EdycjaOgloszenia.TextBoxTytulOgl.Text = ogloszenia[ListViewOgloszenia.SelectedIndex].Tytul;
            EdycjaOgloszenia.TextBoxTrescOgl.Text = ogloszenia[ListViewOgloszenia.SelectedIndex].Tresc;

            // potrzebne do weryfikacji zmian przy edycji ogloszenia
            StronaOgloszenia.TytulWybranegoOgloszenia = ogloszenia[ListViewOgloszenia.SelectedIndex].Tytul;
            StronaOgloszenia.TrescWybranegoOgloszenia = ogloszenia[ListViewOgloszenia.SelectedIndex].Tresc;

            

            // sprawdzenie czy uzytkownik jest wlascicielem wybranego ogloszenia lub czy jest adminem
            // jezeli NIE to ukrywam przyciski odpowiadajace za edycje i usuniecie ogloszenia
            int idUzytkownika = ogloszenia[ListViewOgloszenia.SelectedIndex].UzytkownikId;
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
            DodawanieOgloszen.SkadWchodze = "z moich ogloszen";
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var kolumna = (sender as GridViewColumnHeader);
            var ogloszenia = (List<Ogloszenie>)ListViewOgloszenia.ItemsSource;

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
                ListViewOgloszenia.ItemsSource = ogloszenia;
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
                ListViewOgloszenia.ItemsSource = ogloszenia;
                OgloszeniaKopia = ogloszenia;
                posortowano = false;
            }
        }

        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var ogloszenia = (List<Ogloszenie>)ListViewOgloszenia.ItemsSource;
            if (TextBoxFilter.Text == string.Empty)
            {
                ogloszenia = OgloszeniaKopia;
            }
            else
            {
                ogloszenia = ogloszenia.Where(o => o.Tytul.Contains(TextBoxFilter.Text)).ToList();
            }
            ListViewOgloszenia.ItemsSource = ogloszenia;
        }
    }
}
