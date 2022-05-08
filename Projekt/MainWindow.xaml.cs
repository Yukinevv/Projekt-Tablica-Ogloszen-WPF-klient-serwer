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
        public static int id;

        public MainWindow()
        {
            InitializeComponent();

            // pokaz i ukryj odpowiednie gridy - roboczo
            logowanie.Visibility = (Visibility)0;
            TextBlock1.Visibility = (Visibility)0;

            rejestracja.Visibility = Visibility.Hidden;
            LabelPassError.Visibility = Visibility.Hidden;
            program.Visibility = Visibility.Hidden;
            dodajOgloszenie.Visibility = Visibility.Hidden;
            edycjaOgloszenia.Visibility = Visibility.Hidden;

            rezultatDodania.Visibility = Visibility.Hidden;
            rezultatEdycji.Visibility = Visibility.Hidden;

            // wypisz dosetpne konta uzytkownikow - roboczo
            TextBlock1.Text = "Dostepni uzytkownicy:\n(hasło do jank: qwerty123)\n";
            foreach (string elements in Connect.SelectRecords())
            {
                TextBlock1.Text += elements + "\n";
            }

            // pokaz opcje comboboxa
            ComboBox1.ItemsSource = new string[] { "Rosnąco", "Malejąco" };
            ComboBox2.ItemsSource = new string[] { "Id_o", "Tytul", "Kategoria" };
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia();
            if(ComboBox2.SelectedItem == null)
            {
                if ((string)ComboBox1.SelectedItem == "Rosnąco")
                {
                    ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                    ListView1.ItemsSource = ogloszenia;
                }
                else if ((string)ComboBox1.SelectedItem == "Malejąco")
                {
                    ogloszenia = ogloszenia.OrderByDescending(x => x.Id_o).ToList();
                    ListView1.ItemsSource = ogloszenia;
                }
            }
            else
            {
                if ((string)ComboBox1.SelectedItem == "Rosnąco")
                {
                    switch (ComboBox2.SelectedItem as string)
                    {
                        case "Id_o":
                            ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                            break;

                        case "Tytul":
                            ogloszenia = ogloszenia.OrderBy(x => x.Tytul).ToList();
                            break;

                        //case "Kategoria":
                        //    ogloszenia = ogloszenia.OrderBy(x => x.Kategoria).ToList();
                        //    break;
                    }
                    ListView1.ItemsSource = ogloszenia;
                }
                else if ((string)ComboBox1.SelectedItem == "Malejąco")
                {
                    switch (ComboBox2.SelectedItem as string)
                    {
                        case "Id_o":
                            ogloszenia = ogloszenia.OrderByDescending(x => x.Id_o).ToList();
                            break;

                        case "Tytul":
                            ogloszenia = ogloszenia.OrderByDescending(x => x.Tytul).ToList();
                            break;

                        //case "Kategoria":
                        //    ogloszenia = ogloszenia.OrderByDescending(x => x.Kategoria).ToList();
                        //    break;
                    }
                    ListView1.ItemsSource = ogloszenia;
                }
            }  
        }

        public Predicate<object> GetFilter()
        {
            switch (ComboBox2.SelectedItem as string)
            {
                case "Id_o":
                    return IdoFilter;
                case "Tytul":
                    return TytulFilter;
                //case "Kategoria":
                //    return KategoriaFilter;
            }
            return IdoFilter;
        }

        private bool IdoFilter(object obj)
        {
            var Filterobj = obj as Ogloszenia;
            return Filterobj.Id_o.ToString().Contains(FilterTextBox.Text);
        }

        private bool TytulFilter(object obj)
        {
            var Filterobj = obj as Ogloszenia;
            return Filterobj.Tytul.Contains(FilterTextBox.Text);
        }

        //private bool KategoriaFilter(object obj)
        //{
        //    var Filterobj = obj as Ogloszenia;
        //    return Filterobj.Kategoria.Contains(FilterTextBox.Text);
        //}

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FilterTextBox.Text == null)
            {
                ListView1.Items.Filter = null;
            }
            else
            {
                ListView1.Items.Filter = GetFilter();
            }
        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView1.Items.Filter = GetFilter();
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
                    string query = @"SELECT * FROM login(:_login, :_haslo)"; //login() jest funkcja w postgresie

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    // hashowanie hasla w celu porownania zahashowanego znajdujacego sie w bazie
                    SHA256 sha256Hash = SHA256.Create();
                    string hash = Operacje.GetHash(sha256Hash, PassBox.Password);

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
                        int iloscOgloszen = Operacje.PoliczOgloszenia();
                        IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";

                        // wypisz nazwe uzytkownika
                        WitajLabel.Content = $"Witaj {LoginBox.Text}!";

                        // pokaz grid glownego layoutu
                        program.Visibility = (Visibility)0;
                        logowanie.Visibility = (Visibility)1;

                        // wypisz dostepne ogloszenia posortowane rosnaco po id_o
                        TextBlock1.Visibility = Visibility.Hidden;
                        List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia();
                        ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                        ListView1.ItemsSource = ogloszenia;
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

        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListView1.SelectedItem == null)
            {
                return;
            }    

            program.Visibility = Visibility.Hidden;
            edycjaOgloszenia.Visibility = Visibility.Visible;

            ZatwierdzButton.Visibility = Visibility.Hidden;
            UsunButton.Visibility = Visibility.Hidden;

            try
            {
                List<Ogloszenia> ogloszenia = (List<Ogloszenia>)ListView1.ItemsSource;
                Tytul.Text = ogloszenia[ListView1.SelectedIndex].Tytul;
                Tresc.Text = ogloszenia[ListView1.SelectedIndex].Tresc;

                //sprawdzenie uprawnien zalogowanego uzytkownika do edycji i usuwania wybranego ogloszenia
                using (NpgsqlConnection conn = Connect.GetConnection())
                {
                    string query = @"SELECT U.login FROM uzytkownicy U JOIN ogloszenia O ON U.id=O.id_u WHERE O.id_u=:_id_u OR U.uprawnienia='admin'";
                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("_id_u", ogloszenia[ListView1.SelectedIndex].Id_u);

                    string login = "";

                    conn.Open();

                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            login = (string)reader["login"];

                            if (login == LoginBox.Text)
                            {
                                ZatwierdzButton.Visibility = Visibility.Visible;
                                UsunButton.Visibility = Visibility.Visible;
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
                    string hash = Operacje.GetHash(sha256Hash, PassBox1.Password);

                    cmd.Parameters.AddWithValue("_login", TextBoxLogin.Text);
                    cmd.Parameters.AddWithValue("_haslo", hash);
                    cmd.Parameters.AddWithValue("_imie", TextBoxImie.Text);
                    cmd.Parameters.AddWithValue("_nazwisko", TextBoxNazwisko.Text);
                    cmd.Parameters.AddWithValue("_email", TextBoxEmail.Text);
                    //cmd.Parameters.AddWithValue("_data_ur", DatePicker1.SelectedDate);
                    cmd.Parameters.AddWithValue("_data_ur", DatePicker1.SelectedDate.ToString());

                    // warunki poprawnosci hasla
                    if (PassBox1.Password.Length < 8 || PassBox1.Password.Length > 20)
                    {
                        LabelPassError.Visibility = (Visibility)0;
                        LabelPassError.Content = "Niepoprawna długosc hasła!";
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

        private void ZatwierdzButton_Click(object sender, RoutedEventArgs e)
        {
            using (NpgsqlConnection conn = Connect.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"UPDATE ogloszenia SET tytul=:_tytul, tresc=:_tresc, data_ed=:_data_ed WHERE id_o=:_id_o";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("_tytul", Tytul.Text);
                    cmd.Parameters.AddWithValue("_tresc", Tresc.Text);
                    cmd.Parameters.AddWithValue("_data_ed", DateTime.Now.ToString("yyyy-MM-dd"));

                    List<Ogloszenia> ogloszenia = (List<Ogloszenia>)ListView1.ItemsSource;
                    cmd.Parameters.AddWithValue("_id_o", ogloszenia[ListView1.SelectedIndex].Id_o);

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

            try
            {
                if (ComboBox1.SelectedIndex == 0 && ComboBox2.SelectedIndex == 0)
                {
                    List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia();
                    ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                    ListView1.ItemsSource = ogloszenia;
                }
                int iloscOgloszen = Operacje.PoliczOgloszenia();
                IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OdswiezButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia();
                ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                ListView1.ItemsSource = ogloszenia;

                int iloscOgloszen = Operacje.PoliczOgloszenia();
                IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ComboBox1.SelectedIndex = 0;
            ComboBox2.SelectedIndex = 0;
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
                    //string query = @"INSERT INTO ogloszenia VALUES(
                    //                 nextval('increment_id_ogloszenia'), :_id_u, :_tytul, now(), now(), :_tresc)";

                    string query = @"INSERT INTO ogloszenia VALUES(
                                     nextval('increment_id_ogloszenia'), :_id_u, :_tytul, :_data_utw, :_data_ed, :_tresc)";

                    NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("_tytul", TytulD.Text);
                    cmd.Parameters.AddWithValue("_tresc", TrescD.Text);
                    cmd.Parameters.AddWithValue("_id_u", id);
                    cmd.Parameters.AddWithValue("_data_utw", DateTime.Now.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("_data_ed", DateTime.Now.ToString("yyyy-MM-dd"));

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

            try
            {
                if(ComboBox1.SelectedIndex == 0 && ComboBox2.SelectedIndex == 0)
                {
                    List<Ogloszenia> ogloszenia = Connect.SelectRecordsOgloszenia();
                    ogloszenia = ogloszenia.OrderBy(x => x.Id_o).ToList();
                    ListView1.ItemsSource = ogloszenia;
                }
                int iloscOgloszen = Operacje.PoliczOgloszenia();
                IloscOgloszenLabel.Content = $"Wyświetlono {iloscOgloszen} ogłoszeń/nia";
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                    List<Ogloszenia> ogloszenia = (List<Ogloszenia>)ListView1.ItemsSource;
                    cmd.Parameters.AddWithValue("_id_o", ogloszenia[ListView1.SelectedIndex].Id_o);

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
