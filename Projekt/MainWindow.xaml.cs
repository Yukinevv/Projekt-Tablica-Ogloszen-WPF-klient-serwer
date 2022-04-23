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
using System.Threading;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int id;
        public string[] ComboBox1Options { get; set; }
        public string[] ComboBox2Options { get; set; }

        private int[] sortIndexes;

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
            TextBlock1.Text = "Dostepni uzytkownicy:\n(hasło do jankowalski: qwerty123)\n";
            foreach (string elements in Connect.SelectRecords())
            {
                TextBlock1.Text += elements + "\n";
            }

            // pokaz opcje comboboxa
            ComboBox1Options = new string[] { "Rosnąco", "Malejąco" };
            ComboBox1.ItemsSource = ComboBox1Options;

            ComboBox2Options = new string[] { "Id", "Tytuł", "Kategoria" };
            ComboBox2.ItemsSource = ComboBox2Options;
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

            ZatwierdzButton.Visibility = Visibility.Hidden;
            UsunButton.Visibility = Visibility.Hidden;

            try
            {
                List<string> ogloszenia = new List<string>();
                //ogloszenia = Connect.SelectRecordsOgloszenia();
                ogloszenia = (List<string>)ListBox1.ItemsSource;

                string[] tmp = new string[1000];
                //tmp = ogloszenia[ListBox1.SelectedIndex].Split(' ');
                tmp = ogloszenia[ListBox1.SelectedIndex].Split('\t');

                Tytul.Text = tmp[2];
                Kategoria.Text = tmp[3];
                //Tresc.Text = "";
                /*for (int i = 6; i < tmp.Length; i++)
                {
                    Tresc.Text += tmp[i] + " ";
                }*/
                Tresc.Text = tmp[6];

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
                    //tmp2 = ListBox1.SelectedItem.ToString().Split(' ');
                    tmp2 = ListBox1.SelectedItem.ToString().Split('\t');
                    cmd.Parameters.AddWithValue("_id_u", int.Parse(tmp2[1]));
                    //cmd.Parameters.AddWithValue("_id_u", int.Parse(ListBox1.SelectedItem.ToString()[2].ToString()));

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
                    //tmp = ListBox1.SelectedItem.ToString().Split(' ');
                    tmp = ListBox1.SelectedItem.ToString().Split('\t');
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

            try
            {
                ListBox1.ItemsSource = Connect.SelectRecordsOgloszenia();
                PoliczOgloszenia();
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
                ListBox1.ItemsSource = Connect.SelectRecordsOgloszenia();
                PoliczOgloszenia();
            }
            catch (Exception err)
            {
                MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak 2", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ComboBox1.SelectedItem = null;
            ComboBox2.SelectedItem = null;
            ComboBox1.Text = "Jak sortować";
            ComboBox2.Text = "Sortuj według";
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

            try
            {
                ListBox1.ItemsSource = Connect.SelectRecordsOgloszenia();
                PoliczOgloszenia();
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

                    string[] tmp = new string[1000];
                    //tmp = ListBox1.SelectedItem.ToString().Split(' ');
                    tmp = ListBox1.SelectedItem.ToString().Split('\t');
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

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> tmp = Connect.SelectRecordsOgloszenia();
            
            if(ComboBox2.SelectedItem == null)
            {
                tmp.Sort();
                if ((string)ComboBox1.SelectedItem == "Rosnąco")
                {
                    ListBox1.ItemsSource = tmp;
                }
                else if ((string)ComboBox1.SelectedItem == "Malejąco")
                {
                    tmp.Reverse();
                    ListBox1.ItemsSource = tmp;
                }
            }
            else
            {
                List<string> newList = new List<string>();

                if ((string)ComboBox1.SelectedItem == "Rosnąco")
                {
                    for (int i = tmp.Count - 1; i >= 0; i--)
                    {
                        newList.Add(tmp[sortIndexes[i]]);
                    }
                    ListBox1.ItemsSource = newList;
                }
                else if ((string)ComboBox1.SelectedItem == "Malejąco")
                {
                    for (int i = 0; i < tmp.Count; i++)
                    {
                        newList.Add(tmp[sortIndexes[i]]);
                    }
                    ListBox1.ItemsSource = newList;
                }
            }
        }

        private void ComboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> tmp = Connect.SelectRecordsOgloszenia();
            string[] tmp2;

            string[] sortElements = new string[1000];
            int[] indeksy = new int[1000];
            int i;

            for(i = 0; i < tmp.Count; i++)
            {
                tmp2 = tmp[i].Split('\t');
                if((string)ComboBox2.SelectedItem == "Id")
                {
                    sortElements[i] = tmp2[0];                
                }
                else if((string)ComboBox2.SelectedItem == "Tytuł")
                {
                    sortElements[i] = tmp2[2];
                }
                else if ((string)ComboBox2.SelectedItem == "Kategoria")
                {
                    sortElements[i] = tmp2[3];
                }
                indeksy[i] = i;
            }
            sortIndexes = BubbleSortWithIndexes(sortElements, indeksy);
        }

        private static int[] BubbleSortWithIndexes(string[] arr, int[] indeksy)
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    try
                    {
                        if (string.Compare(arr[j], arr[j + 1]) < 0)
                        {
                            (arr[j + 1], arr[j]) = (arr[j], arr[j + 1]);

                            (indeksy[j + 1], indeksy[j]) = (indeksy[j], indeksy[j + 1]);
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Blad: " + err.Message, "Cos poszlo nie tak", MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown();
                    }
                }
            }
            return indeksy;
        }

    }
}
