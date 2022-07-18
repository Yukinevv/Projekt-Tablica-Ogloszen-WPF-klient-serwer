﻿using BibliotekaEncje.Encje;
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

        public static PanelAdmina PanelAdmina;

        public static bool CzyPanelAdminaOtwarty;

        public static string czyAdmin;

        private static List<Kategoria> Kategorie_kopia;

        private static bool posortowano = false;

        public StronaGlowna()
        {
            InitializeComponent();

            ListViewKat = ListViewKategorie;

            // sprawdzenie czy uzytkownik ma uprawnienia administratora
            OperacjeKlient.Wyslij("CZY ADMIN");
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);
            czyAdmin = OperacjeKlient.Odbierz();
            if (czyAdmin == "nie admin")
            {
                UsunKategorieButton.Visibility = Visibility.Hidden;
                PanelAdministracyjnyButton.Visibility = Visibility.Hidden;
            }

            OperacjeKlient.Wyslij("KATEGORIE");
            string katSerialized = OperacjeKlient.Odbierz();
            var kategorie = JsonConvert.DeserializeObject<List<Kategoria>>(katSerialized);
            ListViewKat.ItemsSource = kategorie;
            Kategorie_kopia = kategorie;
        }

        private void ListViewKategorie_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewKat.SelectedItem == null)
            {
                return;
            }

            var kategorie = (List<Kategoria>)ListViewKat.ItemsSource;
            idKategorii = kategorie[ListViewKat.SelectedIndex].Id;

            MainWindow.rama.Content = new StronaOgloszenia();
        }

        private void WylogujButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new Logowanie();
            if (CzyPanelAdminaOtwarty)
            {
                PanelAdmina.Close();
                CzyPanelAdminaOtwarty = false;
            }
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

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var kolumna = (sender as GridViewColumnHeader);
            var kategorie = (List<Kategoria>)ListViewKat.ItemsSource;

            if (posortowano == false)
            {
                if (kolumna.Tag.ToString() == "Id")
                {
                    kategorie = kategorie.OrderBy(k => k.Id).ToList();
                }
                else if (kolumna.Tag.ToString() == "Nazwa")
                {
                    kategorie = kategorie.OrderBy(k => k.Nazwa).ToList();
                }
                else if (kolumna.Tag.ToString() == "Data_utw")
                {
                    kategorie = kategorie.OrderBy(k => k.Data_utw).ToList();
                }
                ListViewKat.ItemsSource = kategorie;
                Kategorie_kopia = kategorie;
                posortowano = true;
            }
            else
            {
                if (kolumna.Tag.ToString() == "Id")
                {
                    kategorie = kategorie.OrderByDescending(k => k.Id).ToList();
                }
                else if (kolumna.Tag.ToString() == "Nazwa")
                {
                    kategorie = kategorie.OrderByDescending(k => k.Nazwa).ToList();
                }
                else if (kolumna.Tag.ToString() == "Data_utw")
                {
                    kategorie = kategorie.OrderByDescending(k => k.Data_utw).ToList();
                }
                ListViewKat.ItemsSource = kategorie;
                Kategorie_kopia = kategorie;
                posortowano = false;
            }
        }

        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var kategorie = (List<Kategoria>)ListViewKat.ItemsSource;
            if (TextBoxFilter.Text == string.Empty)
            {
                kategorie = Kategorie_kopia;
            }
            else
            {
                kategorie = kategorie.Where(k => k.Nazwa.Contains(TextBoxFilter.Text)).ToList();
            }
            ListViewKat.ItemsSource = kategorie;


            // roboczo
            //if (TextBoxFilter.Text == null)
            //{
            //    ListViewKat.Items.Filter = null;
            //}
            //else
            //{
            //    ListViewKat.Items.Filter = (sender) =>
            //    {
            //        var filterObject = sender as Kategoria;
            //        return filterObject.Nazwa.Contains(TextBoxFilter.Text);
            //    };
            //}
        }

        private void PanelAdministracyjnyButton_Click(object sender, RoutedEventArgs e)
        {
            PanelAdmina = new PanelAdmina();
            PanelAdmina.Show();
            CzyPanelAdminaOtwarty = true;
        }
    }
}
