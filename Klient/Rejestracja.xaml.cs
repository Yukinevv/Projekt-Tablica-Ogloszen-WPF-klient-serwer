using Newtonsoft.Json;
using BibliotekaEncje.Encje;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
using System.Security.Cryptography;

namespace Klient
{
    /// <summary>
    /// Logika interakcji dla klasy Rejestracja.xaml
    /// </summary>
    public partial class Rejestracja : Page
    {
        public Rejestracja()
        {
            InitializeComponent();

            DatePicker1.DisplayDateEnd = DateTime.Today;
            DatePicker1.DisplayDateStart = DateTime.Parse("1900-01-01");
        }

        private void ZarejestrujButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxImie.Text == string.Empty || TextBoxNazwisko.Text == string.Empty || TextBoxLogin.Text == string.Empty
                || TextBoxEmail.Text == string.Empty || PassBoxHaslo1.Password == string.Empty || PassBoxHaslo2.Password == string.Empty
                || DatePicker1.SelectedDate.ToString() == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }
            else if (PassBoxHaslo1.Password != PassBoxHaslo2.Password)
            {
                MessageBox.Show("Podane hasła nie są takie same!");
                return;
            }
            else if (PassBoxHaslo1.Password.Length < 8 || PassBoxHaslo1.Password.Length > 20)
            {
                MessageBox.Show("Hasło powinno miec długość od 8 do 20 znaków");
                return;
            }
            else if (!TextBoxEmail.Text.Contains("@"))
            {
                MessageBox.Show("Nieprawidłowy format adresu Email");
                return;
            }

            // hashuje podane przez uzytkownika haslo
            SHA256 sha256Hash = SHA256.Create();
            string hash = GetHash(sha256Hash, PassBoxHaslo1.Password);

            OperacjeKlient.Wyslij("REJESTRACJA");      
            Uzytkownik uzytkownik = new Uzytkownik()
            {
                Imie = TextBoxImie.Text,
                Nazwisko = TextBoxNazwisko.Text,
                Login = TextBoxLogin.Text,
                Email = TextBoxEmail.Text,
                Haslo = hash,
                Data_ur = (DateTime)DatePicker1.SelectedDate,
                Uprawnienia = "uzytkownik"
            };
            string uzytkownikSerialized = JsonConvert.SerializeObject(uzytkownik);
            OperacjeKlient.Wyslij(uzytkownikSerialized);

            string odpowiedz = OperacjeKlient.Odbierz();
            if (odpowiedz == "zarejestrowano")
            {
                MessageBox.Show("Udana rejestracja!\n Teraz mozesz sie zalogowac.");
                MainWindow.rama.Content = new Logowanie();        
            }
            else
            {
                MessageBox.Show(odpowiedz);
            }        
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new Logowanie();
        }

        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
