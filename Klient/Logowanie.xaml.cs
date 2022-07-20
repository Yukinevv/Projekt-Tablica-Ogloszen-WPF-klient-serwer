using BibliotekaEncje.Encje;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public partial class Logowanie : Page
    {
        public static TextBox TextBoxLogowanie;

        public Logowanie()
        {
            InitializeComponent();

            TextBoxLogowanie = TextBoxLogin;
        }

        private void RejestracjaButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.rama.Content = new Rejestracja();
        }

        private void LogowanieButton_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxLogin.Text == string.Empty || PassBoxHaslo.Password == string.Empty)
            {
                MessageBox.Show("Uzupelnij wszystkie pola!");
                return;
            }

            // hashuje podane przez uzytkownika haslo
            SHA256 sha256Hash = SHA256.Create();
            string hash = Rejestracja.GetHash(sha256Hash, PassBoxHaslo.Password);

            OperacjeKlient.Wyslij("LOGOWANIE");
            OperacjeKlient.Wyslij(TextBoxLogin.Text);
            OperacjeKlient.Odbierz();
            OperacjeKlient.Wyslij(hash);
            string czyZalogowac = OperacjeKlient.Odbierz();

            if (czyZalogowac == "true")
            {
                MainWindow.rama.Content = new StronaGlowna();
            }
            else
            {
                MessageBox.Show("Nieprawidlowe dane!");
            }
        }
    }
}
