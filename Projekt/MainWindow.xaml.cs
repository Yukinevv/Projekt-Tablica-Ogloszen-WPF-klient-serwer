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
using Npgsql;
using System.Security.Cryptography;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // pokaz i ukryj odpowiednie gridy - roboczo
            logowanie.Visibility = (Visibility)0;
            TextBlock1.Visibility = (Visibility)0;

            rejestracja.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Hidden;
            edycjaOgloszenia.Visibility = Visibility.Hidden;

            // wypisz dosetpne konta uzytkownikow - roboczo
            TextBlock1.Text = "Dostepni uzytkownicy:\n";
            foreach (string elements in Connect.SelectRecords())
            {
                TextBlock1.Text += elements + "\n";
            }
        }

        private void LoginBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginBox.Text = "";
            LoginBox.TextAlignment = 0;
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            rejestracja.Visibility = (Visibility)1;
            // pokaz grid logowania
            logowanie.Visibility = (Visibility)0;
            TextBlock1.Visibility = (Visibility)0;
        }

        private void ZarejestrujSieDoGrida_Click(object sender, RoutedEventArgs e)
        {
            logowanie.Visibility = (Visibility)1;
            // pokaz grid rejestracji
            rejestracja.Visibility = (Visibility)0;
            TextBlock1.Visibility = (Visibility)1;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    //string query = @"SELECT * FROM uzytkownicy WHERE login = :_login AND haslo = :_haslo";
                    string query = @"SELECT * FROM login(:_login, :_haslo)"; //login() jest funkcja w postgresie

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    // hashowanie hasla w celu porownania zahashowanego znajdujacego sie w bazie
                    SHA256 sha256Hash = SHA256.Create();
                    string hash = GetHash(sha256Hash, PassBox.Password);

                    cmd.Parameters.AddWithValue("_login", LoginBox.Text);
                    cmd.Parameters.AddWithValue("_haslo", hash);
                    
                    int result = (int)cmd.ExecuteScalar();

                    if (result == 1) // logowanie powiodlo sie
                    {   
                        // pokaz grid glownego layoutu
                        program.Visibility = (Visibility)0;
                        logowanie.Visibility = (Visibility)1;

                        // wypisz dostepne ogloszenia
                        TextBlock1.Visibility = Visibility.Hidden;
                        ListBox1.ItemsSource = Connect.SelectRecordsOgloszenia();
                    }
                    else
                    {
                        MessageBox.Show("Sprawdz swoje dane logowania", "Blad logowania", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        conn.Close();
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // pokaz grid logowania
            logowanie.Visibility = (Visibility)0;
            program.Visibility = (Visibility)1;

            // wypisz dostepne konta uztykownikow - roboczo
            TextBlock1.Visibility = Visibility.Visible;
            TextBlock1.Text = "Dostepni uzytkownicy:\n";
            foreach (string elements in Connect.SelectRecords())
            {
                TextBlock1.Text += elements + "\n";
            }
        }

        private void Rejestruj_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO uzytkownicy VALUES(
                                     nextval('increment_id_uzytkownicy'), :_login, :_haslo, :_imie, :_nazwisko, :_email, :_data_ur, 'user')";
                                    //increment_id_uzytkownicy jest to sekwencja robiaca za AUTO INCREMENT w postgresie

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    // hashowanie hasla
                    SHA256 sha256Hash = SHA256.Create();
                    string hash = GetHash(sha256Hash, PassBox1.Password);

                    cmd.Parameters.AddWithValue("_login", TextBoxLogin.Text);
                    cmd.Parameters.AddWithValue("_haslo", hash);
                    cmd.Parameters.AddWithValue("_imie", TextBoxImie.Text);
                    cmd.Parameters.AddWithValue("_nazwisko", TextBoxNazwisko.Text);
                    cmd.Parameters.AddWithValue("_email", TextBoxEmail.Text);
                    cmd.Parameters.AddWithValue("_data_ur", DatePicker1.SelectedDate);

                    // warunki poprawnosci hasla
                    if (PassBox1.Password.Length < 8 || PassBox1.Password.Length > 20)
                    {
                        LabelPassError.Visibility = (Visibility)0;
                        LabelPassError.Content = "Niepoprawna dlugosc hasla!";
                        conn.Close();
                        return;
                    }
                    if (PassBox1.Password != PassBox2.Password)
                    {
                        LabelPassError.Visibility = (Visibility)0;
                        LabelPassError.Content = "Podane hasła nie są takie same!";
                        conn.Close();
                        return;
                    }

                    int n = cmd.ExecuteNonQuery();
                    if (n == 1)
                    {
                        LabelPassError.Visibility = (Visibility)1;
                        TextBlock1.Visibility = (Visibility)0;
                        TextBlock1.Text = "Rejestracja zakonczona powodzeniem!";
                        // pokaz grid logowania
                        logowanie.Visibility = (Visibility)0;
                        rejestracja.Visibility = (Visibility)1;
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private void TextBoxImie_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxImie.Text = "";
            TextBoxImie.TextAlignment = 0;
        }

        private void TextBoxNazwisko_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxNazwisko.Text = "";
            TextBoxNazwisko.TextAlignment = 0;
        }

        private void TextBoxLogin_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            TextBoxLogin.Text = "";
            TextBoxLogin.TextAlignment = 0;
        }

        private void PassBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LabelPass2.Visibility = (Visibility)1;
        }
    }
}
