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
    /// Logika interakcji dla klasy PanelAdminaMenu.xaml
    /// </summary>
    public partial class PanelAdminaMenu : Page
    {
        private static bool posortowano = false;

        private static List<Uzytkownik> UzytkownicyKopia;

        public PanelAdminaMenu()
        {
            InitializeComponent();

            OperacjeKlient.Wyslij("UZYTKOWNICY");
            OperacjeKlient.Wyslij(Logowanie.TextBoxLogowanie.Text);
            string uzytkownicySerialized = OperacjeKlient.Odbierz();
            var uzytkownicy = JsonConvert.DeserializeObject<List<Uzytkownik>>(uzytkownicySerialized);
            uzytkownicy = uzytkownicy.OrderBy(u => u.Id).ToList();
            ListViewUzytkownicy.ItemsSource = uzytkownicy;
            UzytkownicyKopia = uzytkownicy;
        }

        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var kolumna = (sender as GridViewColumnHeader);
            var uzytkownicy = (List<Uzytkownik>)ListViewUzytkownicy.ItemsSource;

            if (posortowano == false)
            {
                if (kolumna.Tag.ToString() == "Id")
                {
                    uzytkownicy = uzytkownicy.OrderBy(u => u.Id).ToList();
                }
                else if (kolumna.Tag.ToString() == "Login")
                {
                    uzytkownicy = uzytkownicy.OrderBy(u => u.Login).ToList();
                }
                ListViewUzytkownicy.ItemsSource = uzytkownicy;
                UzytkownicyKopia = uzytkownicy;
                posortowano = true;
            }
            else
            {
                if (kolumna.Tag.ToString() == "Id")
                {
                    uzytkownicy = uzytkownicy.OrderByDescending(u => u.Id).ToList();
                }
                else if (kolumna.Tag.ToString() == "Login")
                {
                    uzytkownicy = uzytkownicy.OrderByDescending(u => u.Login).ToList();
                }
                ListViewUzytkownicy.ItemsSource = uzytkownicy;
                UzytkownicyKopia = uzytkownicy;
                posortowano = false;
            }
        }

        private void TextBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var uzytkownicy = (List<Uzytkownik>)ListViewUzytkownicy.ItemsSource;
            if (TextBoxFilter.Text == string.Empty)
            {
                uzytkownicy = UzytkownicyKopia;
            }
            else
            {
                uzytkownicy = uzytkownicy.Where(u => u.Login.Contains(TextBoxFilter.Text)).ToList();
            }
            ListViewUzytkownicy.ItemsSource = uzytkownicy;
        }

        private void ListViewUzytkownicy_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListViewUzytkownicy.SelectedItem == null)
            {
                return;
            }

            PanelAdmina.rama.Content = new PanelAdminaUzytkownicy();

            var uzytkownicy = (List<Uzytkownik>)ListViewUzytkownicy.ItemsSource;

            PanelAdminaUzytkownicy.TextBoxImieUzytkownika.Text = uzytkownicy[ListViewUzytkownicy.SelectedIndex].Imie;
            PanelAdminaUzytkownicy.TextBoxNazwiskoUzytkownika.Text = uzytkownicy[ListViewUzytkownicy.SelectedIndex].Nazwisko;
            PanelAdminaUzytkownicy.TextBoxLoginUzytkownika.Text = uzytkownicy[ListViewUzytkownicy.SelectedIndex].Login;
            PanelAdminaUzytkownicy.TextBoxEmailUzytkownika.Text = uzytkownicy[ListViewUzytkownicy.SelectedIndex].Email;
            PanelAdminaUzytkownicy.TextBoxDataUrodzeniaUzytkownika.Text = uzytkownicy[ListViewUzytkownicy.SelectedIndex].Data_ur.ToString();
            PanelAdminaUzytkownicy.TextBoxRolaUzytkownika.Text = uzytkownicy[ListViewUzytkownicy.SelectedIndex].Uprawnienia;
        }
    }
}
