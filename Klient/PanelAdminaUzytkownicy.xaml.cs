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
    /// Logika interakcji dla klasy PanelAdminaUzytkownicy.xaml
    /// </summary>
    public partial class PanelAdminaUzytkownicy : Page
    {
        public static TextBox TextBoxImieUzytkownika;

        public static TextBox TextBoxNazwiskoUzytkownika;

        public static TextBox TextBoxLoginUzytkownika;

        public static TextBox TextBoxEmailUzytkownika;

        public static TextBox TextBoxDataUrodzeniaUzytkownika;

        public static TextBox TextBoxRolaUzytkownika;

        public PanelAdminaUzytkownicy()
        {
            InitializeComponent();

            TextBoxImieUzytkownika = TextBoxImie;
            TextBoxNazwiskoUzytkownika = TextBoxNazwisko;
            TextBoxLoginUzytkownika = TextBoxLogin;
            TextBoxEmailUzytkownika = TextBoxEmail;
            TextBoxDataUrodzeniaUzytkownika = TextBoxDataUrodzenia;
            TextBoxRolaUzytkownika = TextBoxRola;
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            PanelAdmina.rama.Content = new PanelAdminaMenu();
        }

        private void AwansujButton_Click(object sender, RoutedEventArgs e)
        {
            OperacjeKlient.Wyslij("AWANS UZYTKOWNIKA");
            OperacjeKlient.Wyslij(TextBoxLoginUzytkownika.Text);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "awansowano")
            {
                TextBoxRolaUzytkownika.Text = "admin";
                MessageBox.Show("Użytkownik został awansowany na administratora!");
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }

        private void ZdegradujButton_Click(object sender, RoutedEventArgs e)
        {
            OperacjeKlient.Wyslij("ZDEGRADOWANIE UZYTKOWNIKA");
            OperacjeKlient.Wyslij(TextBoxLoginUzytkownika.Text);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "zdegradowano")
            {
                TextBoxRolaUzytkownika.Text = "uzytkownik";
                MessageBox.Show("Użytkownik administrator zostal zdegradowany do rangi zwykłego użytkownika!");
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }
        }
    }
}
