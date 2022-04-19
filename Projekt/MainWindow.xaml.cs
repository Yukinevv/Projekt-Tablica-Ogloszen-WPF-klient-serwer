﻿using System;
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
using System.Threading;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int id;

        public MainWindow()
        {
            InitializeComponent();

            // pokaz i ukryj odpowiednie gridy - roboczo
            logowanie.Visibility = (Visibility)0;
            TextBlock1.Visibility = (Visibility)0;

            rejestracja.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Hidden;
            dodajOgloszenie.Visibility = Visibility.Hidden;
            edycjaOgloszenia.Visibility = Visibility.Hidden;

            rezultatDodania.Visibility = Visibility.Hidden;
            rezultatEdycji.Visibility = Visibility.Hidden;

            // wypisz dosetpne konta uzytkownikow - roboczo
            TextBlock1.Text = "Dostepni uzytkownicy: (hasło do jankowalski: qwerty123)\n";
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
                        // wez id zalogowanego uzytkownika
                        string query2 = @"SELECT id FROM uzytkownicy WHERE login=:_login";
                        NpgsqlCommand cmd2 = new NpgsqlCommand(query2, conn);

                        cmd2.Parameters.AddWithValue("_login", LoginBox.Text);

                        using (NpgsqlDataReader reader = cmd2.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                id = int.Parse(reader["id"].ToString());
                            }
                        }

                        // policz ilosc dostepnych ogloszen
                        PoliczOgloszenia();

                        // wypisz nazwe uzytkownika
                        WitajLabel.Content = $"Witaj {LoginBox.Text}!";

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

        private void PoliczOgloszenia()
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    string query = @"SELECT COUNT(*) AS ile FROM ogloszenia";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    int iloscOgloszen = 0;

                    conn.Open();

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            iloscOgloszen = int.Parse(reader["ile"].ToString());
                        }
                    }
                    IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }          
        }

        private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            program.Visibility = Visibility.Hidden;
            edycjaOgloszenia.Visibility = Visibility.Visible;

            ZatwierdzButton.Visibility = Visibility.Visible;
            UsunButton.Visibility = Visibility.Visible;

            try
            {
                List<string> ogloszenia = new List<string>();
                ogloszenia = Connect.SelectRecordsOgloszenia();

                string[] tmp = new string[1000];
                tmp = ogloszenia[ListBox1.SelectedIndex].Split(' ');

                Tytul.Text = tmp[2];
                Kategoria.Text = tmp[3];
                Tresc.Text = "";
                for (int i = 8; i < tmp.Length; i++)
                {
                    Tresc.Text += tmp[i] + " ";
                }

                // test
                /*List<Ogloszenia> ogloszenia = new List<Ogloszenia>();
                ogloszenia = Connect.SelectRecordsOgloszenia2();

                Tytul.Text = ogloszenia[ListBox1.SelectedIndex].Tytul;
                Kategoria.Text = ogloszenia[ListBox1.SelectedIndex].Kategoria;
                Tresc.Text = ogloszenia[ListBox1.SelectedIndex].Tresc;*/

                // sprawdzenie uprawnien zalogowanego uzytkownika do edycji i usuwania wybranego ogloszenia
                using (NpgsqlConnection conn = Connect.GetConnection())
                {
                    string query = @"SELECT U.login FROM uzytkownicy U JOIN ogloszenia O ON U.id=O.id_u WHERE O.id_u=:_id_u OR U.uprawnienia='admin'";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    string[] tmp2 = new string[1000];
                    tmp2 = ListBox1.SelectedItem.ToString().Split(' ');
                    cmd.Parameters.AddWithValue("_id_u", int.Parse(tmp2[1]));
                    //cmd.Parameters.AddWithValue("_id_u", int.Parse(ListBox1.SelectedItem.ToString()[2].ToString()));

                    string login = "";

                    conn.Open();

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            login = (string)reader["login"];

                            if (login != LoginBox.Text)
                            {
                                ZatwierdzButton.Visibility = Visibility.Hidden;
                                UsunButton.Visibility = Visibility.Hidden;
                                break;
                            }
                        }
                    }
                }     
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ZatwierdzButton_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"UPDATE ogloszenia SET tytul=:_tytul, kategoria=:_kategoria, tresc=:_tresc WHERE id_o=:_id_o";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("_tytul", Tytul.Text);
                    cmd.Parameters.AddWithValue("_kategoria", Kategoria.Text);
                    cmd.Parameters.AddWithValue("_tresc", Tresc.Text);
                    
                    string[] tmp = new string[1000];
                    tmp = ListBox1.SelectedItem.ToString().Split(' ');
                    cmd.Parameters.AddWithValue("_id_o", int.Parse(tmp[0]));
                    //cmd.Parameters.AddWithValue("_id_o", int.Parse(ListBox1.SelectedItem.ToString()[0].ToString()));

                    int n = cmd.ExecuteNonQuery();
                    if (n == 1)
                    {
                        rezultatEdycji.Visibility = Visibility.Visible;
                        rezultatEdycji.Content = "Zedytowano ogłoszenie!";
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            rezultatEdycji.Visibility = Visibility.Hidden;

            edycjaOgloszenia.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Visible;

            ListBox1.ItemsSource = Connect.SelectRecordsOgloszenia();
            PoliczOgloszenia();
        }

        private void DodajOgoszenieButton_Click(object sender, RoutedEventArgs e)
        {
            program.Visibility = Visibility.Hidden;
            dodajOgloszenie.Visibility = Visibility.Visible;
        }

        private void ZatwierdzButtonD_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO ogloszenia VALUES(
                                     nextval('increment_id_ogloszenia'), :_id_u, :_tytul, :_kategoria, now(), now(), :_tresc)";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("_tytul", TytulD.Text);
                    cmd.Parameters.AddWithValue("_kategoria", KategoriaD.Text);
                    cmd.Parameters.AddWithValue("_tresc", TrescD.Text);
                    cmd.Parameters.AddWithValue("_id_u", id);

                    int n = cmd.ExecuteNonQuery();
                    if (n == 1)
                    {
                        rezultatDodania.Visibility = Visibility.Visible;
                        rezultatDodania.Content = "Dodano nowe ogłoszenie!";
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PowrotButtonD_Click(object sender, RoutedEventArgs e)
        {
            rezultatDodania.Visibility = Visibility.Hidden;

            dodajOgloszenie.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Visible;

            ListBox1.ItemsSource = Connect.SelectRecordsOgloszenia();
            PoliczOgloszenia();
        }

        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"DELETE FROM ogloszenia WHERE id_o = :_id_o";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    string[] tmp = new string[1000];
                    tmp = ListBox1.SelectedItem.ToString().Split(' ');
                    cmd.Parameters.AddWithValue("_id_o", int.Parse(tmp[0]));
                    //cmd.Parameters.AddWithValue("_id_o", int.Parse(ListBox1.SelectedItem.ToString()[0].ToString()));   

                    int n = cmd.ExecuteNonQuery();
                    if (n == 1)
                    {
                        rezultatEdycji.Visibility = Visibility.Visible;
                        rezultatEdycji.Content = "Usunieto ogloszenie!";
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
