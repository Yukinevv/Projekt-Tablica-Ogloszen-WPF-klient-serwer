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
        }

        private void ZarejestrujButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxImie.Text == string.Empty || TextBoxNazwisko.Text == string.Empty || TextBoxLogin.Text == string.Empty
                || TextBoxEmail.Text == string.Empty || TextBoxHaslo1.Text == string.Empty || TextBoxHaslo2.Text == string.Empty
                || DatePicker1.SelectedDate.ToString() == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }

            OperacjeKlient.Wyslij("REJESTRACJA");
            Uzytkownik uzytkownik = new Uzytkownik()
            {
                Imie = TextBoxImie.Text,
                Nazwisko = TextBoxNazwisko.Text,
                Login = TextBoxLogin.Text,
                Email = TextBoxEmail.Text,
                Haslo = TextBoxHaslo1.Text,
                Data_ur = (DateTime)DatePicker1.SelectedDate,
                Uprawnienia = "uzytkownik"
            };
            string uzytkownikSerialized = JsonConvert.SerializeObject(uzytkownik);

            byte[] buffer = Encoding.ASCII.GetBytes(uzytkownikSerialized);
            OperacjeKlient.clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None); // wyslanie rzadania do serwera

            MainWindow.rama.Content = new StronaGlowna();
            StronaGlowna.textBoxInfo.Text = "Udana rejestracja!\n Teraz mozesz sie zalogowac.";
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new StronaGlowna();
        }
    }
}
