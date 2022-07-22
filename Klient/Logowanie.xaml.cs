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

        private static string[] daneLogowania = new string[2];

        public Logowanie()
        {
            InitializeComponent();

            TextBoxLogowanie = TextBoxLogin;

            daneLogowania = Ustawienia.WczytajDaneLogowania();

            if (TextBoxLogowanie.Text == string.Empty && PassBoxHaslo.Password == string.Empty)
            {
                TextBoxLogowanie.Text = daneLogowania[0];
                PassBoxHaslo.Password = daneLogowania[1];
            }
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

            if (CheckBoxZapamietaj.IsChecked == true)
            {
                Ustawienia.ZapiszDaneLogowania(TextBoxLogowanie.Text, PassBoxHaslo.Password);
            }

            // hashuje podane przez uzytkownika haslo
            SHA256 sha256Hash = SHA256.Create();
            string hash = Rejestracja.GetHash(sha256Hash, PassBoxHaslo.Password);

            OperacjeKlient.Wyslij("LOGOWANIE");
            OperacjeKlient.Wyslij(TextBoxLogowanie.Text);
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

        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LogowanieButton_Click(null, null);
            }
        }
    }
}
